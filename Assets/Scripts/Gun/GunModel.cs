using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Guns
{
    public class GunModel  
    {
        private GunStats gunStats;
        private Clip clip;
        private Aim aim;
        private GunPositioning gunPositioning;
        public GunPositioning GunPositioning => gunPositioning;

        private ReloadState reloadState;
        public float ReloadTime
        {
            get => reloadState.Duration;
            set
            {
                if (value >= 0) reloadState.Duration = value;
            }
        }

        public int ClipSize => clip.ClipSize;
        public int BulletsLeftInClip => clip.BulletsLeftInClip;
        public bool IsReloading => reloadState.Reloading;

        public bool IsFlipped => aim.Flipped; 
        public float AimAngle => aim.AimAngle; 
        public float AimAngleBullet => IsFlipped ? 180 - AimAngle : AimAngle; 

        public event Action OnReloadBegin;
        public event Action OnReloadEnd;
        public event Action OnChangeInBulletsLeftInClip;
        public event Action OnChangeInClipSize;
        public event Action OnBulletShot;

        private float timeSinceLastShot;

        public bool CanShoot => !IsReloading && !clip.IsEmpty() && (gunStats.FireRate * timeSinceLastShot >= 1);

        public GunModel(Clip clip, Aim aim, GunPositioning gunPositioning, float reloadTime, GunStats gunStats)
        {
            this.clip = clip;
            this.aim = aim;
            this.gunPositioning = gunPositioning;
            this.gunStats = gunStats;

            aim.GunPositioning = gunPositioning;

            reloadState = new ReloadState(FinishReload);
            ReloadTime = reloadTime;

            clip.ReloadBullets();
        }

        public bool TryShoot()
        {
            if (CanShoot)
            {
                timeSinceLastShot = 0;
                OnBulletShot?.Invoke();
                DecrementBullets();
                ReloadIfEmpty();
                return true;
            }
            return false;
        }

        private void DecrementBullets()
        {
            clip.DecrementBullets();
            OnChangeInBulletsLeftInClip?.Invoke();
        }

        private void ReloadIfEmpty()
        {
            if(clip.IsEmpty())
            {
                OnReloadBegin?.Invoke();
                reloadState.StartReload();
            }
        }

        public void Update(float deltaTime)
        {
            timeSinceLastShot += deltaTime;
            if(reloadState.Reloading)
            {
                reloadState.Update(deltaTime);
            }
        }

        private void FinishReload()
        {
            ReloadClip();
            OnReloadEnd?.Invoke();
        }

        public void AimAt(Vector2 targetPos)
        {
            aim.AimAt(targetPos);
        }

        public void ChangeClipSize(int newClipSize)
        {
            if (newClipSize < 0) return;

            clip.ClipSize = newClipSize;
            OnChangeInClipSize?.Invoke();

            ReloadClip();
        }

        private void ReloadClip()
        {
            clip.ReloadBullets();
            OnChangeInBulletsLeftInClip?.Invoke();
        }
    }
    public class ReloadState
    {
        public float Duration { get; set; }
        private float elapsed;

        public bool Done => elapsed >= Duration;
        public bool Reloading { get; private set; }

        private Action reloadFinished;

        public ReloadState(Action reloadFinished)
        {
            this.reloadFinished = reloadFinished;
        }

        public void StartReload()
        {
            elapsed = 0;
            Reloading = true;
        }

        public void Update(float dt)
        {
            elapsed += dt;
            if(Done)
            {
                Reloading = false;
                reloadFinished();
            }
        }
    }
}