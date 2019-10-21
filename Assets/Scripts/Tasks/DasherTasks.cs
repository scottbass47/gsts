using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;
using System;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using Panda;

public class DasherTasks : MonoBehaviour
{
    [SerializeField] private Transform feet;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Texture2D dashForward;
    [SerializeField] private Texture2D dashBackward;

    private AI ai;
    private Movement movement;
    private BasicTasks basicTasks;
    private AnimationTasks animationTasks;
    private Physics physics;
    private GunController gun;
    private Transform targetBody;
    private ParticleSystem particles;
    private PathFindingTasks pathFinding;

    private float attackCooldown;

    private Coroutine shuffle;

    private Action onDrawGizmoAction;
    private List<Vector3> patrolWaypoints;
    private int currPatrolWaypoint;
    private int bulletsToShoot;
    private Vector2 dashDir;
    private DasherStats dasherStats => (DasherStats)ai.EnemyStats;

    [Task]
    public bool MoreWaypoints => currPatrolWaypoint < patrolWaypoints.Count;

    [Task]
    public bool IsActing { get; set; }

    [Task]
    public bool StillShooting => bulletsToShoot > 0;

    private void Awake()
    {
        patrolWaypoints = new List<Vector3>();
    }

    public void Start()
    {
        ai = GetComponent<AI>();
        basicTasks = GetComponent<BasicTasks>();
        animationTasks = GetComponent<AnimationTasks>();
        physics = GetComponent<Physics>();
        movement = GetComponent<BasicMovement>();
        gun = GetComponentInChildren<GunController>();
        particles = GetComponent<ParticleSystem>();
        pathFinding = GetComponent<PathFindingTasks>();
        pathFinding.SetMovementParameters(dasherStats.Speed, dasherStats.TurningVelocity);

        var playerBody = GameManager.Instance.Player.GetComponent<Body>();
        targetBody = playerBody.CenterBody;
    }

    [Task]
    public void TargetInAttackRange()
    {
        basicTasks.TargetInRange(dasherStats.AttackRange);
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
        bulletsToShoot = Random.Range(1, dasherStats.MaxBullets);
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
            var result = Physics2D.CircleCast(ai.Pos.position, circleRadius, dashDir, dasherStats.DashDistance + dashPadding, LayerMask.GetMask("Wall"));
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
        float dashTime = dasherStats.DashTime;
        float dashSpeed = dasherStats.DashDistance / dashTime;

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
        animationTasks.Animator.SetTrigger("dash_over");
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
    public void SetChasePathParameters()
    {
        pathFinding.SetMovementParameters(dasherStats.Speed, dasherStats.TurningVelocity);
        Task.current.Succeed();
    }

    [Task]
    public void SetPatrolPathParameters()
    {
        pathFinding.SetMovementParameters(dasherStats.PatrolSpeed, dasherStats.TurningVelocity);
        Task.current.Succeed();
    }

    [Task]
    public void FindPatrolWaypoints()
    {
        currPatrolWaypoint = 0;
        var currPatrolPos = ai.Pos.position;
        var grid = ai.Level.Grid;
        patrolWaypoints.Clear();

        for (int i = 0; i < dasherStats.PatrolWaypoints; i++)
        {
            var nodes = grid.GetNodesInMinMaxRange(grid.NodeFromWorldPoint(currPatrolPos), 3, dasherStats.PatrolRadius);
            var randomNode = nodes[Random.Range(0, nodes.Count)];
            patrolWaypoints.Add(randomNode.WorldPosition);
            currPatrolPos = randomNode.WorldPosition;
        }

        Task.current.Succeed();
    }

    [Task]
    public void ObtainPathToWaypoint()
    {
        pathFinding.GetPathToPos(patrolWaypoints[currPatrolWaypoint]);
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
