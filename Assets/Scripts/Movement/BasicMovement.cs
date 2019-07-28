using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour, IMovement
{
    private Animator bodyAnim;
    private SpriteRenderer bodyRenderer;

    private float moveSpeed;
    public float MoveSpeed
    {
        get => moveSpeed;
        set
        {
            moveSpeed = value;
            bodyAnim.SetFloat("move_speed", IsMoving ? 0.9f : 0.1f);
        }
    }

    private Vector2 moveDir;
    public Vector2 MoveDir => moveDir;
    public bool FacingRight => moveDir.x > 0;
    public bool FacingBack => moveDir.y > 0;
    public bool IsMoving => moveSpeed != 0;

    public void Idle()
    {
        MoveSpeed = 0;
    }

    public void SetMoveDir(Vector2 moveDir)
    {
        this.moveDir = moveDir.normalized;
        bodyRenderer.flipX = MoveDir.x < 0;
        bodyAnim.SetFloat("direction", FacingBack ? 0.9f : 0.1f);
    }

    public void SetAnimator(Animator animator)
    {
        bodyAnim = animator;
        bodyRenderer = animator.GetComponent<SpriteRenderer>();
    }
}
