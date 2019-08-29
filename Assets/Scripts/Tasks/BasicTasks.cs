using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTasks : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    protected AIController ai;
    protected IMovement movement;
    protected PathParameters pathParameters;
    protected EnemyStats enemyStats;

    // Variables used if not specified in tasks
    protected float Speed { get; set; }
    protected float TurningVelocity { get; set; }

    public virtual void Awake()
    {
        ai = GetComponent<AIController>();
        movement = GetComponent<IMovement>();
        movement.SetAnimator(animator);
    }

    public virtual void Start() { }

    [Task]
    public void PlayAnimation(string animation)
    {
        if(animator == null)
        {
            Debug.Log("No animator assigned in BasicTasks");
            Task.current.Fail();
            return;
        }
        animator.SetTrigger(animation);
        Task.current.Succeed();
    }

    [Task]
    public void GetPathToTarget()
    {
        var task = Task.current;

        if(task.isStarting)
        {
            ai.PendingPath = true;
            ai.PathFailed = false;
            ai.Level.PathRequestManager.RequestPath(ai.Pos.position, ai.Target.position, (path, success, parameters) =>
            {
                ai.PendingPath = false;
                if (!success)
                {
                    ai.PathFailed = true;
                    return;
                }
                ai.Path = new Path(path, ai.Pos.position, parameters.TurningRadius);
            }, pathParameters);
        }
        if (ai.PathFailed)
        {
            task.Fail();
        }
        else if (!ai.PendingPath)
        {
            task.Succeed();
        }
    }

    [Task]
    public void TargetInLOS()
    {
        var task = Task.current;
        if (ai.Target == null)
        {
            task.Fail();
            return; 
        }
        Vector2 diff = ai.Target.position - ai.Pos.position;
        RaycastHit2D ray = Physics2D.Raycast(ai.Pos.position, diff, 100, LayerMask.GetMask("Wall", "Player Feet"));

        //Debug.DrawLine(ai.Pos.position, ray.point, Color.red, 0.1f);
        var InLOS = ray.rigidbody != null && ray.rigidbody.gameObject.tag == "Player";
        if (InLOS)
        {
            task.Succeed();
        }
        else
        {
            task.Fail();
        }
    }

    [Task]
    public void TargetInRange(float range)
    {
        var task = Task.current;

        var sqrDistToTarget = (ai.Target.position - ai.Pos.position).sqrMagnitude;

        if (sqrDistToTarget < range * range)
        {
            task.Succeed();
        }
        else
        {
            task.Fail();
        }
    }

    [Task]
    public void TargetInRange(string rangeStat)
    {
        TargetInRange(enemyStats.GetStat<float>(rangeStat));
    }


    [Task]
    public void TargetOnPath()
    {
        var task = Task.current;

        if (ai.Path.TargetOnPath(ai.Target.position))
        {
            task.Succeed();
        }
        else
        {
            task.Fail();
        }
    }

    [Task]
    public void WaitStats(string stat)
    {
        var task = Task.current;
        if (task.isStarting)
        {
            task.item = new WaitTime { Elapsed = 0, Duration = enemyStats.GetStat<float>(stat) };
        }
        
        var waitTime = task.item as WaitTime;
        waitTime.Elapsed += Time.deltaTime;

        if(waitTime.Elapsed >= waitTime.Duration)
        {
            task.Succeed();
        }
    }

    [Task]
    public void MoveOnPath()
    {
        MoveOnPath(Speed, TurningVelocity);
    }

    [Task]
    public void MoveOnPath(float speed, float turningVelocity)
    {
        var path = ai.Path;

        bool done = ai.Path.Done;
        while(!done && path.AtNextWaypoint(ai.Pos.position))
        {
            path.AdvanceWaypoint();
            done = path.Done;
        }
        if (done)
        {
            Task.current.Succeed();
            return;
        }

        Vector2 dir = path.CurrentWaypoint - ai.Pos.position;
        dir.Normalize();

        movement.SetMoveDir(path.FirstWaypoint ? dir : Vector2.Lerp(movement.MoveDir, dir, Time.deltaTime * turningVelocity));
        movement.MoveSpeed = speed;
    }

    [Task]
    public void Idle()
    {
        movement.Idle();
        Task.current.Succeed();
    }

    [Task]
    public void StopMoving()
    {
        movement.MoveSpeed = 0;
        Task.current.Succeed();
    }

    [Task]
    public void FaceTarget()
    {
        movement.SetMoveDir(ai.Target.position - ai.Pos.position);
        Task.current.Succeed();
   }
}
