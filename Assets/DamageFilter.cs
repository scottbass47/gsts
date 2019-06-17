using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFilter : MonoBehaviour
{
    private bool isInvulnerable;

    public bool IsInvulnerable
    {
        set => isInvulnerable = value;
        get => isInvulnerable;
    }

    [SerializeField] private DamageEvent damageEvent;
    public DamageEvent DamageEvent => damageEvent; 
}

public abstract class DamageEvent : ScriptableObject
{
    public abstract void OnDamage(GameObject obj, float amount);
}
