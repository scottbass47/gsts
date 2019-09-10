using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Laser Dome")]
public class LaserDomeStats : EnemyStats
{
    [Header("General")]
    [SerializeField] private float health;
    [SerializeField] private float speed;

    [Header("Hop")]
    [SerializeField] private float hopGravity;

    public override float Health => health;
    public float Speed => speed;

    public float HopGravity => hopGravity;
}
