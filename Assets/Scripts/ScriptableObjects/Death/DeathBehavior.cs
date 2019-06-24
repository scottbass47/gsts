using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeathBehavior : ScriptableObject
{
    public abstract void OnDeath(Death death);
}
