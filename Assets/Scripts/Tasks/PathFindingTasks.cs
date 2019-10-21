using Panda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(AI))]
[RequireComponent(typeof(Movement))]
public class PathFindingTasks : MonoBehaviour
{
    private AI ai;
    private Movement movement;

    private PathParameters pathParameters = PathParameters.Default;
    private float moveSpeed;
    private float turningVelocity;

    private Func<float> getMoveSpeed;
    private Func<float> getTurningVelocity;

    public float MoveSpeed => getMoveSpeed != null ? getMoveSpeed() : moveSpeed;
    public float TurningVelocity => getTurningVelocity != null ? getTurningVelocity() : turningVelocity;

    private Path path;
    private bool pathPending;
    private bool pathFailed;
    private bool pathInvalid;
    private Vector3 pathDestination;
    private bool pathToTarget;

    private void Awake()
    {
        ai = GetComponent<AI>();
        movement = GetComponent<Movement>();
    }

    public void SetPathParameters(PathParameters pathParameters)
    {
        this.pathParameters = pathParameters;
    }

    public void SetMovementParameters(float moveSpeed, float turningVelocity)
    {
        this.moveSpeed = moveSpeed;
        this.turningVelocity = turningVelocity;
    }

    public void SetMoveSpeedFunction(Func<float> moveSpeedFunction)
    {
        getMoveSpeed = moveSpeedFunction;
    }

    public void SetTurningVelocityFunction(Func<float> turningVelocityFunction)
    {
        getTurningVelocity = turningVelocityFunction;
    }

    public void InvalidatePath()
    {
        pathInvalid = true;
    }

    [Task]
    public void GetPathToTarget()
    {
        GetPathToPos(ai.Target.position, true);
    }

    [Task]
    public void GetPathToPos(Vector3 pos, bool pathToTarget = false)
    {
        pathDestination = pos;
        this.pathToTarget = pathToTarget;

        var task = Task.current;
        if(task.isStarting)
        {
            pathPending = true;
            pathFailed = false;
            ai.Level.PathRequestManager.RequestPath(ai.Pos.position, pos, (path, success, parameters) =>
            {
                pathPending = false;
                if (!success)
                {
                    pathFailed = true;
                    return;
                }
                this.path = new Path(path, ai.Pos.position, parameters.TurningRadius);
            }, pathParameters);
        }
        if (pathFailed)
        {
            task.Fail();
        }
        else if (!pathPending)
        {
            pathInvalid = false;
            task.Succeed();
        }
    }


    // Moves the enemy along the generated path. Assumes a valid path has been generated.
    // To make sure assumption holds, this task should be wrapped in a while PathValid loop.
    [Task]
    public void MoveOnPath()
    {
        bool done = path.Done;
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

        movement.MoveDir = path.FirstWaypoint ? dir : Vector2.Lerp(movement.MoveDir, dir, Time.deltaTime * TurningVelocity);
        movement.MoveSpeed = MoveSpeed;
    }

    [Task]
    public void PathValid()
    {
        var task = Task.current;

        if (pathInvalid || (pathToTarget && !path.TargetOnPath(ai.Target.position)))
        {
            task.Fail();
        }
        else
        {
            task.Succeed();
        }
    }
}
