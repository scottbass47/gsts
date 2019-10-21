using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public abstract class StatusEffect 
    {
        public float Duration { get; set;  }
        private float elapsed;
        protected float Elapsed => elapsed;

        public bool Done => elapsed >= Duration;

        private EffectType effectType;
        public EffectType EffectType => effectType;

        public StatusEffect(EffectType type, float duration)
        {
            effectType = type;
            Duration = duration;
            elapsed = 0;
        }

        public void Update()
        {
            if (Done) return;
            OnUpdate();
            elapsed += Time.deltaTime;
        }

        public void ResetEffect()
        {
            elapsed = 0;
        }

        public abstract void CombineEffects(StatusEffect effect);
        public abstract void OnApply(GameObject target);
        public abstract void OnUpdate();
        public abstract void OnRemove();
    }
}
