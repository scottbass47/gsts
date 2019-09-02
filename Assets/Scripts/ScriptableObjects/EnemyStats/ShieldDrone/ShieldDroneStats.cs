using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Shield Drone")]
public class ShieldDroneStats : EnemyStats
{
    [SerializeField] private float speed;
    [SerializeField] private float health;

    [Header("Attack")]
    [SerializeField] private int numBullets;
    [SerializeField] private float spreadAngle;
    [SerializeField] private float minBulletSpeed;
    [SerializeField] private float maxBulletSpeed;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackRange;

    public float Speed => speed;
    public override float Health => health;
    public int NumBullets => numBullets;
    public float SpreadAngle => spreadAngle;
    public float MinBulletSpeed => minBulletSpeed;
    public float MaxBulletSpeed => maxBulletSpeed;
    public float AttackCooldown => attackCooldown;
    public float AttackRange => attackRange;
}
