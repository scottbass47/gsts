using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;
using System;
using UnityEngine.Rendering;

public class GruntController : MonoBehaviour
{
    [SerializeField] private Transform feet;
    [SerializeField] private GruntStats stats;
    [SerializeField] private Animator anim; 

    private AIController ai;
    private MovementAnimator moveAnim;

    private bool isAttacking;

    private void Start()
    {
        moveAnim = GetComponent<MovementAnimator>();
        ai = GetComponent<AIController>();
        ai.Pos = feet;
        ai.Target = GameManager.Instance.Player.GetComponent<Movement>().HitboxCenter;
        ai.Tree = CreateTree();

        var health = GetComponent<Health>();
        health.Amount = stats.Health;
    }

    private IBehaviourTreeNode CreateTree()
    {
        return new TreeBuilder(ai)
            .Selector("Main")
                .Sequence("Attack")
                    .Selector("")
                        .Sequence("")
                            .LOS()
                            .Range(1.5f)
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

        yield return new WaitForSeconds(stats.AttackFrame);

        var dir = ai.Target.position - ai.Pos.position;
        var hit = Physics2D.Raycast(
            ai.Pos.position,
            dir,
            stats.AttackRange,
            LayerMask.GetMask("Player Feet")
        );

        if (hit.collider != null)
        {
            var player = hit.collider.gameObject;
            DamageManager.DealDamage(player);
        }

        yield return new WaitForSeconds(stats.AttackCooldown);

        isAttacking = false;
    }
}
