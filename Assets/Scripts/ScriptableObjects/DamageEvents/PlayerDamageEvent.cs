using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Damage Event")]
public class PlayerDamageEvent : DamageEvent
{
    public override void OnDamage(GameObject obj, float amount)
    {
        var playerController = obj.GetComponentInParent<PlayerController>();
        playerController.TakeDamage();
    }
}
