using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Settings")]
public class Settings : ScriptableObject
{
    public int GameWidth;
    public int GameHeight;
    public int ExtendedWidth => GameWidth + 32;
    public int ExtendedHeight => GameHeight + 18;
    public ScaleMode ScaleMode;
    public readonly int PPU = 16;
    public bool SubpixelMovement = true;

    public float OrthographicZoom => ExtendedHeight / (float)PPU * 0.5f;
    public float ResolutionRatio => ExtendedWidth / (float)ExtendedHeight;
}

public enum ScaleMode
{
    Stretch,
    ScaleToFit,
    PerfectFit
}
