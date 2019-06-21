using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractZone : MonoBehaviour
{
    public event Action<GameObject> OnEnter;
    public event Action<GameObject> OnExit;

    public bool ZoneEnabled { get; set; } = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ZoneEnabled) return;
        OnEnter?.Invoke(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!ZoneEnabled) return;
        OnExit?.Invoke(collision.gameObject);
    }
}

