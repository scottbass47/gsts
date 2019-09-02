using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStats : ScriptableObject
{
    public abstract float Health { get; }

    public T GetStat<T>(string name)
    {
        return (T)this.GetType().GetProperty(name).GetValue(this);
    }
}
