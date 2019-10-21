using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTasks : MonoBehaviour
{
    private AI ai;
    private Movement movement;
    private EnemyStats stats;

    public void Awake()
    {
        ai = GetComponent<AI>();
        movement = GetComponent<Movement>();
        stats = ai.EnemyStats;
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
    public void TargetInCircleLOS(float radius)
    {
        var task = Task.current;
        if (ai.Target == null)
        {
            task.Fail();
            return; 
        }
        Vector2 diff = ai.Target.position - ai.Pos.position;

        var ray = Physics2D.CircleCast(ai.Pos.position, radius, diff, diff.magnitude, LayerMask.GetMask("Wall", "Player Feet"));

        Debug.DrawLine(ai.Pos.position, ray.point, Color.red, 0.1f);
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
        TargetInRange(stats.GetStat<float>(rangeStat));
    }

    [Task]
    public void WaitRandom(string stat)
    {
        FloatRange range = stats.GetStat<FloatRange>(stat);
        var waitTime = Random.Range(range.Min, range.Max);
        MyWait(waitTime);
    }

    [Task]
    public void WaitStats(string stat)
    {
        MyWait(stats.GetStat<float>(stat));
    }

    [Task]
    public void MyWait(float time)
    {
        var task = Task.current;
        if (task.isStarting)
        {
            task.item = new WaitTime { Elapsed = 0, Duration = time };
        }
        
        var waitTime = task.item as WaitTime;
        waitTime.Elapsed += Time.deltaTime;

        if(waitTime.Elapsed >= waitTime.Duration)
        {
            task.Succeed();
        }
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
        movement.MoveDir = ai.Target.position - ai.Pos.position;
        Task.current.Succeed();
    }
    
    [Task]
    public void MeleeAttack(string rangeStat)
    {
        MeleeAttack(stats.GetStat<float>(rangeStat));
    }

    [Task]
    public void MeleeAttack(float range)
    {
        var dir = ai.Target.position - ai.Pos.position;
        var hit = Physics2D.Raycast(
            ai.Pos.position,
            dir,
            range,
            LayerMask.GetMask("Player Feet")
        );

        if (hit.collider != null)
        {
            var player = hit.collider.gameObject;
            DamageManager.DealDamage(player);
        }
        Task.current.Succeed();
    }
}
