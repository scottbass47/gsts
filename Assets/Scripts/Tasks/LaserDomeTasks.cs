using Panda;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDomeTasks : BasicTasks
{
    protected override float PathSpeed => hopStarted ? laserDomeStats.Speed : 0;
    protected override float PathTurningVelocity => 2f;

    private LaserDomeAnimationHelper animationHelper;
    private bool hopping;
    private bool hopStarted;

    private LaserDomeStats laserDomeStats => (LaserDomeStats)stats;

    [SerializeField] private Transform hopTransform;
    private float hopTime = 0.4f;
    private float hopVelInitial => -laserDomeStats.HopGravity * hopTime * 0.5f;

    public override void Start()
    {
        base.Start();

        animationHelper = GetComponentInChildren<LaserDomeAnimationHelper>();
    }

    [Task]
    public void StartHopping()
    {
        if (hopping)
        {
            Task.current.Succeed();
            return;
        }
        hopping = true;
        hopStarted = false;
        animator.SetBool("hopping", true);
        StartCoroutine(HoppingRoutine());
        Task.current.Succeed();
    }

    private IEnumerator HoppingRoutine()
    {
        float yVel = 0;
        float y = 0;
        while (hopping)
        {
            if(animationHelper.HopState == HopState.InAir && !hopStarted)
            {
                hopStarted = true;
                yVel = hopVelInitial;
            }
            else if(animationHelper.HopState == HopState.Landed && hopStarted)
            {
                hopStarted = false;
                yVel = 0;
                y = 0;
                hopTransform.localPosition = Vector3.zero;
            }

            if (hopStarted)
            {
                yVel += laserDomeStats.HopGravity * Time.deltaTime;
                y += yVel * Time.deltaTime;

                hopTransform.localPosition = Vector3.up * y;
            }

            yield return null;
        }
    }
}
