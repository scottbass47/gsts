using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinterTasks : MonoBehaviour
{
    private AI ai;
    private SprinterStats sprinterStats => (SprinterStats)ai.EnemyStats;

    private bool isSprinting;

    [Task] public bool IsAttacking { get; set; }
    [Task] public bool IsCoolingDown { get; set; } 

    private PathFindingTasks pathFinding;

    public void Start()
    {
        ai = GetComponent<AI>();
        pathFinding = GetComponent<PathFindingTasks>();
        pathFinding.SetMovementParameters(sprinterStats.RunSpeed, sprinterStats.TurningVelocity);
        pathFinding.SetMoveSpeedFunction(() => isSprinting ? sprinterStats.SprintSpeed : sprinterStats.RunSpeed);
    }

    [Task]
    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
        Task.current.Succeed();
    }

    [Task]
    public void SetAttacking(bool attacking)
    {
        IsAttacking = attacking;
        Task.current.Succeed();
    }

    [Task]
    public void SetSprintCooldown(bool cooldown)
    {
        IsCoolingDown = cooldown;
        Task.current.Succeed();
    }
}

