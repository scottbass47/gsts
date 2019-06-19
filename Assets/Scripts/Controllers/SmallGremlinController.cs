using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;
using System;

public class SmallGremlinController : MonoBehaviour
{
    [SerializeField] private Transform feet;
    [SerializeField] private SmallGremlinSettings settings;
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
                    .MoveOnPath(settings.Speed, settings.TurningVelocity, moveAnim)
                .End()
            .End()
            .Build();
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(settings.AttackDelay);

        anim.SetTrigger("attack");

        yield return new WaitForSeconds(settings.AttackFrame);

        var dir = ai.Target.position - ai.Pos.position;
        var hit = Physics2D.Raycast(
            ai.Pos.position,
            dir,
            settings.AttackRange,
            LayerMask.GetMask("Player Feet")
        );

        if (hit.collider != null)
        {
            var player = hit.collider.gameObject;
            DamageManager.DealDamage(player);
        }

        yield return new WaitForSeconds(settings.AttackCooldown);

        isAttacking = false;
    }
}
