using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;
using System;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using Panda;

public class DasherTasks : BasicTasks
{
    [SerializeField] private Transform feet;
    [SerializeField] private DasherStats stats;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Texture2D dashForward;
    [SerializeField] private Texture2D dashBackward;

    private Physics physics;
    private GunController gun;
    private Transform targetBody;
    private ParticleSystem particles;

    private float attackCooldown;

    private Coroutine shuffle;

    private Action onDrawGizmoAction;
    private List<Vector3> patrolWaypoints;
    private int currPatrolWaypoint;
    private int bulletsToShoot;
    private Vector2 dashDir;

    [Task]
    public bool MoreWaypoints => currPatrolWaypoint < patrolWaypoints.Count;

    [Task]
    public bool IsActing { get; set; }

    [Task]
    public bool StillShooting => bulletsToShoot > 0;

    public override void Awake()
    {
        base.Awake();
        patrolWaypoints = new List<Vector3>();

        Speed = stats.Speed;
        TurningVelocity = stats.TurningVelocity;
    }

    public override void Start()
    {
        base.Start();
        physics = GetComponent<Physics>();
        movement = GetComponent<BasicMovement>();
        gun = GetComponentInChildren<GunController>();
        particles = GetComponent<ParticleSystem>();
        ai.Pos = feet;

        var playerBody = GameManager.Instance.Player.GetComponent<Body>();
        ai.Target = playerBody.CenterFeet;
        targetBody = playerBody.CenterBody;

        var health = GetComponent<Health>();
        health.Amount = stats.Health;
        enemyStats = stats;
    }

    [Task]
    public void TargetInAttackRange()
    {
        TargetInRange(stats.AttackRange);
    }

    [Task]
    public void StartAction()
    {
        IsActing = true;
        Task.current.Succeed();
    }

    [Task]
    public void StopAction()
    {
        IsActing = false;
        Task.current.Succeed();
    }

    [Task]
    public void PickNumberOfShots()
    {
        var task = Task.current;
        bulletsToShoot = Random.Range(1, stats.MaxBullets);
        task.debugInfo = $"{bulletsToShoot}";
        task.Succeed();
    }

    [Task]
    public void AimAtTarget()
    {
        gun.AimAt(ai.Target.position, ai.Pos.position);
        Task.current.Succeed();
    }

    [Task]
    public void Shoot()
    {
        gun.Shoot();
        Task.current.Succeed();
    }

    [Task]
    public void DecrementBullets()
    {
        bulletsToShoot--;
        Task.current.Succeed();
    }

    [Task]
    public void StartParticles()
    {
        particles.Clear();
        particles.Play();
        particles.GetComponent<ParticleSystemRenderer>().material.mainTexture = movement.FacingBack ? dashBackward : dashForward;
        particles.GetComponent<ParticleSystemRenderer>().flip = movement.FacingRight ? Vector3.zero : Vector3.right;
        Task.current.Succeed();
    }

    [Task]
    public void PickDashDirection()
    {
        var dir = ai.Target.position - ai.Pos.position;

        bool colliding = false;
        int maxAttempts = 20;
        int attemptNum = 0;
        var circleRadius = 0.5f; // for the circle cast
        var dashPadding = 0.5f; // to ensure dasher doesn't end up in a wall
        var dashDir = Vector3.zero;
        do
        {
            var rotation = 90 + Random.Range(-20f, 20f);
            rotation = Random.Range(0f, 1f) > 0.5f ? -rotation : rotation;
            dashDir = Quaternion.Euler(0, 0, rotation) * dir;
            dashDir.Normalize();
            var result = Physics2D.CircleCast(ai.Pos.position, circleRadius, dashDir, stats.DashDistance + dashPadding, LayerMask.GetMask("Wall"));
            colliding = result.rigidbody != null;
            attemptNum++;
        } while (colliding && attemptNum < maxAttempts);

        this.dashDir = dashDir;

        // All dash variations collided with wall
        if (colliding)
        {
            Task.current.Fail();
        }
        else
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void Dash()
    {
        float dashTime = stats.DashTime;
        float dashSpeed = stats.DashDistance / dashTime;

        var task = Task.current;

        if (task.isStarting)
        {
            task.item = new WaitTime { Elapsed = 0, Duration = dashTime };
        }
        var info = task.item as WaitTime;

        info.Elapsed += Time.deltaTime;
        physics.AddForce(dashDir * dashSpeed);

        if(info.Elapsed > info.Duration)
        {
            task.Succeed();
        }
    }

    [Task]
    public void EndDash()
    {
        animator.SetTrigger("dash_over");
        particles.Stop();
        Task.current.Succeed();
    }

    [Task]
    public void AimGunAlongPath()
    {
        var target = 5 * movement.MoveDir + (Vector2)ai.Pos.position; 
        gun.AimAt(target, ai.Pos.position);
        //gun.SetDrawOrder(!movement.FacingBack);
    }

    [Task]
    public void FindPatrolWaypoints()
    {
        currPatrolWaypoint = 0;
        var currPatrolPos = ai.Pos.position;
        var grid = ai.Level.Grid;
        patrolWaypoints.Clear();

        for (int i = 0; i < stats.PatrolWaypoints; i++)
        {
            var nodes = grid.GetNodesInMinMaxRange(grid.NodeFromWorldPoint(currPatrolPos), 3, stats.PatrolRadius);
            var randomNode = nodes[Random.Range(0, nodes.Count)];
            patrolWaypoints.Add(randomNode.WorldPosition);
            currPatrolPos = randomNode.WorldPosition;
        }

        Task.current.Succeed();
    }

    private bool waitingOnPath;
    private bool pathRequested;

    [Task]
    public void ResetPathVariables()
    {
        waitingOnPath = false;
        pathRequested = false;
        Task.current.Succeed();
    }

    [Task]
    public void ObtainPathToWaypoint()
    {
        if (waitingOnPath) return;

        var waypoint = patrolWaypoints[currPatrolWaypoint];
        bool pathFail = false;

        if (!pathRequested)
        {
            ai.Level.PathRequestManager.RequestPath(ai.Pos.position, waypoint, (path, success, parameters) =>
            {
                if (!success)
                {
                    pathFail = true;
                    return;
                }
                ai.Path = new Path(path, ai.Pos.position, parameters.TurningRadius);
                waitingOnPath = false;
            });
            waitingOnPath = true;
            pathRequested = true;
        }

        if (pathFail)
        {
            Task.current.Fail();
        }
        else
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void IncrementWaypoint()
    {
        currPatrolWaypoint++;
    }

    private void OnDrawGizmos()
    {
        onDrawGizmoAction?.Invoke();
    }
}
