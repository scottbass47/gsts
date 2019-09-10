using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDomeAnimationHelper : MonoBehaviour
{
    public HopState HopState;

    public void OnHopBegin()
    {
        HopState = HopState.Hopping;
    }

    public void OnHopEnd()
    {
        HopState = HopState.Landed;
    }

    public void OnHopBeginAir()
    {
        HopState = HopState.InAir;
    }
}

public enum HopState
{
    Landed,
    Hopping,
    InAir
}

