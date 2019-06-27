using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Extend BaseEnemy controller for the MovementAnimator to work

// For now, we'll check the Rigidbody to see if it's moving
[RequireComponent(typeof(Rigidbody2D))]
public class MovementAnimator : MonoBehaviour {

    private Vector2 dir;
    public Animator bodyAnim;
    public SpriteRenderer bodyRenderer;
    public Rigidbody2D rb2d;

    private bool isMoving;
    private bool isBack;
    public bool IsBack => isBack;
    private bool isAttacking = false;

    public void SetLookAngle(Vector2 dir, bool moving)
    {
        this.dir = dir;
        bodyRenderer.flipX = dir.x < 0;
        isBack = dir.y > 0;
        isMoving = moving;

        bodyAnim.SetFloat("direction", isBack ? 0.9f : 0.1f);
        bodyAnim.SetFloat("move_speed", isMoving ? 0.9f : 0.1f);
    }

    //public void LateUpdate()
    //{
    //    bool flipped = LookAngle > 90 && LookAngle < 270;
    //    bool back = LookAngle > 5 && LookAngle < 175;
    //    bool moving = rb2d.velocity.SqrMagnitude() > 0.1f;

    //    bodyRenderer.flipX = flipped;

    //    bodyAnim.SetFloat("direction", back ? 1.0f : 0.0f);
    //    bodyAnim.SetFloat("move_speed", moving ? 1.0f : 0.0f);
    //}
}
