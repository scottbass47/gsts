using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : ScriptableObject
{
    public T GetStat<T>(string name)
    {
        return (T)this.GetType().GetProperty(name).GetValue(this);
    }
}
