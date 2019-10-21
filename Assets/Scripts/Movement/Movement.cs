using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    public bool Locked { get; set; }

    private float moveSpeed;
    public virtual float MoveSpeed
    {
        get
        {
            return Locked ? 0 : moveSpeed;
        }
        set  
        {
            moveSpeed = value;
        }
    }

    private Vector2 moveDir;
    public virtual Vector2 MoveDir
    {
        get => moveDir;
        set => moveDir = value.normalized;
    }

    public virtual bool FacingRight => moveDir.x > 0; 
    public virtual bool FacingBack => moveDir.y > 0;
    public virtual bool IsMoving => MoveSpeed > 0; 

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
    }

    public virtual void Idle()
    {
        MoveSpeed = 0;
    }

}
