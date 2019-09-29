using Panda;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LaserDomeTasks : MonoBehaviour
{
    private AI ai;
    private AnimationTasks animationTasks;
    private LaserDomeAnimationHelper animationHelper;
    private PathFindingTasks pathFinding;

    private LaserDomeStats laserDomeStats => (LaserDomeStats)ai.EnemyStats;

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

    public void Start()
    {
        ai = GetComponent<AI>();
        animationTasks = GetComponent<AnimationTasks>();
        animationHelper = GetComponentInChildren<LaserDomeAnimationHelper>();
        pathFinding = GetComponentInChildren<PathFindingTasks>();
        pathFinding.SetMovementParameters(laserDomeStats.Speed, laserDomeStats.TurningVelocity);
        pathFinding.SetMoveSpeedFunction(() => hopStarted ? laserDomeStats.Speed : 0);

        laserObj = Instantiate(laserPrefab, laserSortingGroup.transform);
        laserObj.SetActive(false);
        laser = laserObj.GetComponent<Laser>();
    }

    [Task]
    public void SetHopping(bool hopping)
    {
        var task = Task.current;
        if (this.hopping == hopping)
        {
            task.Succeed();
            return;
        }

        this.hopping = hopping;
        animationTasks.Animator.SetBool("hopping", hopping);
        task.Succeed();
    }

    [Task]
    public void SetAttacking(bool attacking)
    {
        isAttacking = attacking;
        Task.current.Succeed();
    }

    private float lastAngle;

    [Task]
    public void ShootLaser()
    {
        var task = Task.current;

        if (task.isStarting)
        {
            shootingLaser = true;
            laserObj.SetActive(true);
            lastAngle = -90;
        }
        var toTarget = ai.Target.position - ai.Pos.position;
        var targetAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

        GameObject hitObj = null;

        if(AngleBetween(lastAngle, laserAngle, targetAngle))
        {
            hitObj = laser.Shoot(targetAngle);
            if (hitObj.CompareTag("Player"))
            {
                DamageManager.DealDamage(hitObj);
            }
        }
        hitObj = laser.Shoot(laserAngle);

        lastAngle = laserAngle;
        laserSortingGroup.sortingOrder = -(int)Mathf.Sign(Mathf.Sin(laserAngle * Mathf.Deg2Rad));

        if (animationHelper.LaserAttackFinished && !task.isStarting)
        {
            laserObj.SetActive(false);
            shootingLaser = false;
            Task.current.Succeed();
        }
    }

    private bool AngleBetween(float start, float end, float mid)
    {
        end = (end - start) < 0.0f ? end - start + 360.0f : end - start;
        mid = (mid - start) < 0.0f ? mid - start + 360.0f : mid - start;
        return mid < end;
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
}
