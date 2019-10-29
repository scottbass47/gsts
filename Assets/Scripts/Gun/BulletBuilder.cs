using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns
{
    public class BulletBuilder
    {
        private GameObject bulletObj;
        private Bullet bullet;

        public BulletBuilder(GameObject bulletObj)
        {
            this.bulletObj = bulletObj;
            this.bullet = bulletObj.GetComponent<Bullet>();
        }

        public BulletBuilder Damage(float damage)
        {
            bullet.Damage = damage;
            return this;
        }

        public BulletBuilder Knockback(float knockback)
        {
            bullet.KnockbackAmount = knockback;
            return this;
        }

        public BulletBuilder Speed(float speed)
        {
            bullet.Speed = speed;
            return this;
        }

        public BulletBuilder RotateTransform(bool rotate)
        {
            bullet.RotateTransform = rotate;
            return this;
        }

        public Bullet Build()
        {
            return bullet;
        } 
    }
}
