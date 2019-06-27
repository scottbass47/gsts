using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Base Pistol")]
public class GunStats : ScriptableObject
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float knockbackAmount;
    [SerializeField] private float fireRate;
    [SerializeField] private float reloadRate;
    [SerializeField] private int magSize;
    [SerializeField] private bool semiAuto;
    [SerializeField] private bool enemyGun;

    public float Speed => speed;
    public float Damage => damage;
    public float KnockbackAmount => knockbackAmount;
    public float FireRate => fireRate;
    public float ReloadRate => reloadRate;
    public int MagSize => magSize;
    public bool IsSemiAuto => semiAuto;
    public bool IsEnemyGun => enemyGun;
}
