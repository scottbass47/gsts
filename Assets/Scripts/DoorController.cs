using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private InteractZone zone;
    [SerializeField] private TextMeshProUGUI doorText;

    private void Start()
    {
        zone = GetComponentInChildren<InteractZone>();

        zone.OnEnter += (gameObj) =>
        {
            doorText.enabled = true;
        };

        zone.OnExit += (gameObj) =>
        {
            doorText.enabled = false;
        };

        var events = GameManager.Instance.Events;
        enabled = true;

        events.AddListener<WaveStarted>((obj) => 
        {
            zone.ZoneEnabled = false;
        });
        events.AddListener<WaveEnded>((obj) => 
        {
            zone.ZoneEnabled = true;
        });
    }
}
