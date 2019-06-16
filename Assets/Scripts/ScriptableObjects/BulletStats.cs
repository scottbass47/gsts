using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Base Pistol")]
public class BulletStats : ScriptableObject
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float knockbackAmount;
    [SerializeField] private float fireRate;

    public float Speed => speed;
    public float Damage => damage;
    public float KnockbackAmount => knockbackAmount;
    public float FireRate => fireRate;
}
