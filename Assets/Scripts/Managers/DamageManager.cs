using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageManager 
{
    public static bool DealDamage(GameObject to)
    {
        return DealDamage(to, 1);
    }

    public static bool DealDamage(GameObject to, float amount)
    {
        if (amount == 0) return false;

        var health = to.GetComponentInParent<Health>();
        if (health == null) return false;

        var damageFilter = to.GetComponentInParent<DamageFilter>();
        if(damageFilter)
        {
            if (damageFilter.IsInvulnerable) return false;
        }

        health.Amount -= amount;

        if (damageFilter)
        {
            damageFilter.DamageEvent?.OnDamage(damageFilter, amount);
        }
        return true;
    }
}
