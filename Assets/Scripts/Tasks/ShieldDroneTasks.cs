using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDroneTasks : BasicTasks
{
    [SerializeField] private Transform feet;
    [SerializeField] private ShieldDroneStats stats;

    public override void Start()
    {
        base.Start();
        ai.Pos = feet;

        var playerBody = GameManager.Instance.Player.GetComponent<Body>();
        ai.Target = playerBody.CenterFeet;

        var health = GetComponent<Health>();
        health.Amount = stats.Health;

        Speed = stats.Speed;
        TurningVelocity = float.MaxValue;
        pathParameters = new PathParameters(false, 0.05f);
    }
}
