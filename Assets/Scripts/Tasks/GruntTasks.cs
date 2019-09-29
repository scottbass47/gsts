using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntTasks : MonoBehaviour
{
    [SerializeField] private Transform feet;

    [Task]
    public bool IsAttacking { get; set; }

    private GruntStats gruntStats => (GruntStats)ai.EnemyStats;

    private AI ai;
    private Rigidbody2D rb2d;
    private PathFindingTasks pathFinding;

    public void Start()
    {
        ai = GetComponent<AI>();
        rb2d = GetComponent<Rigidbody2D>();
        pathFinding = GetComponent<PathFindingTasks>();
        pathFinding.SetMovementParameters(gruntStats.Speed, gruntStats.TurningVelocity);
    }

    [Task]
    public void Bite()
    {
        var dir = ai.Target.position - ai.Pos.position;
        var hit = Physics2D.Raycast(
            ai.Pos.position,
            dir,
            gruntStats.AttackRange,
            LayerMask.GetMask("Player Feet")
        );

        if (hit.collider != null)
        {
            var player = hit.collider.gameObject;
            DamageManager.DealDamage(player);
        }
        Task.current.Succeed();
    }

    [Task]
    public void SetAttacking (bool attacking)
    {
        IsAttacking = attacking;
        Task.current.Succeed();
    }

    [Task]
    public void FreezePosition()
    {
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        Task.current.Succeed();
    }

    [Task]
    public void UnfreezePosition()
    {
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        Task.current.Succeed();
    }
}
