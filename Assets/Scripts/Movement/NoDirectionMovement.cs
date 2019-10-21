using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDirectionMovement : Movement
{
    public override float MoveSpeed
    {
        get => base.MoveSpeed;
        set
        {
            base.MoveSpeed = value;
            animator.SetFloat("move_speed", MoveSpeed);
        }
    }
}
