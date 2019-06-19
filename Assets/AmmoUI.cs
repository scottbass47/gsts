using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoClip;
    [SerializeField] private TextMeshProUGUI ammoMag;

    private void Start()
    {
        var events = GameManager.Instance.Events;

        events.AddListener<WeaponClipChange>((eventObj) => 
        {
            ammoClip.SetText($"{eventObj.BulletsInClip}");
        });

        events.AddListener<WeaponMagChange>((eventObj) => 
        {
            ammoMag.SetText($"{eventObj.Bullets}");
        });
    }

}
