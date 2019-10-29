using Guns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GunStats")]
public class BaseGunStats : ScriptableObject, GunStats
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float damage;
    [SerializeField] private float knockbackAmount;
    [SerializeField] private float fireRate;
    [SerializeField] private float reloadRate;
    [SerializeField] private float spread;
    [SerializeField] private int clipSize;
    [SerializeField] private bool semiAuto;

    public float BulletSpeed => bulletSpeed;
    public float Damage => damage;
    public float KnockbackAmount => knockbackAmount;
    public float FireRate => fireRate;
    public float ReloadRate => reloadRate;
    public float Spread => spread;
    public int ClipSize => clipSize;
    public bool IsSemiAuto => semiAuto;

}
