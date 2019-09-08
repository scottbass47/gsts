using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Settings")]
public class Settings : ScriptableObject
{
    public int GameWidth;
    public int GameHeight;
    public ScaleMode ScaleMode;
    public readonly int PPU = 16;

    public float OrthographicZoom => GameHeight / (float)PPU * 0.5f;
    public float ResolutionRatio => GameWidth / (float)GameHeight;
}

public enum ScaleMode
{
    Stretch,
    ScaleToFit,
    PerfectFit
}
