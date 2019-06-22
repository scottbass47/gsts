using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Death : MonoBehaviour
{
    private Health health;

    public event Action OnDeath;

    private void Start()
    {
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if(health.Amount < 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }                
    }
}
