using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBarDisplay : MonoBehaviour
{
    private Image image;

    private int ammoInClip;
    public int AmmoInClip
    {
        get => ammoInClip;
        set
        {
            if (value < 0 || value > AmmoInMag) return;
            ammoInClip = value;
            UpdateDisplay();
        }
    }

    private int ammoInMag = 1;
    public int AmmoInMag
    {
        get => ammoInMag;
        set
        {
            if (value < 1) return;
            ammoInMag = value;
            UpdateDisplay();
        }
    }

    private void Start()
    {
        image = GetComponent<Image>();       
    }

    private void UpdateDisplay()
    {
        image.fillAmount = AmmoInClip / (float)AmmoInMag;
    }
}
