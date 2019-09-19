using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for enemies that shouldn't move. Really the only reason to have
// this is so that all enemies can have an IMovement.
public class NoMovement : MonoBehaviour, IMovement
{
    public float MoveSpeed { get => 0; set { } }
    public Vector2 MoveDir => Vector2.zero;
    public bool FacingRight => false;
    public bool FacingBack => false;
    public bool IsMoving => false;

    public void Idle()
    {
    }

    public void SetAnimator(Animator animator)
    {
    }

    public void SetMoveDir(Vector2 moveDir)
    {
    }
}
