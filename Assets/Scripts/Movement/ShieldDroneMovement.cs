using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDroneMovement : Movement
{
    public override Vector2 MoveDir
    {
        get => base.MoveDir;
        set
        {
            base.MoveDir = value;
            animator.SetFloat("horizontal", Mathf.Abs(MoveDir.x));
            animator.SetFloat("vertical", MoveDir.y);
            spriteRenderer.flipX = !FacingRight;
        }
    }
}
