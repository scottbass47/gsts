﻿using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntTasks : BasicTasks
{
    [SerializeField] private GruntStats stats;
    [SerializeField] private Transform feet;

    [Task]
    public bool IsAttacking { get; set; }

    public override void Awake()
    {
        base.Awake();
        Speed = stats.Speed;
        TurningVelocity = stats.TurningVelocity;
    }

    public override void Start()
    {
        base.Start();
        ai.Pos = feet;

        var playerBody = GameManager.Instance.Player.GetComponent<Body>();
        ai.Target = playerBody.CenterFeet;

        var health = GetComponent<Health>();
        health.Amount = stats.Health;
    }

    [Task]
    public void Bite()
    {
        var dir = ai.Target.position - ai.Pos.position;
        var hit = Physics2D.Raycast(
            ai.Pos.position,
            dir,
            stats.AttackRange,
            LayerMask.GetMask("Player Feet")
        );

        if (hit.collider != null)
        {
            var player = hit.collider.gameObject;
            DamageManager.DealDamage(player);
        }
        Task.current.Succeed();
    }

    [Task]
    public void SetAttacking (bool attacking)
    {
        IsAttacking = attacking;
        Task.current.Succeed();
    }
}