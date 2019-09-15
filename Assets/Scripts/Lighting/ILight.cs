using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILight
{
    float Intensity
    {
        get;
        set;
    }

    Color Color
    {
        get;
        set;
    }

    float InnerRadius
    {
        get;
        set;
    }

    float OuterRadius
    {
        get;
        set;
    }

    void SetLightData(float intensity, Color color, float innerRadius, float outerRadius);
}
