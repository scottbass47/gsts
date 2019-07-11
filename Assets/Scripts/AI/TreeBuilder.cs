using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;
using System;

public class TreeBuilder
{
    private AIController ai;
    private BehaviourTreeBuilder builder;

    public TreeBuilder(AIController ai)
    {
        this.ai = ai;
        builder = new BehaviourTreeBuilder();
    }

    public TreeBuilder Selector(string name)
    {
        builder.Selector(name);
        return this;
    }

    public TreeBuilder Sequence(string name)
    {
        builder.Sequence(name);
        return this;
    }

    public TreeBuilder Condition(string name, Func<TimeData, bool> fn)
    {
        builder.Condition(name, fn);
        return this;
    }

    public TreeBuilder Do(string name, Func<TimeData, BehaviourTreeStatus> action)
    {
        builder.Do(name, action);
        return this;
    }

    public TreeBuilder End()
    {
        builder.End();
        return this;
    }

    public TreeBuilder LOS()
    {
        builder.Condition("LOS", t =>
        {
            if (ai.Target == null) return false;
            Vector2 diff = ai.Target.position - ai.Pos.position;
            RaycastHit2D ray = Physics2D.Raycast(ai.Pos.position, diff, 100, LayerMask.GetMask("Wall", "Player Feet"));

            //Debug.DrawLine(ai.Pos.position, ray.point, Color.red, 0.1f);
            var InLOS = ray.rigidbody != null && ray.rigidbody.gameObject.tag == "Player";
            return InLOS;
        });
        return this;
    }

    public TreeBuilder StopMoving()
    {
        builder.Do("Stop Moving", t =>
        {
            ai.Movement.AddForce(Vector2.zero);
            return BehaviourTreeStatus.Success;
        });
        return this;
    }

    public TreeBuilder Range(float range)
    {
        builder.Condition("Range", t =>
        {
            Vector2 diff = ai.Target.position - ai.Pos.position;
            return diff.SqrMagnitude() <= range * range;
        });
        return this;
    }

    public TreeBuilder HasPath()
    {
        builder.Condition("Has Path", t =>
        {
            return ai.Path != null;
        });
        return this;
    }

    public TreeBuilder GetPath()
    {
        builder.Condition("Get Path", t =>
        {
            if (ai.Path != null && ai.Path.TargetOnPath(ai.Target.position) && !ai.Path.Done) return true;

            if (!ai.PendingPath)
            {
                ai.PendingPath = true;
                ai.Level.PathRequestManager.RequestPath(ai.Pos.position, ai.Target.position, (path, success) =>
                {
                    ai.PendingPath = false;
                    if (!success) return;
                    ai.Path = new Path(path, ai.Pos.position, 1f);
                });
            }

            return false;
        });
        return this;
    }

    public TreeBuilder MoveOnPath(float speed, float turningVelocity, MovementAnimator moveAnim)
    {
        builder.Do("Move On Path", t =>
        {
            var path = ai.Path;

            bool done = false;
            while(path.AtNextWaypoint(ai.Pos.position))
            {
                path.AdvanceWaypoint();
                done = path.Done;
            }
            if (done) return BehaviourTreeStatus.Failure;

            Vector2 dir = path.CurrentWaypoint - ai.Pos.position;
            dir.Normalize();

            ai.MoveDir = path.FirstWaypoint ? dir : Vector2.Lerp(ai.MoveDir, dir, t.deltaTime * turningVelocity);
            ai.Move(speed);
            moveAnim.SetLookAngle(ai.MoveDir, true);

            return BehaviourTreeStatus.Success;
        });
        return this;
    }
    public IBehaviourTreeNode Build()
    {
        return builder.Build();
    }
}
