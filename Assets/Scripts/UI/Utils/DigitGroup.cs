using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DigitGroup
{
    private int places;

    private int[] digits;
    public int[] Digits => digits;

    private int maxValue;
    public int MaxValue => maxValue;

    private int value;

    public DigitGroup(int places)
    {
        if (places < 1) throw new ArgumentException("Can't create digit group with less than 1 digit places.");
        this.places = places;
        digits = new int[places];
        maxValue = (int)Mathf.Pow(10, places) - 1; 
    }

    public void SetValue(int value)
    {
        if (value > maxValue) throw new ArgumentException($"Value {value} is greater than the max value.");
        this.value = value;

        for (int i = 0; i < digits.Length; i++)
        {
            digits[i] = value % 10;
            value /= 10;
        }
    }

    public bool IsDigitUsed(int place)
    {
        return value >= Mathf.Pow(10, place);
    }

    public int GetDigitValue(int place)
    {
        return Digits[place];
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < digits.Length; i++)
        {
            sb.Append(digits[i]);
        }
        return sb.ToString();
    }
}
