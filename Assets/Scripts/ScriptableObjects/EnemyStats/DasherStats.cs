using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Dasher")]
public class DasherStats : ScriptableObject
{
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private float turningVelocity;
    [SerializeField] private float speed;
    [SerializeField] private float health;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashDistance;

    public float AttackDelay => attackDelay;
    public float AttackCooldown => attackCooldown;
    public float AttackRange => attackRange;
    public float TurningVelocity => turningVelocity;
    public float Speed => speed;
    public float Health => health;
    public float DashTime => dashTime;
    public float DashDistance => dashDistance;
    
}
