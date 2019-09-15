using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedLight : MonoBehaviour
{
    [SerializeField] private CompoundLight animatedLight;
    public float intensity; 
    public Color color;
    public float innerRadius;
    public float outerRadius;

    public bool IsAnimating { get; set; } = true;

    [HideInInspector]
    public int AnimationMask;
    public static readonly int IntensityMask = 1 << 0;
    public static readonly int ColorMask = 1 << 1;
    public static readonly int InnerRadiusMask = 1 << 2;
    public static readonly int OuterRadiusMask = 1 << 3;

    private void Update()
    {
        if(IsAnimating && animatedLight != null)
        {
            if ((AnimationMask & IntensityMask) != 0) animatedLight.Intensity = intensity;
            if ((AnimationMask & ColorMask) != 0) animatedLight.Color = color;
            if ((AnimationMask & InnerRadiusMask) != 0) animatedLight.InnerRadius = innerRadius;
            if ((AnimationMask & OuterRadiusMask) != 0) animatedLight.OuterRadius = outerRadius;
        }
    }
}
