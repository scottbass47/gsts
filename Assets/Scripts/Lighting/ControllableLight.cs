using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

[ExecuteInEditMode]
public class ControllableLight : MonoBehaviour
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

    // For the animator
    [Header("Animation")]
    [ReadOnly] public float AnimateIntensity; 
    [ReadOnly] public Color AnimateColor;
    [ReadOnly] public float AnimateInnerRadius;
    [ReadOnly] public float AnimateOuterRadius;
    [ReadOnly] public bool IsAnimating;
    
    [HideInInspector] public int AnimationMask;
    public static readonly int IntensityMask = 1 << 0;
    public static readonly int ColorMask = 1 << 1;
    public static readonly int InnerRadiusMask = 1 << 2;
    public static readonly int OuterRadiusMask = 1 << 3;

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
        if (Application.isEditor && settings != null/* && !Application.isPlaying*/)
        {
            intensity = settings.Intensity;
            color = settings.Color;
            innerRadius = settings.InnerRadius;
            outerRadius = settings.OuterRadius;
            UpdateLights();
        }
#endif
        if (IsAnimating)
        {
            SetLightData(
                (AnimationMask & IntensityMask) != 0 ? AnimateIntensity : intensity,
                (AnimationMask & ColorMask) != 0 ? AnimateColor : color,
                (AnimationMask & InnerRadiusMask) != 0 ? AnimateInnerRadius : innerRadius,
                (AnimationMask & OuterRadiusMask) != 0 ? AnimateOuterRadius : outerRadius
            );
        }
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
