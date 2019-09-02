using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Dasher")]
public class DasherStats : EnemyStats
{
    [Header("Basic Attributes")]
    [SerializeField] private float speed;
    [SerializeField] private float health;
    [SerializeField] private float turningVelocity;

    [Header("Attack")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackAnticipation;
    [SerializeField] private int maxBullets;

    [Header("Dash")]
    [SerializeField] private float dashTime;
    [SerializeField] private float dashDistance;

    [Header("Idle/Patrol")]
    [SerializeField] private int patrolWaypoints;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float patrolRadius;
    [SerializeField] private float minIdleTime;
    [SerializeField] private float maxIdleTime;

    public float AttackCooldown => attackCooldown;
    public float AttackRange => attackRange;
    public float AttackAnticipation => attackAnticipation;
    public int MaxBullets => maxBullets;

    public float TurningVelocity => turningVelocity;
    public float Speed => speed;
    public override float Health => health;

    public float DashTime => dashTime;
    public float DashDistance => dashDistance;

    public int PatrolWaypoints => patrolWaypoints;
    public float PatrolSpeed => patrolSpeed;
    public float PatrolRadius => patrolRadius;
    public float MinIdleTime => minIdleTime;
    public float MaxIdleTime => maxIdleTime;
}
