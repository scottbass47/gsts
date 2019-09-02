using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinterTasks : BasicTasks
{
    private SprinterStats sprinterStats => (SprinterStats)stats;

    private bool isSprinting;

    [Task] public bool IsAttacking { get; set; }
    [Task] public bool IsCoolingDown { get; set; } 

    protected override float PathSpeed => isSprinting ? sprinterStats.SprintSpeed : sprinterStats.RunSpeed;
    protected override float PathTurningVelocity => sprinterStats.TurningVelocity;

    [Task]
    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
        Task.current.Succeed();
    }

    [Task]
    public void Sprint()
    {
        movement.MoveSpeed = sprinterStats.SprintSpeed;
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

