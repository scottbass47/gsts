using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    float MoveSpeed { get; set; }
    Vector2 MoveDir { get; }
    bool FacingRight { get; }
    bool FacingBack { get; }
    bool IsMoving { get; }

    void SetMoveDir(Vector2 moveDir);
    void SetAnimator(Animator animator);
    void Idle();

}
