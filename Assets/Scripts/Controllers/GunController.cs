using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public bool Flipped { get; set; }
    private float aimAngle;
    private float angleThreshold = 15; // Used for over-aiming
    private bool aimingRight;
    private Vector2 offset;
    private Vector2 flippedOffset;
    private SpriteRenderer spriteRenderer;

    private float elapsedTime = 0f;
    public GameObject bulletPrefab;
    [SerializeField] private BulletStats stats;

    public Vector2 BarrelOffset => new Vector2(0.9f, 0.375f);
    
    [SerializeField] private float accurateShootingRange;

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
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        offset = transform.localPosition;
        flippedOffset = new Vector2(-offset.x, offset.y);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    public bool Shoot()
    {
        if (elapsedTime * stats.FireRate < 1) return false;
        elapsedTime = 0;

        float randomOffset = 0; // Random.Range(-stats.Accuracy.ModdedValue(), stats.Accuracy.ModdedValue());
        float radians = Mathf.Deg2Rad * (aimAngle + randomOffset);
        radians = Flipped ? Mathf.PI - radians : radians;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;

        //bool critical = Random.Range(0f, 1f) < stats.CriticalChance.Mod;

        var bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.Damage = stats.Damage; //critical ? stats.CriticalDamage.ModdedValue() : stats.Damage.ModdedValue();
        bulletComp.Speed = stats.Speed; // stats.BulletSpeed.ModdedValue();
        bulletComp.KnockbackAmount = stats.KnockbackAmount; 
        bullet.transform.position = ShotOrigin; //+ new Vector3(0, yOff, 0);
        bullet.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder - 1;
        bulletComp.Shoot(radians);

        //bulletsLeft--;
        //if (bulletsLeft <= 0)
        //{
        //    Reloading = true;
        //    reloadTime = stats.ReloadSpeed.ModdedValue();
        //}
        return true;
    }

    public void AimAt(Vector2 target, Vector2 center)
    {
        var aimVec = target - center;
        Flipped = aimVec.x < 0;

        Vector2 pivotToTarget = target - (Vector2)transform.position;
        float targetDist = pivotToTarget.magnitude;
        float degrees = 0;

        if(targetDist < accurateShootingRange)
        {
            degrees = Mathf.Atan2(aimVec.y, Flipped ? -aimVec.x : aimVec.x) * Mathf.Rad2Deg;
        }
        else
        {
            if (Flipped) pivotToTarget.x = -pivotToTarget.x;

            float barrelDist = BarrelOffset.magnitude;

            float theta = Mathf.Acos(BarrelOffset.x / barrelDist);
            float beta = Mathf.PI - theta;
            float gamma = Mathf.Asin(barrelDist / targetDist * Mathf.Sin(beta));

            Vector2 pivotToTargetPrime = targetDist * new Vector2(Mathf.Cos(gamma), Mathf.Sin(gamma));

            degrees = Vector2.SignedAngle(pivotToTargetPrime, pivotToTarget);

            //Debug.DrawLine(transform.position, (Vector2)transform.position + pivotToTargetPrime, Color.red);
            //Debug.DrawLine(transform.position, GetBarrelPos(), Color.red);
            //Debug.DrawLine(GetBarrelPos(), (Vector2)transform.position + pivotToTargetPrime, Color.red);

            //Debug.DrawLine(transform.position, target, Color.blue);
            //Debug.DrawLine(transform.position, ShotOrigin, Color.blue);
            //Debug.DrawLine(ShotOrigin, target, Color.blue);
        }
        aimAngle = degrees;

        transform.localPosition = Flipped ? flippedOffset : offset;
        spriteRenderer.flipX = Flipped;
        transform.eulerAngles = new Vector3(0, 0, Flipped ? 360 - aimAngle : aimAngle);
    }

    public void SetDrawOrder(bool inFront)
    {
        spriteRenderer.sortingOrder = inFront ? 1 : -1;
    }

    public Vector2 GetBarrelPos()
    {
        var off = Flipped ? new Vector2(-BarrelOffset.x, BarrelOffset.y) : BarrelOffset;
        return (Vector2)transform.position + off;
    }

    private bool RightSide(float angle)
    {
        return angle >= 270 || angle <= 90;
    }
}
