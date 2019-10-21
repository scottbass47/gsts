using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public abstract class VisualEffect : StatusEffect
    {
        public VisualEffect(EffectType type, float duration) : base(type, duration)
        {
        }

        public abstract bool UsingRenderers();
        public abstract void Render(MaterialPropertyBlock materialProps);
        public abstract void ResetProperties(MaterialPropertyBlock materialProps);
    }

}
