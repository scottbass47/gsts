using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns
{
    public class SimpleEnemyGun : Gun
    {
        [Header("Gun Stats")]
        [SerializeField] private float AccurateShotRange = 2.0f;
        [SerializeField] private BaseGunStats stats;

        private GunModel gunModel;
        public override GunModel GunModel => gunModel;
        public override GunStats GunStats => stats; 

        public void Start()
        {
            gunModel = new GunModel(
                new BottomlessClip(),
                new AccurateAim(AccurateShotRange),
                new GunPositioning(center, hand, gun, pivot, barrel),
                0.0f,
                stats
            );
            rotateBullet = true;
        }
    }
}
