using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Small Gremlin")]
public class SmallGremlinSettings : ScriptableObject
{
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDamage;
    [SerializeField] private float turningVelocity;
    [SerializeField] private float speed;

    public float AttackDelay => attackDelay;
    public float AttackCooldown => attackCooldown;
    public float AttackRange => attackRange;
    public float AttackDamage => attackDamage;
    public float TurningVelocity => turningVelocity;
    public float Speed => speed;
    
}
