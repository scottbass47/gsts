using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDomeAnimationHelper : MonoBehaviour
{
    public HopState HopState;
    public float yOff;
    public float RotationPercent;
    public bool LaserAttackFinished;

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

    public void OnAttackStart()
    {
        LaserAttackFinished = false;
    }

    public void OnAttackEnd()
    {
        LaserAttackFinished = true;
    }
}

public enum HopState
{
    Landed,
    Hopping,
    InAir
}

