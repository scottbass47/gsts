using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDirectionMovement : MonoBehaviour, IMovement
{
    private float moveSpeed;
    public float MoveSpeed
    {
        get => moveSpeed;
        set
        {
            moveSpeed = value;
            animator.SetFloat("move_speed", moveSpeed);
        }
    }

    private Vector2 moveDir;
    public Vector2 MoveDir => moveDir;

    public bool FacingRight => false;
    public bool FacingBack => false;

    public bool IsMoving => moveSpeed > 0;

    private Animator animator;

    public void Idle()
    {
        MoveSpeed = 0;
    }

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }

    public void SetMoveDir(Vector2 moveDir)
    {
        this.moveDir = moveDir;
    }
}
