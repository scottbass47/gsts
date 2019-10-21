using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Effects
{
    public class StunEffect : VisualEffect
    {
        private GameObject target;

        public StunEffect(float duration) : base(EffectType.Stun, duration)
        {
        }

        public override void CombineEffects(StatusEffect effect)
        {
            Duration = Mathf.Max(effect.Duration, Duration);
            ResetEffect();
        }

        public override void OnApply(GameObject target)
        {
            this.target = target;

            Stun(true);
        }

        public override void OnRemove()
        {
            Stun(false);
        }

        private void Stun(bool stunned)
        {
            StunAI(stunned);
            StunAnimators(stunned);
            StunMovement(stunned);
        }

        private void StunAI(bool stunned)
        {
            var ai = target.GetComponent<AI>();
            if(ai != null)
            {
                ai.Stunned = stunned;
            }
        }

        private void StunAnimators(bool stunned)
        {
            var animators = target.GetComponentsInChildren<Animator>();
            foreach (var animator in animators) animator.enabled = !stunned;
        }

        private void StunMovement(bool stunned)
        {
            var movement = target.GetComponent<Movement>();
            if(movement != null)
            {
                movement.Locked = stunned;
            }
        }

        public override void OnUpdate()
        {
        }

        public override void Render(MaterialPropertyBlock materialProps)
        {
            materialProps.SetFloat("_Stunned", 1.0f);
        }

        public override bool UsingRenderers()
        {
            return true;
        }

        public override void ResetProperties(MaterialPropertyBlock materialProps)
        {
            materialProps.SetFloat("_Stunned", 0.0f);
        }
    }
}
