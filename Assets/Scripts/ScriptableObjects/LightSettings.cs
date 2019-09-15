using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lights/Settings")]
public class LightSettings : ScriptableObject
{
    [SerializeField] private float intensity;
    [SerializeField] private Color color;
    [SerializeField] private float innerRadius;
    [SerializeField] private float outerRadius;

    public float Intensity => intensity;
    public Color Color => color;
    public float InnerRadius => innerRadius;
    public float OuterRadius => outerRadius;
}
