using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDroneMovement : MonoBehaviour, IMovement
{
    private Animator animator;
    private new SpriteRenderer renderer;

    private float moveSpeed;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    private Vector2 moveDir;
    public Vector2 MoveDir => moveDir;
    public bool FacingRight => moveDir.x > 0;
    public bool FacingBack => moveDir.y > 0;
    public bool IsMoving => moveSpeed >= 0;

    public void Idle()
    {
        MoveSpeed = 0;
    }

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
        renderer = animator.GetComponent<SpriteRenderer>();
    }

    public void SetMoveDir(Vector2 moveDir)
    {
        this.moveDir = moveDir.normalized;
        animator.SetFloat("horizontal", Mathf.Abs(MoveDir.x));
        animator.SetFloat("vertical", MoveDir.y);
        renderer.flipX = !FacingRight;
    }
}
