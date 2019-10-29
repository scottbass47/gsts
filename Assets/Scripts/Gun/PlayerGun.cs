using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns
{
    public class PlayerGun : Gun
    {
        private GunModel gunModel;
        public override GunModel GunModel => gunModel;

        [Header("Gun Stats")]
        [SerializeField] private float accurateShootingRange;
        [SerializeField] private BaseGunStats baseGunStats;
        public override GunStats GunStats => baseGunStats;

        private EventManager events;

        private void Start()
        {
            events = GameManager.Instance.Events;

            gunModel = new GunModel(
                new BasicClip(baseGunStats.ClipSize),
                new AccurateAim(accurateShootingRange),
                new GunPositioning(center, hand, gun, pivot, barrel),
                baseGunStats.ReloadRate,
                GunStats
            );

            GunModel.OnReloadBegin += OnReloadBegin;
            GunModel.OnChangeInBulletsLeftInClip += OnChangeInBulletsLeftInClip;
            GunModel.OnChangeInClipSize += OnChangeInClipSize;

            // Triggers updates in the HUD 
            GunModel.ChangeClipSize(baseGunStats.ClipSize);
        }

        public override void ShootBullet()
        {
            base.ShootBullet();
            PlayGunShotSound();
            FireBulletShotEvent();
        }

        private void PlayGunShotSound()
        {
            SoundManager.PlaySound(Sounds.PlayerGunshot);
        }

        private void FireBulletShotEvent()
        {
            events.FireEvent(new WeaponFired());
        }

        // Events

        private void OnChangeInClipSize()
        {
            events.FireEvent(new WeaponMagChange { Bullets = GunModel.ClipSize });
        }

        private void OnChangeInBulletsLeftInClip()
        {
            events.FireEvent(new WeaponClipChange { BulletsInClip = GunModel.BulletsLeftInClip });
        }

        private void OnReloadBegin()
        {
            events.FireEvent(new Reload { Duration = GunModel.ReloadTime });
        }
    }
}
