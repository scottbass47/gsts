using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Shield Drone")]
public class ShieldDroneStats : ScriptableObject
{
    [SerializeField] private float speed;
    [SerializeField] private float health;

    [Header("Attack")]
    [SerializeField] private int numBullets;
    [SerializeField] private float spreadAngle;
    [SerializeField] private float minBulletSpeed;
    [SerializeField] private float maxBulletSpeed;

    public float Speed => speed;
    public float Health => health;
    public int NumBullets => numBullets;
    public float SpreadAngle => spreadAngle;
    public float MinBulletSpeed => minBulletSpeed;
    public float MaxBulletSpeed => maxBulletSpeed;
}
