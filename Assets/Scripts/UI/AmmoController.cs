using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoController : MonoBehaviour
{
    [SerializeField] private AmmoBarDisplay ammoBarDisplay;
    [SerializeField] private DigitsDisplay ammoMagDisplay;
    [SerializeField] private DigitsDisplay ammoClipDisplay;

    private void Start()
    {
        var events = GameManager.Instance.Events;

        events.AddListener<WeaponMagChange>(this.gameObject, (magChangeEvent) => 
        {
            ammoBarDisplay.AmmoInMag = magChangeEvent.Bullets;
            ammoMagDisplay.Value = magChangeEvent.Bullets;
        });

        events.AddListener<WeaponClipChange>(this.gameObject, (clipChangeEvent) => 
        {
            ammoBarDisplay.AmmoInClip = clipChangeEvent.BulletsInClip;
            ammoClipDisplay.Value = clipChangeEvent.BulletsInClip;
        });
    }
}
