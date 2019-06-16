using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Extend BaseEnemy controller for the MovementAnimator to work

// For now, we'll check the Rigidbody to see if it's moving
[RequireComponent(typeof(Rigidbody2D))]
public class MovementAnimator : MonoBehaviour {

    // Angle from 0-360
    public float LookAngle
    {
        get => lookAngle;
        set
        {
            lookAngle = value - 360 * Mathf.Floor(value / 360f);
        }
    }
    private float lookAngle;
    public Animator bodyAnim;
    public SpriteRenderer bodyRenderer;
    public Rigidbody2D rb2d;

    private bool isMoving;
    private bool isBack;
    private bool isAttacking = false;

    public void LateUpdate()
    {
        bool flipped = LookAngle > 90 && LookAngle < 270;
        bool back = LookAngle > 5 && LookAngle < 175;
        bool moving = rb2d.velocity.SqrMagnitude() > 0.1f;

        bodyRenderer.flipX = flipped;

        bodyAnim.SetFloat("direction", back ? 1.0f : 0.0f);
        bodyAnim.SetFloat("move_speed", moving ? 1.0f : 0.0f);
    }
}
