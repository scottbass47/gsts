using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class FlashEffect : VisualEffect
    {
        public FlashEffect(float duration) : base(EffectType.Flash, duration)
        {
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
            materialProps.SetFloat("_FlashOn", 1.0f);
        }

        public override void ResetProperties(MaterialPropertyBlock materialProps)
        {
            materialProps.SetFloat("_FlashOn", 0.0f);
        }

        public override bool UsingRenderers()
        {
            return true;
        }
    }
}
