using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class DigitsDisplay : MonoBehaviour
{
    [SerializeField] private GameObject[] digits;
    [SerializeField] private bool alignLeft;

    [Header("Appearance")]
    [SerializeField] private Color onColor;
    [SerializeField] private Color offColor;
    [SerializeField] private TMP_FontAsset font;

    private DigitGroup digitGroup;
    private int maxDigits => digits.Length;

    private int value;
    public int Value
    {
        set
        {
            if (value < 0 || value > digitGroup.MaxValue) return;
            this.value = value;
            SetDigitGroupValue(value);
            UpdateDisplay(); 
        }
        get => value;
    }

    private void Start()
    {
        digitGroup = new DigitGroup(maxDigits);
        digitGroup.SetValue(value);

        foreach (var digit in digits) SetDigitFont(digit, font);

        UpdateDisplay(); 
    }

    private void SetDigitGroupValue(int value)
    {
        digitGroup.SetValue(value);
    }

    private void UpdateDisplay()
    {
        if (alignLeft)
        {
            UpdateDisplayAlignLeft();
        }
        else
        {
            UpdateDisplayRightAlign();
        }
    }

    private void UpdateDisplayRightAlign()
    {
        for (int i = 0; i < digits.Length; i++)
        {
            var digit = digits[i];
            SetDigitColor(digit, digitGroup.IsDigitUsed(i) ? onColor : offColor);
            SetDigitValue(digit, digitGroup.GetDigitValue(i));
        }
    }

    private void UpdateDisplayAlignLeft()
    {
        int numUnusedDigits = NumberOfUnusedDigits();
        for (int i = 0; i < digits.Length; i++)
        {
            var digit = digits[i];
            if(i < numUnusedDigits)
            {
                SetDigitColor(digit, offColor);
                SetDigitValue(digit, 0);
            }
            else
            {
                SetDigitColor(digit, onColor);
                SetDigitValue(digit, digitGroup.GetDigitValue(i - numUnusedDigits));
            }
        }
    }

    private int NumberOfUnusedDigits()
    {
        int numUnused = 0;
        for(int i = 0; i < digits.Length; i++)
        {
            if(!digitGroup.IsDigitUsed(i))
            {
                numUnused++;
            }
        }
        return numUnused;
    }

    private void SetDigitValue(GameObject digit, int value)
    {
        digit.GetComponent<TextMeshProUGUI>().text = value.ToString();
    }

    private void SetDigitColor(GameObject digit, Color color)
    {
        digit.GetComponent<TextMeshProUGUI>().color = color;
    }

    private void SetDigitFont(GameObject digit, TMP_FontAsset font)
    {
        digit.GetComponent<TextMeshProUGUI>().font = font;
    }
} 

