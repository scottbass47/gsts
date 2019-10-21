using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class BlinkEffect : VisualEffect
    {
        private float blinkFrequency;

        public BlinkEffect(float duration, float blinkFrequency) : base(EffectType.Blink, duration)
        {
            this.blinkFrequency = blinkFrequency;
        }

        public override void CombineEffects(StatusEffect effect)
        {
        }

        public override void OnApply(GameObject target)
        {
        }

        public override void OnRemove()
        {
        }

        public override void OnUpdate()
        {
        }

        public override void Render(MaterialPropertyBlock materialProps)
        {
            var blinkNum = (int)(Elapsed / blinkFrequency);
            bool on = blinkNum % 2 == 0;
            materialProps.SetFloat("_BlinkOn", on ? 1.0f : 0.0f);
        }

        public override void ResetProperties(MaterialPropertyBlock materialProps)
        {
            materialProps.SetFloat("_BlinkOn", 0.0f);
        }

        public override bool UsingRenderers()
        {
            return true;
        }
    }

}
