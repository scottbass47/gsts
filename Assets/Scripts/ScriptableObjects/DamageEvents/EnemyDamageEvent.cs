using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Damage Event")]
public class EnemyDamageEvent : DamageEvent
{
    [SerializeField] private float flashDuration;

    public override void OnDamage(DamageFilter filter, float amount)
    {
        var obj = filter.gameObject;
        obj.GetComponent<AIController>().Flash(flashDuration);

        if(obj.GetComponent<Health>().Amount > 0)
        {
            SoundManager.PlaySound(Sounds.EnemyFleshHit);
        }
    }
}
