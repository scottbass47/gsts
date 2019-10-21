using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class KnockbackEffect : StatusEffect
    {
        private KnockbackHandler knockbackHandler;
        private Physics targetPhysics;

        public KnockbackEffect(float duration, Vector2 dir, float force) : base(EffectType.Knocback, duration)
        {
            knockbackHandler = new KnockbackHandler();
            knockbackHandler.SetKnockback(dir, duration, force);
        }

        public override void CombineEffects(StatusEffect existingEffect)
        {
            var existingKnockback = (KnockbackEffect)existingEffect;
            knockbackHandler.SetKnockback(existingKnockback.knockbackHandler);
        }

        public override void OnApply(GameObject target)
        {
            targetPhysics = target.GetComponent<Physics>();
            knockbackHandler.Weight = targetPhysics.Weight;
        }

        public override void OnRemove()
        {
        }

        public override void OnUpdate()
        {
            knockbackHandler.Update();
            targetPhysics.AddForce(knockbackHandler.GetKnockbackForce());
        }
    }

    public class KnockbackHandler
    {
        private bool applyingKnockback;
        private Vector2 knockbackForce;
        private float duration;
        private float elapsed;
        private float weight;

        public bool KnockbackActive => applyingKnockback;
        public float Weight
        {
            get => weight;
            set => weight = value;
        }

        public KnockbackHandler()
        {
            weight = 1;
        }

        public void SetKnockback(Vector2 knockbackDir, float duration, float force)
        {
            knockbackDir.Normalize();
            if (applyingKnockback)
            {
                float oldForce = knockbackForce.magnitude;
                SetKnockbackDirection(knockbackDir * force);
                SetKnockbackForce(oldForce, force);
                this.duration = Mathf.Max(this.duration, duration);
            }
            else
            {
                knockbackForce = knockbackDir * force;
                this.duration = duration;
            }
            applyingKnockback = true;
            elapsed = 0;
        }

        public void SetKnockback(KnockbackHandler knockbackHandler)
        {
            SetKnockback(
                knockbackHandler.knockbackForce.normalized,
                knockbackHandler.duration,
                knockbackHandler.knockbackForce.magnitude
            );
        }

        private void SetKnockbackDirection(Vector2 newKnockbackVector)
        {
            knockbackForce += newKnockbackVector;
        }

        private void SetKnockbackForce(float oldForce, float newForce)
        {
            knockbackForce = knockbackForce.normalized * Mathf.Min(oldForce, newForce);
        }

        public void Update()
        {
            elapsed += Time.deltaTime;
            if(elapsed >= duration)
            {
                applyingKnockback = false;
                return;
            }
        }

        public Vector2 GetKnockbackForce()
        {
            if (!applyingKnockback) return Vector2.zero;
            float timeLeft = duration - elapsed;
            float forceFraction = timeLeft / duration;
            float force = knockbackForce.magnitude;
            return knockbackForce.normalized * forceFraction * force / weight; 
        }
    }
}
