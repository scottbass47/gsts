using Assets.FastRotSprite.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public bool Flipped { get; set; }
    [SerializeField] private float aimAngle;
    private float angleThreshold = 15; // Used for over-aiming
    private Vector2 offset;
    private Vector2 flippedOffset;
    private SpriteRenderer spriteRenderer;
    private RotationBase rotator;

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
    [SerializeField] private GunStats stats;
    public GunStats Stats => stats;

    [SerializeField] private Transform pivot;
    [SerializeField] private Transform barrelOffset;
    public Vector3 BarrelOffset => barrelOffset.localPosition;

    [SerializeField] private float accurateShootingRange;
    [SerializeField] private bool rotateBullet;
    [SerializeField] private bool drawDebug = false;
    [SerializeField] private GameObject muzzleFlashPrefab;

    private Vector3 pivotPos => pivot.localPosition;   

    public event Action<float> OnReload;
    public event Action<int> OnClipChange;
    public event Action OnFire;

    // Returns the location in world space where the bullet originates in the shot
    public Vector2 ShotOrigin
    {
        get
        {
            Vector3 offset = Quaternion.Euler(0, 0, aimAngle) * new Vector3(BarrelOffset.x, BarrelOffset.y);
            if (Flipped) offset.x = -offset.x;
            return transform.position + offset; 
        } 
    }

    // Start is called before the first frame update
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rotator = GetComponent<RotationBase>();
        offset = transform.localPosition;
        flippedOffset = new Vector2(-offset.x, offset.y);
        BulletsInClip = stats.MagSize;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    public bool Shoot()
    {
        if ((elapsedTime * stats.FireRate < 1 || reloading) && !stats.IsEnemyGun) return false;
        elapsedTime = 0;

        float randomOffset = 0; // Random.Range(-stats.Accuracy.ModdedValue(), stats.Accuracy.ModdedValue());
        float radians = Mathf.Deg2Rad * (aimAngle + randomOffset);
        radians = Flipped ? Mathf.PI - radians : radians;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;

        var bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.Damage = stats.Damage; 
        bulletComp.Speed = stats.Speed; 
        bulletComp.KnockbackAmount = stats.KnockbackAmount; 
        bullet.transform.position = ShotOrigin; 
        bullet.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder - 1;
        bulletComp.RotateTransform = rotateBullet;
        bulletComp.Shoot(radians);

        if(muzzleFlashPrefab != null)
        {
            var obj = Instantiate(muzzleFlashPrefab);
            var muzzle = obj.GetComponent<MuzzleFlash>();
            muzzle.InitializeFlash(ShotOrigin, aimAngle, Flipped, spriteRenderer.sortingOrder);
        }

        OnFire?.Invoke();

        BulletsInClip--;

        if (BulletsInClip == 0 && !stats.IsEnemyGun)
        {
            StartCoroutine(Reload(stats.ReloadRate));
        }
        return true;
    }

    private IEnumerator Reload(float duration)
    {
        Reloading = true;
        yield return new WaitForSeconds(duration);
        BulletsInClip = stats.MagSize;
        Reloading = false;
    }

    public void AimAt(Vector2 target, Vector2 center)
    {
        var aimVec = target - center;
        Flipped = aimVec.x < 0;
        transform.localPosition = Flipped ? flippedOffset : offset;

        var pivot = (Vector2)transform.position;
        Vector2 pivotToTarget = target - pivot;
        Vector2 pivotToTargetPrime = Vector2.zero;
        float targetDist = pivotToTarget.magnitude;
        float degrees = 0;
        bool inAccurateShootingRange = false;

        if(targetDist < accurateShootingRange)
        {
            degrees = Mathf.Atan2(aimVec.y, Flipped ? -aimVec.x : aimVec.x) * Mathf.Rad2Deg;
        }
        else
        {
            var pivotToBarrel = BarrelOffset - pivotPos;
            inAccurateShootingRange = true;
            if (Flipped) pivotToTarget.x = -pivotToTarget.x;

            float barrelDist = pivotToBarrel.magnitude;

            float theta = Mathf.Acos(pivotToBarrel.x / barrelDist);
            float beta = Mathf.PI - theta;
            float gamma = Mathf.Asin(barrelDist / targetDist * Mathf.Sin(beta));

            pivotToTargetPrime = targetDist * new Vector2(Mathf.Cos(gamma), Mathf.Sin(gamma));

            degrees = Vector2.SignedAngle(pivotToTargetPrime, pivotToTarget);

        }
        aimAngle = degrees;

        // Adjust gun position around pivot
        var rotation = Quaternion.AngleAxis(aimAngle, new Vector3(0, 0, 1));
        var rotatedPivot = rotation * pivotPos;
        rotatedPivot.x = Flipped ? -rotatedPivot.x : rotatedPivot.x;
        transform.localPosition -= rotatedPivot;

        spriteRenderer.flipX = Flipped;
        rotator.RotationDegrees = aimAngle;

        if(inAccurateShootingRange && drawDebug)
        {
            Debug.DrawLine(transform.position + rotatedPivot, (Vector2)transform.position + pivotToTargetPrime, Color.red);
            Debug.DrawLine(pivot, GetBarrelPos(), Color.red);
            Debug.DrawLine(GetBarrelPos(), pivot + pivotToTargetPrime, Color.red);

            Debug.DrawLine(pivot, target, Color.blue);
            Debug.DrawLine(pivot, ShotOrigin, Color.blue);
            Debug.DrawLine(ShotOrigin, target, Color.blue);
        }
    }

    public void SetDrawOrder(bool inFront)
    {
        spriteRenderer.sortingOrder = inFront ? 1 : -1;
    }

    public Vector2 GetBarrelPos()
    {
        var off = Flipped ? new Vector2(-BarrelOffset.x, BarrelOffset.y) : (Vector2)BarrelOffset;
        return (Vector2)transform.position + off;
    }
}
