using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns
{
    public class AccurateAim : Aim
    {
        public Vector2 originWorldPos => gunPositioning.CenterWorldPos;

        private float aimAngle;
        public float AimAngle => aimAngle;

        public bool Flipped => OriginToTarget.x < 0;

        private Vector2 pivotWorldPos => gunPositioning.PivotWorldPos;
        private Vector2 barrelOffsetWorldPos => gunPositioning.BarrelWorldPos;
        private Vector2 targetWorldPos;

        private Vector2 OriginToPivot => pivotWorldPos - originWorldPos;
        private Vector2 PivotToBarrel => barrelOffsetWorldPos - pivotWorldPos;
        private float PivotToBarrelDist => PivotToBarrel.magnitude;

        private Vector2 OriginToTarget => targetWorldPos - originWorldPos;
        private Vector2 OriginToTargetRightSide => Flipped ? CreateReflectedOverYAxisVector(OriginToTarget) : OriginToTarget;

        private Vector2 pivotToTarget => OriginToTargetRightSide - OriginToPivot;
        private float PivotToTargetDist => pivotToTarget.magnitude;

        private float accurateShootingRange;
        public float AccurateShootingRange
        {
            get => accurateShootingRange;
            set => accurateShootingRange = value;
        }

        private GunPositioning gunPositioning;
        public GunPositioning GunPositioning
        {
            set => gunPositioning = value;
        }

        public AccurateAim(float accurateShootingRange)
        {
            this.accurateShootingRange = accurateShootingRange;
        }

        public void AimAlong(Vector2 direction)
        {
            AimAt(originWorldPos + direction);
        }

        public void AimAt(Vector2 targetWorldPos)
        {
            gunPositioning.PutGunInStandardPosition();

            this.targetWorldPos = targetWorldPos;

            if (PivotToTargetDist < accurateShootingRange)
            {
                SetSimpleAim();
            }
            else
            {
                SetAccurateAim();
            }

            gunPositioning.RotateGun(AimAngle, Flipped);
        }

        private void SetSimpleAim()
        {
            aimAngle = Mathf.Atan2(OriginToTargetRightSide.y, OriginToTargetRightSide.x) * Mathf.Rad2Deg;
        }

        private void SetAccurateAim()
        {
            float theta = Mathf.Acos(PivotToBarrel.x / PivotToBarrelDist);
            float beta = Mathf.PI - theta;
            float gamma = Mathf.Asin(PivotToBarrelDist / PivotToTargetDist * Mathf.Sin(beta));
            gamma = Mathf.Asin(PivotToBarrel.y / PivotToTargetDist);

            Vector2 pivotToTargetPrime = PivotToTargetDist * new Vector2(Mathf.Cos(gamma), Mathf.Sin(gamma));

            aimAngle = Vector2.SignedAngle(pivotToTargetPrime, pivotToTarget);
        }

        private Vector2 CreateReflectedOverYAxisVector(Vector2 vec)
        {
            return new Vector2(-vec.x, vec.y);
        }
    }
}
