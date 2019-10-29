using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Guns
{
    public abstract class Gun : MonoBehaviour
    {
        public bool BulletShot { get; private set; }

        public abstract GunModel GunModel { get; }
        public abstract GunStats GunStats { get; }

        [Header("Bullet")]
        [SerializeField] protected GameObject bulletPrefab;

        [Header("Transforms")]
        [SerializeField] protected Transform center;
        [SerializeField] protected Transform hand;
        [SerializeField] protected Transform gun;
        [SerializeField] protected Transform pivot;
        [SerializeField] protected Transform barrel;

        [Header("Muzzle Flash")]
        [SerializeField] protected GameObject muzzleFlashPrefab;

        protected bool rotateBullet;
        protected SpriteRenderer gunRenderer;

        public bool GunInFront => Mathf.Sin(Mathf.Deg2Rad * GunModel.AimAngle) < 0;

        private float FinalShotAngle
        {
            get
            {
                float angle = GunModel.AimAngleBullet;
                float offset = Random.Range(-GunStats.Spread, GunStats.Spread);
                return angle + offset;
            }
        }

        public virtual void Awake()
        {
            gunRenderer = GetComponent<SpriteRenderer>();
        }

        public void Shoot()
        {
            BulletShot = GunModel.TryShoot();

            if(BulletShot)
            {
                ShootBullet();
            }
        }

        public virtual void ShootBullet()
        {
            var bulletObj = CreateBullet();
            ApplyStats(bulletObj);
            ModifyBullet(bulletObj);
            SendBullet(bulletObj);
            if(muzzleFlashPrefab != null)
            {
                CreateMuzzleFlash();
            }
        }

        public virtual void CreateMuzzleFlash()
        {
            var obj = Instantiate(muzzleFlashPrefab);
            var muzzle = obj.GetComponent<MuzzleFlash>();
            muzzle.InitializeFlash(
                GunModel.GunPositioning.BarrelWorldPos, 
                GunModel.AimAngle, 
                GunModel.IsFlipped, 
                gunRenderer.sortingOrder
            );
        }

        public virtual GameObject CreateBullet()
        {
            var bulletObj = Instantiate(bulletPrefab);
            bulletObj.transform.position = GunModel.GunPositioning.BarrelWorldPos;
            return bulletObj;
        }

        public virtual void ApplyStats(GameObject bulletObj)
        {
            new BulletBuilder(bulletObj)
                .Speed(GunStats.BulletSpeed)
                .Damage(GunStats.Damage)
                .Knockback(GunStats.KnockbackAmount)
                .RotateTransform(rotateBullet);
        }

        public virtual void SendBullet(GameObject bulletObj)
        {
            var bullet = bulletObj.GetComponent<Bullet>();
            bullet.ShootDegrees(FinalShotAngle);
        }

        public virtual void ModifyBullet(GameObject baseBullet)
        {
        }

        public virtual void AimAt(Vector2 target)
        {
            GunModel.AimAt(target);
            UpdateGunRenderer();
        }

        public virtual void UpdateGunRenderer()
        {
            gunRenderer.flipX = GunModel.IsFlipped;
            gunRenderer.sortingOrder = GunInFront ? 1 : -1;
        }

        private void Update()
        {
            GunModel.Update(Time.deltaTime);
        }
    }
}
