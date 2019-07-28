using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Death : MonoBehaviour
{
    private Health health;

    public bool DestroyOnDeath { get; set; } = true;
    public event Action OnDeath;
    public event Action OnDeathAnimFinish;
    private bool deathTriggered = false;

    private void Start()
    {
        health = GetComponent<Health>();
    }

    public void OnAnimationFinish()
    {
        OnDeathAnimFinish?.Invoke();
    }

    private void Update()
    {
        if(health.Amount < 0 && !deathTriggered)
        {
            deathTriggered = true;
            OnDeath?.Invoke();
            if(DestroyOnDeath) Destroy(gameObject);
        }                
    }
}
