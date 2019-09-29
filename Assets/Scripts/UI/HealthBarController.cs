using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    private void Start()
    {
        var healthBarDisplay = GetComponent<HealthBarDisplay>();
        var playerStats = GetComponentInParent<HUDUtils>().PlayerStats;
        healthBarDisplay.MaxHealth = playerStats.MaxHealth;
        healthBarDisplay.Health = playerStats.MaxHealth;

        var events = GameManager.Instance.Events;
        events.AddListener<PlayerHealthEvent>(this.gameObject, (health) => 
        {
            healthBarDisplay.Health = health.Health;
        });
        
    }
}
