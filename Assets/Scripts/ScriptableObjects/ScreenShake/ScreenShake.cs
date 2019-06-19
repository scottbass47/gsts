using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Screen Shake")]
public class ScreenShakeSettings : ScriptableObject
{
    [SerializeField] private float duration;
    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;

    public float Duration => duration;
    public float Amplitude => amplitude;
    public float Frequency => frequency;
}
