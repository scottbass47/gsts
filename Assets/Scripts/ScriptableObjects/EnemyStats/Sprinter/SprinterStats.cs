using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Sprinter")]
public class SprinterStats : EnemyStats
{
    [Header("Basic Attributes")]
    [SerializeField] private float runSpeed;
    [SerializeField] private int health;
    [SerializeField] private float turningVelocity;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float sprintDuration;
    [SerializeField] private float sprintCooldown;
    [SerializeField] private float sprintRange;

    [Header("Attack")]
    [SerializeField] private float attackRange;
    [SerializeField] private float attackRadius;

    public float RunSpeed => runSpeed;
    public override float Health => health;
    public float TurningVelocity => turningVelocity;

    public float SprintSpeed => sprintSpeed;
    public float SprintDuration => sprintDuration;
    public float SprintCooldown => sprintCooldown;
    public float SprintRange => sprintRange;

    public float AttackRange => attackRange;
    public float AttackRadius => attackRadius;

}
