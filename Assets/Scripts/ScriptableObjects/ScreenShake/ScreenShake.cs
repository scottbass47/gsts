using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(menuName = "Game/Screen Shake")]
public class ScreenShakeSettings : ScriptableObject
{
    [SerializeField] private float duration;
    [SerializeField] private NoiseSettings noise;


    public float Duration => duration;
    public NoiseSettings Noise => noise;
}
