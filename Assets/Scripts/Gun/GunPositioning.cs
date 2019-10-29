using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns
{
    public class GunPositioning 
    {
        private Transform center;
        private Transform hand;
        private Transform gun;
        private Transform pivot;
        private Transform barrel;

        public Vector2 CenterWorldPos => center.transform.position;
        public Vector2 PivotWorldPos => pivot.transform.position;
        public Vector2 BarrelWorldPos => barrel.transform.position;

        private Vector2 gunLocalRHS;
        private Vector2 handLocalRHS;
        private Vector2 pivotLocalRHS;
        private Vector2 barrelLocalRHS;

        // Transform structure
        // -------------------
        // center
        //      hand
        //      gun
        //          pivot
        //          barrel
        public GunPositioning(Transform center, Transform hand, Transform gun, Transform pivot, Transform barrel)
        {
            this.center = center;
            this.hand = hand;
            this.gun = gun;
            this.pivot = pivot;
            this.barrel = barrel;

            SaveLocalPositions();
        }

        private void SaveLocalPositions()
        {
            this.gunLocalRHS = new Vector2(gun.localPosition.x, gun.localPosition.y);
            this.handLocalRHS = new Vector2(hand.localPosition.x, hand.localPosition.y);
            this.pivotLocalRHS = new Vector2(pivot.localPosition.x, pivot.localPosition.y);
            this.barrelLocalRHS = new Vector2(barrel.localPosition.x, barrel.localPosition.y);
        }

        public void PutGunInStandardPosition()
        {
            gun.localPosition = handLocalRHS - pivotLocalRHS;
            gun.rotation = Quaternion.identity;
            pivot.localPosition = pivotLocalRHS;
            barrel.localPosition = barrelLocalRHS;
        }

        public void RotateGun(float aimAngle, bool flipped)
        {
            PutGunInStandardPosition();

            gun.localPosition += (Vector3)pivotLocalRHS;

            var rotation = Quaternion.Euler(0, 0, aimAngle);
            gun.rotation = rotation;

            var pivotPos = rotation * pivotLocalRHS;
            gun.localPosition -= pivotPos;

            if (flipped) FlipGun(aimAngle);
        }

        private void FlipGun(float aimAngle)
        {
            FlipGunRotate(aimAngle);
            FlipLocal(gun);
            FlipLocal(pivot);
            FlipLocal(barrel);
        }

        private void FlipGunRotate(float aimAngle)
        {
            gun.rotation = Quaternion.Euler(0, 0, 360 - aimAngle);
        }

        private void FlipLocal(Transform transform)
        {
            var flippedPos = new Vector3(
                -transform.localPosition.x,
                transform.localPosition.y
            );
            transform.localPosition = flippedPos;
        }
    }
}
