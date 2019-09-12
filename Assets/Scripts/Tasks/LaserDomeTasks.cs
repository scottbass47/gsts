using Panda;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LaserDomeTasks : BasicTasks
{
    protected override float PathSpeed => hopStarted ? laserDomeStats.Speed : 0;
    protected override float PathTurningVelocity => laserDomeStats.TurningVelocity;

    private LaserDomeAnimationHelper animationHelper;

    private LaserDomeStats laserDomeStats => (LaserDomeStats)stats;

    [SerializeField] private Transform hopTransform;
    private bool hopStarted;
    private bool hopping;

    [Task]
    public bool IsAttacking => isAttacking;
    private bool isAttacking;

    [Task] public bool Grounded => animationHelper.HopState == HopState.Landed;

    [SerializeField] private GameObject laserPrefab;
    private GameObject laserObj;
    private Laser laser;
    private float laserAngle => animationHelper.RotationPercent * 360 - 90;
    private bool shootingLaser;

    [SerializeField] private Transform enemySortingTransform;
    [SerializeField] private SortingGroup laserSortingGroup;

    public override void Start()
    {
        base.Start();

        animationHelper = GetComponentInChildren<LaserDomeAnimationHelper>();

        laserObj = Instantiate(laserPrefab, transform);
        laserObj.transform.parent = laserSortingGroup.transform;
        laserObj.SetActive(false);
        laser = laserObj.GetComponent<Laser>();
    }

    [Task]
    public void StartHopping()
    {
        SetHopping(true);
        Task.current.Succeed();
    }

    [Task]
    public void StopHopping()
    {
        SetHopping(false);
        Task.current.Succeed();
    }

    private void SetHopping(bool hopping)
    {
        if (this.hopping == hopping) return;

        this.hopping = hopping;
        animator.SetBool("hopping", hopping);
    }

    [Task]
    public void StartAttacking()
    {
        isAttacking = true;
        Task.current.Succeed();
    }

    [Task]
    public void StopAttacking()
    {
        isAttacking = false;
        Task.current.Succeed();
    }

    [Task]
    public void ShootLaser()
    {
        var task = Task.current;

        if (task.isStarting)
        {
            shootingLaser = true;
            laserObj.SetActive(true);
        }

        var hitObj = laser.Shoot(laserAngle);


        laserSortingGroup.sortingOrder = -(int)Mathf.Sign(Mathf.Sin(laserAngle * Mathf.Deg2Rad));

        if (hitObj.CompareTag("Player"))
        {
            DamageManager.DealDamage(hitObj);
        }

        if (animationHelper.LaserAttackFinished && !task.isStarting)
        {
            laserObj.SetActive(false);
            shootingLaser = false;
            Task.current.Succeed();
        }
    }

    private void Update()
    {
        if (hopping || isAttacking)
        {
            hopTransform.localPosition = Vector3.up * (animationHelper.yOff / laserDomeStats.HopScale);
            if (animationHelper.HopState == HopState.InAir && !hopStarted)
            {
                hopStarted = true;
            }
            else if (animationHelper.HopState == HopState.Landed && hopStarted)
            {
                hopStarted = false;
            }
        }
        else
        {
            hopTransform.localPosition = Vector3.zero; 
        }
    }

    //private IEnumerator HoppingRoutine()
    //{
    //    float yVel = 0;
    //    float y = 0;
    //    while (hopping)
    //    {
    //        if(animationHelper.HopState == HopState.InAir && !hopStarted)
    //        {
    //            hopStarted = true;
    //            yVel = hopVelInitial;
    //        }
    //        else if(animationHelper.HopState == HopState.Landed && hopStarted)
    //        {
    //            hopStarted = false;
    //            yVel = 0;
    //            y = 0;
    //            hopTransform.localPosition = Vector3.zero;
    //        }

    //        if (hopStarted)
    //        {
    //            yVel += laserDomeStats.HopGravity * Time.deltaTime;
    //            y += yVel * Time.deltaTime;

    //            hopTransform.localPosition = Vector3.up * y;
    //        }

    //        yield return null;
    //    }
    //}
}
