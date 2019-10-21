using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for enemies that shouldn't move. Really the only reason to have
// this is so that all enemies can have an IMovement.
public class NoMovement : Movement
{
    public override float MoveSpeed { get => 0; }
}
