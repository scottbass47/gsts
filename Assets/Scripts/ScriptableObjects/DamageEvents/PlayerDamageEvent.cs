using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Damage Event")]
public class PlayerDamageEvent : DamageEvent
{
    public override void OnDamage(DamageFilter filter, float amount)
    {
        var obj = filter.gameObject;
        var playerController = obj.GetComponent<PlayerController>();
        playerController.TakeDamage(amount);
    }
}
