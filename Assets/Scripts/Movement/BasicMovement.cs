using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : Movement
{
    public override float MoveSpeed
    {
        get => base.MoveSpeed;
        set
        {
            base.MoveSpeed = value;
            animator.SetFloat("move_speed", IsMoving ? 0.9f : 0.1f);
        }
    }

    public override Vector2 MoveDir
    {
        get => base.MoveDir;
        set
        {
            base.MoveDir = value;
            spriteRenderer.flipX = MoveDir.x < 0;
            animator.SetFloat("direction", FacingBack ? 0.9f : 0.1f);
        }
    }
}
