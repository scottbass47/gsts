using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public bool Flipped { get; set; }
    private float aimAngle;
    private float angleThreshold = 15; // Used for over-aiming
    private bool aimingRight;
    private Vector3 offset;
    private SpriteRenderer spriteRenderer;

    private float elapsedTime = 0f;
    public GameObject bulletPrefab;
    [SerializeField] private BulletStats stats;

    public Vector2 BarrelOffset => new Vector2(0.9f, 0.375f);

    // Returns the location in world space where the bullet originates in the shot
    // Use this position to calculate the aim angle.
    public Vector2 ShotOrigin
    {
        get
        {
            Vector3 offset = Quaternion.Euler(0, 0, Flipped ? aimAngle + 180 : aimAngle) * 
                             new Vector3(Flipped ? -BarrelOffset.x : BarrelOffset.x, BarrelOffset.y);

            return transform.position + offset; 
        } 

    }


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        offset = transform.localPosition;
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

    // Angle in [0, 360)
    public void SetAimAngle(float angle)
    {
        bool right = RightSide(angle);
        aimAngle = angle;

        bool shouldFlip = false;
        if (aimingRight && !right)
        {
            shouldFlip = angle > 90 + angleThreshold && angle < 270 - angleThreshold;
        }
        else if (!aimingRight && right)
        {
            shouldFlip = angle < 90 - angleThreshold || angle > 270 + angleThreshold;
        }

        bool flipped = shouldFlip == aimingRight;

        Vector3 translate = Vector3.zero;
        transform.eulerAngles = new Vector3(0, 0, flipped ? angle + 180 : angle);
        //pixelRotate.SetRotate(flipped ? 180 - angle : angle, HandlePivot + pixelRotate.PivotCenter, out translate);

        spriteRenderer.flipX = flipped;
        //transform.localPosition = localPos + translate;

        if (flipped)
        {
            transform.localPosition = new Vector3(-offset.x, offset.y, 0);
        }
        else
        {
            transform.localPosition = offset;
        }
        if (shouldFlip) aimingRight = !aimingRight;

        Flipped = flipped;
    }

    public void AimAt(Vector2 target)
    {

    }

    public void SetDrawOrder(bool inFront)
    {
        spriteRenderer.sortingOrder = inFront ? 1 : -1;
    }

    public Vector2 GetAimPoint()
    {
        return (Vector2)transform.position + BarrelOffset;
    }

    private bool RightSide(float angle)
    {
        return angle >= 270 || angle <= 90;
    }
}
