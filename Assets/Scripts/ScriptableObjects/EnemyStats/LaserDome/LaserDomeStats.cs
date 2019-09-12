using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Laser Dome")]
public class LaserDomeStats : EnemyStats
{
    [Header("General")]
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float turningVelocity;

    [Header("Hop")]
    [SerializeField] private float hopScale;

    [Header("Attack")]
    [SerializeField] private float attackRange;

    public override float Health => health;
    public float Speed => speed;
    public float TurningVelocity => turningVelocity;

    public float HopScale => hopScale;

    public float AttackRange => attackRange;
}
