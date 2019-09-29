using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarDisplay : MonoBehaviour
{
    [SerializeField] private Color fullColor;
    [SerializeField] private Color emptyColor;
    [SerializeField] private Color dividerColor;

    private int maxHealth = 1;
    public int MaxHealth
    {
        get => maxHealth;
        set
        {
            if (value < 1) return;
            maxHealth = value;
            UpdateImage();
        }
    }

    private int currentHealth = 1;
    public int Health
    {
        get => currentHealth;
        set
        {
            if (value < 0 || value > MaxHealth) return;
            currentHealth = value;
            UpdateImage();
        }
    }

    private readonly int barTexWidth = 97;
    private readonly int barTexHeight = 5;
    private int barWidthAcross => barTexWidth - barTexHeight + 1;
    private Texture2D barTex;
    private RawImage image;

    private int numDividers => maxHealth - 1;
    private readonly int dividerWidth = 2;
    private int healthWidth
    {
        get
        {
            if (numDividers == 0) return barWidthAcross;
            return ((barWidthAcross + dividerWidth) / maxHealth) - dividerWidth;
        }
    } 
    private int firstHealthWidth
    {
        get
        {
            return healthWidth + firstHealthExtraWidth;
        }
    }
    private int fullHealthThreshold
    {
        get
        {
            if (currentHealth == 0) return 0;
            var combinedWidth = healthWidth + dividerWidth;
            return firstHealthWidth + combinedWidth * (currentHealth - 1);
        }
    }
    private int firstHealthExtraWidth
    {
        get
        {
            return barWidthAcross - (numDividers * dividerWidth + healthWidth * maxHealth);
        }
    }

    private void Awake()
    {
        barTex = new Texture2D(barTexWidth, barTexHeight);
        barTex.filterMode = FilterMode.Point;
        image = GetComponent<RawImage>();
    }

    private void UpdateImage()
    {
        RedrawTexture();
        image.texture = barTex;
    }
    
    private void RedrawTexture()
    {
        for(int x = 0; x < barTexWidth; x++)
        {
            for(int y = 0; y < barTexHeight; y++)
            {
                barTex.SetPixel(x, y, GetPixelColor(x, y));
            }
        }
        barTex.Apply();
    }

    private Color GetPixelColor(int x, int y)
    {
        int xOff = barTexHeight - y - 1;
        if (x < xOff || x >= xOff + barWidthAcross) return Color.clear;

        int xNorm = x - xOff;

        var totalWidth = healthWidth + dividerWidth;

        bool isFirstSeg = xNorm <= firstHealthExtraWidth;
        bool isDivider = (xNorm - firstHealthExtraWidth) % totalWidth >= healthWidth;
        bool isEmpty = xNorm >= fullHealthThreshold && currentHealth != maxHealth;

        if (isDivider && !isFirstSeg)
        {
            return dividerColor;
        } 
        else
        {
            return isEmpty ? emptyColor : fullColor;
        }
    }

}
