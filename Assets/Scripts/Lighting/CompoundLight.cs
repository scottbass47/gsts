using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

[ExecuteInEditMode]
public class CompoundLight : MonoBehaviour, ILight
{
    [SerializeField] private Light2D[] lights;
    [SerializeField] private LightSettings settings;

    private float intensity;
    public float Intensity
    {
        get => intensity;
        set 
        {
            intensity = value;
            UpdateLights();
        }
    }
    private Color color;
    public Color Color
    {
        get => color;
        set 
        {
            color = value;
            UpdateLights();
        }
    }
    private float innerRadius;
    public float InnerRadius
    {
        get => innerRadius;
        set 
        {
            innerRadius = value;
            UpdateLights();
        }
    }

    private float outerRadius;
    public float OuterRadius
    {
        get => outerRadius;
        set 
        {
            outerRadius = value;
            UpdateLights();
        }
    }

    private void UpdateLights()
    {
        foreach(var light in lights)
        {
            light.intensity = intensity;
            light.color = color;
            light.pointLightInnerRadius = innerRadius;
            light.pointLightOuterRadius = outerRadius;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Application.isEditor/* && !Application.isPlaying*/)
        {
            intensity = settings.Intensity;
            color = settings.Color;
            innerRadius = settings.InnerRadius;
            outerRadius = settings.OuterRadius;
            UpdateLights();
        }
#endif
    }

    public void SetLightData(float intensity, Color color, float innerRadius, float outerRadius)
    {
        this.intensity = intensity;
        this.color = color;
        this.innerRadius = innerRadius;
        this.outerRadius = outerRadius;
        UpdateLights();
    }
}
