using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;
using System;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class DasherController : MonoBehaviour
{
    [SerializeField] private Transform feet;
    [SerializeField] private DasherStats stats;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject bulletPrefab;

    private AIController ai;
    private MovementAnimator moveAnim;
    private Movement movement;

    private bool isAttacking;

    private float attackCooldown;

    private void Start()
    {
        movement = GetComponent<Movement>();
        moveAnim = GetComponent<MovementAnimator>();
        ai = GetComponent<AIController>();
        ai.Pos = feet;
        //ai.Target = GameManager.Instance.Player.GetComponent<Movement>().HitboxCenter;
        ai.Target = target;
        ai.Tree = CreateTree();

        var health = GetComponent<Health>();
        health.Amount = stats.Health;
    }

    private IBehaviourTreeNode CreateTree()
    {
        return new TreeBuilder(ai)
            .Selector("Main")
                .Sequence("Attack")
                    .Do("", t => { attackCooldown -= t.deltaTime; return BehaviourTreeStatus.Success; })
                    .Selector("")
                        .Sequence("")
                            .LOS()
                            .Range(stats.AttackRange)
                            .Condition("Cooldown Over", t => attackCooldown < 0)
                        .End()
                        .Condition("Is Attacking", t => isAttacking)
                    .End()
                    .StopMoving()
                    .Do("Attack", t =>
                    {
                        if (!isAttacking)
                        {
                            StartCoroutine(Attack());
                        }
                        return BehaviourTreeStatus.Success;
                    })
                .End()
                .Sequence("Post-Attack")
                    .Condition("Cooling Down", t => attackCooldown > 0)
                    .Do("Shuffle", t =>
                    {

                        return BehaviourTreeStatus.Success;
                    })
                    //.StopMoving()
                .End()
                .Sequence("Follow")
                    .GetPath()
                    .MoveOnPath(stats.Speed, stats.TurningVelocity, moveAnim)
                .End()
            .End()
            .Build();
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(stats.AttackDelay);

        anim.SetTrigger("attack");

        var dir = ai.Target.position - ai.Pos.position;

        bool colliding = false;
        int maxAttempts = 10;
        int attemptNum = 0;
        var dashDir = Vector3.zero;
        do
        {
            var rotation = 90 + Random.Range(-10f, 10f);
            rotation = Random.Range(0f, 1f) > 0.5f ? -rotation : rotation;
            dashDir = Quaternion.Euler(0, 0, rotation) * dir;
            dashDir.Normalize();
            var result = Physics2D.Raycast(ai.Pos.position, dashDir, stats.DashDistance, LayerMask.GetMask("Wall"));
            colliding = result.rigidbody != null;
            attemptNum++;
        } while (colliding && attemptNum < maxAttempts);

        Debug.DrawLine(ai.Pos.position, ai.Pos.position + stats.DashDistance * dashDir, Color.cyan, 1f, false);

        float dashTime = stats.DashTime;
        float dashSpeed = stats.DashDistance / dashTime;
        float t = 0;

        var halfDash = dashDir * dashSpeed * dashTime * 0.5f;
        var halfDashPos = halfDash + ai.Pos.position;
        var aimVec = ai.Target.position - halfDashPos;

        StartCoroutine(Shoot(aimVec, dashTime));
        while(t < dashTime)
        {
            t += Time.deltaTime;
            movement.AddForce(dashDir * dashSpeed);
            yield return null;
        }

        isAttacking = false;

        attackCooldown = stats.AttackCooldown;
    }

    private IEnumerator Shoot(Vector2 dir, float dashTime)
    {
        int bullets = 3;
        int shot = 0;
        while(shot < bullets)
        {
            shot++;
            var bullet = Instantiate(bulletPrefab, ai.Pos.position, Quaternion.identity);
            bullet.transform.position = ai.Pos.position;
            var bulletComp = bullet.GetComponent<Bullet>();
            bulletComp.Speed = 10f;
            bulletComp.Damage = 1f;
            bulletComp.Shoot(dir);
            yield return new WaitForSeconds(dashTime / (bullets - 1));
        }
    }
}
