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
    private GunController gun;
    private Transform targetBody;
    private ParticleSystem particles;

    private bool isAttacking;
    private bool isShuffling;

    private float attackCooldown;

    private void Start()
    {
        movement = GetComponent<Movement>();
        moveAnim = GetComponent<MovementAnimator>();
        gun = GetComponentInChildren<GunController>();
        particles = GetComponent<ParticleSystem>();
        ai = GetComponent<AIController>();
        ai.Pos = feet;

        var playerBody = GameManager.Instance.Player.GetComponent<Body>();
        ai.Target = playerBody.CenterFeet;
        targetBody = playerBody.CenterBody;

        ai.Tree = CreateTree();

        var health = GetComponent<Health>();
        health.Amount = stats.Health;
    }

    private IBehaviourTreeNode CreateTree()
    {
        return new TreeBuilder(ai)
            .Selector("Main")
                //.do("", t =>
                //{
                //    gun.aimat(targetbody.position, ai.pos.position);
                //    moveanim.setlookangle(ai.target.position - ai.pos.position, false);
                //    gun.setdraworder(!moveanim.isback);
                //    gun.shoot();
                //    return behaviourtreestatus.success;
                //})
                .Sequence("Attack")
                    .Do("", t => { attackCooldown -= t.deltaTime; return BehaviourTreeStatus.Success; })
                    .Selector("")
                        .Sequence("")
                            .LOS()
                            .Range(stats.AttackRange)
                            .Condition("Cooldown Over", t => attackCooldown < 0 && !isShuffling)
                        .End()
                        .Condition("Is Attacking", t => isAttacking)
                    .End()
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
                    .Condition("Cooling Down", t => attackCooldown > 0 || isShuffling)
                    .Do("Shuffle", t =>
                    {
                        if (!isShuffling)
                        {
                            StartCoroutine(Shuffle());
                        }
                        return BehaviourTreeStatus.Success;
                    })
                    .Do("Aim", t => Aim(targetBody.position))
                .End()
                .Sequence("Follow")
                    .GetPath()
                    .MoveOnPath(stats.Speed, stats.TurningVelocity, moveAnim)
                    // This sets the target to be aimed at to be an imaginary point in the direction
                    // the enemy is moving in.
                    .Do("Aim", t => Aim(5 * ai.MoveDir + (Vector2)ai.Pos.position))
                .End()
            .End()
            .Build();
    }

    private BehaviourTreeStatus Aim(Vector2 target)
    {
        gun.AimAt(target, ai.Pos.position);
        gun.SetDrawOrder(!moveAnim.IsBack);
        return BehaviourTreeStatus.Success;
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(stats.AttackAnticipation);

        anim.SetTrigger("attack");
        particles.Clear();
        particles.Play();

        var dir = ai.Target.position - ai.Pos.position;

        bool colliding = false;
        int maxAttempts = 20;
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
        var aimTarget = targetBody.position - halfDash;

        StartCoroutine(Shoot(aimTarget, dashTime));
        while (t < dashTime)
        {
            t += Time.deltaTime;
            movement.AddForce(dashDir * dashSpeed);
            yield return null;
        }

        // Guarantees the shooting finishes before moving to shuffling
        yield return new WaitForSeconds(0.05f);

        isAttacking = false;
        anim.SetTrigger("dash_over");
        particles.Stop();

        attackCooldown = stats.AttackCooldown;
    }

    private IEnumerator Shoot(Vector2 target, float dashTime)
    {
        gun.AimAt(target, ai.Pos.position);
        int bullets = 3;
        int shot = 0;
        while(shot < bullets)
        {
            shot++;
            gun.Shoot();
            //var bullet = Instantiate(bulletPrefab, ai.Pos.position, Quaternion.identity);
            //bullet.transform.position = ai.Pos.position;
            //var bulletComp = bullet.GetComponent<Bullet>();
            //bulletComp.Speed = 10f;
            //bulletComp.Damage = 1f;
            //bulletComp.Shoot(dir);
            yield return new WaitForSeconds(dashTime / (bullets - 1));
        }
    }

    private IEnumerator Shuffle()
    {
        isShuffling = true;
        var center = ai.Pos.position; 

        while(attackCooldown > 0)
        {
            yield return StartCoroutine(PickPointAndMove(center));
        }

        isShuffling = false;
    }
    
    private IEnumerator PickPointAndMove(Vector2 center)
    {
        bool colliding = false;
        int maxAttempts = 10;
        int attemptNum = 0;
        var dir = Vector2.zero;
        do
        {
            dir = Random.insideUnitCircle.normalized;
            var result = Physics2D.Raycast(center, dir, stats.ShuffleDistance, LayerMask.GetMask("Wall"));
            colliding = result.rigidbody != null;
            attemptNum++;
        } while (colliding && attemptNum < maxAttempts);

        var moveDir = (center + dir * stats.ShuffleDistance) - (Vector2)ai.Pos.position;
        moveDir.Normalize();

        var time = stats.ShuffleDistance / stats.Speed;
        while(time > 0)
        {
            time -= Time.deltaTime;
            movement.AddForce(moveDir * stats.ShuffleSpeed);
            moveAnim.SetLookAngle(ai.Target.position - ai.Pos.position, true);
            yield return null;
        }
    }
}
