using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageManager 
{
    public static void DealDamage(GameObject to)
    {
        DealDamage(to, 1);
    }

    public static void DealDamage(GameObject to, float amount)
    {
        if (amount == 0) return;

        var health = to.GetComponentInParent<Health>();
        if (health == null) return;

        var damageFilter = to.GetComponentInParent<DamageFilter>();
        if(damageFilter)
        {
            if (damageFilter.IsInvulnerable) return;
        }

        health.Amount -= amount;

        if (damageFilter)
        {
            damageFilter.DamageEvent?.OnDamage(to, amount);
        }
    }
}
