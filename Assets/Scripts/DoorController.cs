﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private InteractZone zone;
    [SerializeField] private TextMeshProUGUI doorText;
    private bool inZone;
    private bool betweenWaves = true;
    private EventManager events;

    public event Action OnDoorInteract;

    private void Start()
    {
        zone = GetComponentInChildren<InteractZone>();

        zone.OnEnter += (gameObj) =>
        {
            inZone = true;
            doorText.enabled = inZone && betweenWaves;
        };

        zone.OnExit += (gameObj) =>
        {
            doorText.enabled = false;
            inZone = false;
        };

        events = GameManager.Instance.Events;
        enabled = true;

        events.AddListener<WaveStarted>((obj) => 
        {
            doorText.enabled = false;
            betweenWaves = false;
        });
        events.AddListener<WaveEnded>((obj) => 
        {
            betweenWaves = true;
        });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && inZone && zone.ZoneEnabled)
        {
            OnDoorInteract?.Invoke();
        }
    }
}
