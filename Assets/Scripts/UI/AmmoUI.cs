using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoClip;
    [SerializeField] private TextMeshProUGUI ammoMag;
    [SerializeField] private Image ammoBar;

    private int magSize;

    private void Start()
    {
        var events = GameManager.Instance.Events;

        events.AddListener<WeaponClipChange>((eventObj) => 
        {
            ammoClip.SetText($"{eventObj.BulletsInClip}");
            ammoBar.fillAmount = eventObj.BulletsInClip / (float)magSize;
        });

        events.AddListener<WeaponMagChange>((eventObj) => 
        {
            magSize = eventObj.Bullets;
            ammoMag.SetText($"{eventObj.Bullets}");
        });
    }

}
