using Guns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerOld : MonoBehaviour
{
    public bool Flipped => gunModel.IsFlipped;
    private float aimAngle => gunModel.AimAngle;
    private SpriteRenderer spriteRenderer;

    private float elapsedTime = 0f;
    private bool reloading;
    public bool Reloading
    {
        get => reloading;
        private set
        {
            reloading = value;
            if (reloading)
            {
                OnReload?.Invoke(stats.ReloadRate);
            }
        }
    }
    private int bulletsInClip;
    public int BulletsInClip
    {
        get => bulletsInClip;
        private set
        {
            if(bulletsInClip != value)
            {
                bulletsInClip = value;
                OnClipChange?.Invoke(bulletsInClip);
            }
        }
    }

    public GameObject bulletPrefab;
    [SerializeField] private BaseGunStats stats;
    public BaseGunStats Stats => stats;

    [SerializeField] private Transform center;
    [SerializeField] private Transform hand;
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform barrel;

    [SerializeField] private float accurateShootingRange;
    [SerializeField] private bool rotateBullet;
    [SerializeField] private bool drawDebug = false;
    [SerializeField] private GameObject muzzleFlashPrefab;
    
    public event Action<float> OnReload;
    public event Action<int> OnClipChange;
    public event Action OnFire;

    private GunModel gunModel;

    // Start is called before the first frame update
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gunModel = new GunModel(
            new BasicClip(10),
            new AccurateAim(accurateShootingRange),
            new GunPositioning(center, hand, transform, pivot, barrel),
            1.0f,
            stats
        );
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    public bool Shoot()
    {
        if ((elapsedTime * stats.FireRate < 1 || reloading) /*&& !stats.IsEnemyGun*/) return false;
        elapsedTime = 0;

        float randomOffset = 0; // Random.Range(-stats.Accuracy.ModdedValue(), stats.Accuracy.ModdedValue());
        float radians = Mathf.Deg2Rad * (aimAngle + randomOffset);
        radians = Flipped ? Mathf.PI - radians : radians;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;

        var bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.Damage = stats.Damage; 
        bulletComp.Speed = stats.BulletSpeed; 
        bulletComp.KnockbackAmount = stats.KnockbackAmount; 
        bullet.transform.position = gunModel.GunPositioning.BarrelWorldPos; 
        bulletComp.RotateTransform = rotateBullet;
        bulletComp.ShootRadians(radians);

        if(muzzleFlashPrefab != null)
        {
            var obj = Instantiate(muzzleFlashPrefab);
            var muzzle = obj.GetComponent<MuzzleFlash>();
            muzzle.InitializeFlash(gunModel.GunPositioning.BarrelWorldPos, aimAngle, Flipped, spriteRenderer.sortingOrder);
        }

        OnFire?.Invoke();

        BulletsInClip--;

        if (BulletsInClip == 0 /*&& !stats.IsEnemyGun*/)
        {
            StartCoroutine(Reload(stats.ReloadRate));
        }
        return true;
    }

    private IEnumerator Reload(float duration)
    {
        Reloading = true;
        yield return new WaitForSeconds(duration);
        BulletsInClip = stats.ClipSize;
        Reloading = false;
    }

    public void AimAt(Vector2 target, Vector2 center)
    {
        gunModel.AimAt(target);
        UpdateSpriteRenderer();
    }

    private void UpdateSpriteRenderer()
    {
        spriteRenderer.flipX = Flipped;
    }

    public void SetDrawOrder(bool inFront)
    {
        spriteRenderer.sortingOrder = inFront ? 1 : -1;
    }
}
