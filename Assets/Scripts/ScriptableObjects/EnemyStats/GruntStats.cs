using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Grunt")]
public class GruntStats : ScriptableObject
{
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackFrame;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private float turningVelocity;
    [SerializeField] private float speed;
    [SerializeField] private float health;

    public float AttackDelay => attackDelay;
    public float AttackFrame => attackFrame;
    public float AttackCooldown => attackCooldown;
    public float AttackRange => attackRange;
    public float TurningVelocity => turningVelocity;
    public float Speed => speed;
    public float Health => health;
    
}
