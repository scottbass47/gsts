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

            Debug.DrawLine(ai.Pos.position, ray.point, Color.red, 0.1f);
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
            Transform pTransform = ai.Target;
            Transform myTransform = ai.Pos;
            Vector2 diff = pTransform.position - myTransform.position;
            var lookAngle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);

            // Both lines can be deleted when debug drawing is no longer needed
            RaycastHit2D ray = Physics2D.Raycast(myTransform.position, diff, 100, LayerMask.GetMask("Wall", "Player Feet"));
            Debug.DrawLine(myTransform.position, ray.point, Color.red, 0.1f);

            Coord playerPos = ai.Level.WorldToTile(pTransform.position);
            Coord enemyPos = ai.Level.WorldToTile(myTransform.position);
            if (ai.Path == null || !ai.Path.OnPath(playerPos) || !ai.Path.OnPath(enemyPos))
            {
                ai.Path = ai.PathFinder.GetPath(
                    enemyPos.tx,
                    enemyPos.ty,
                    playerPos.tx,
                    playerPos.ty
                );
            }

            return ai.Path != null && ai.Path.path.Count > 1;

        });
        return this;
    }

    public TreeBuilder MoveOnPath(float speed, float turningVelocity, MovementAnimator moveAnim)
    {
        builder.Do("Move On Path", t =>
        {
            Coord tile = ai.Level.WorldToTile(ai.Pos.position);
            Vector2 enemyPos = ai.Level.WorldToLevel(ai.Pos.position);

            // tileCenter in Level Coords
            Vector2 tileCenter = new Vector2(tile.tx + 0.5f, tile.ty + 0.5f);

            if (ai.Path.AtGoal(tileCenter) || !ai.Path.OnPath(tileCenter)) return BehaviourTreeStatus.Failure;

            Coord goal = ai.Path.GetNext(tile);

            // Try to get close to the center of the tile
            if ((ai.Level.LevelToWorld(tileCenter) - (Vector2)ai.Pos.position).SqrMagnitude() > 1f)
            {
                goal = tile;
            }

            // Calculate angle between enemy and goal
            var goalVec = new Vector2(goal.tx - enemyPos.x + 0.5f, goal.ty - enemyPos.y + 0.5f).normalized;

            Vector2 dir = Vector2.zero;
            if(Vector2.Angle(goalVec, ai.MoveDir) > 60)
            {
                dir = goalVec; 
            }
            else
            {
                dir = Vector2.Lerp(ai.MoveDir, goalVec, Time.deltaTime * turningVelocity);
            }

            ai.Movement.AddForce(dir * speed);
            ai.MoveDir = dir;

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            moveAnim.LookAngle = angle;

            return BehaviourTreeStatus.Success;

        });
        return this;
    }
    public IBehaviourTreeNode Build()
    {
        return builder.Build();
    }
}
