using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Effects
{
    public class EffectHandler : MonoBehaviour
    {
        private StatusEffectHandler statusEffectHandler;
        private HashSet<EffectType> immunities;

        private void Awake()
        {
            statusEffectHandler = new StatusEffectHandler(this.gameObject);
            immunities = new HashSet<EffectType>();
        }

        public void AddImmunity(EffectType effectType)
        {
            immunities.Add(effectType);
        }

        public void RemoveImmunity(EffectType effectType)
        {
            immunities.Remove(effectType);
        }

        public void AddEffect(StatusEffect effect)
        {
            if (immunities.Contains(effect.EffectType)) return;

            statusEffectHandler.AddEffect(effect);
        }

        public void RemoveAllEffects()
        {
            statusEffectHandler.RemoveAllEffects();
        }

        // Update is called once per frame
        void Update()
        {
            statusEffectHandler.Update();
        }
    }

    class StatusEffectHandler
    {
        private GameObject targetObject;
        private List<StatusEffect> statusEffects;

        private bool renderersInUse = false;

        private Dictionary<SpriteRenderer, Material> defaultMaterials;
        private Dictionary<SpriteRenderer, MaterialPropertyBlock> materialPropsPerRenderer;

        private Material vfxMaterial;

        public StatusEffectHandler(GameObject targetObject)
        {
            this.targetObject = targetObject;
            statusEffects = new List<StatusEffect>();

            vfxMaterial = new Material(Materials.Instance.VFXMaterial);

            defaultMaterials = new Dictionary<SpriteRenderer, Material>();
            materialPropsPerRenderer = new Dictionary<SpriteRenderer, MaterialPropertyBlock>();
            ResetRenderers();
        }

        private void ResetRenderers()
        {
            defaultMaterials.Clear();

            var renderers = targetObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in renderers)
            {
                defaultMaterials.Add(renderer, renderer.material);
                materialPropsPerRenderer.Add(renderer, new MaterialPropertyBlock());
            }
        }

        public void AddEffect(StatusEffect statusEffect)
        {
            var existingEffect = GetStatusEffectOfSameType(statusEffect);
            if(existingEffect != null)
            {
                existingEffect.CombineEffects(statusEffect);
            }
            else
            {
                statusEffect.OnApply(targetObject);
                statusEffects.Add(statusEffect);
                SortEffects();
            }
        }

        private void SortEffects()
        {
            // Descending order by priority
            statusEffects.Sort((effectOne, effectTwo) =>
            {
                int p1 = VFXPriorityTable.GetPriority(effectOne.EffectType);
                int p2 = VFXPriorityTable.GetPriority(effectTwo.EffectType);
                return p2 - p1;
            });
        }

        private StatusEffect GetStatusEffectOfSameType(StatusEffect statusEffect)
        {
            foreach(var effect in statusEffects)
            {
                if(effect.EffectType == statusEffect.EffectType)
                {
                    return effect;
                }
            }
            return null;
        }

        public void Update()
        {
            renderersInUse = false;

            foreach(var statusEffect in statusEffects)
            {
                statusEffect.Update();

                if(statusEffect is VisualEffect)
                {
                    if (!renderersInUse) SetRenderersToVFXMaterial();
                    renderersInUse = true;
                    RenderVFX((VisualEffect)statusEffect);
                }
            }

            RemoveEffectsIfDone();

            if (!renderersInUse)
            {
                SetRenderersToDefaultMaterial();
            }
        }

        private void SetRenderersToDefaultMaterial()
        {
            foreach(var renderer in defaultMaterials.Keys)
            {
                renderer.material = defaultMaterials[renderer];
            }
        }

        private void SetRenderersToVFXMaterial()
        {
            foreach(var renderer in defaultMaterials.Keys)
            {
                renderer.material = vfxMaterial;
            }
        }

        private void RenderVFX(VisualEffect vfx)
        {
            foreach(var renderer in defaultMaterials.Keys)
            {
                var materialProp = materialPropsPerRenderer[renderer];
                renderer.GetPropertyBlock(materialProp);
                vfx.Render(materialProp);
                renderer.SetPropertyBlock(materialProp);
            }
        }

        private void RemoveEffectsIfDone()
        {
            for(int i =  statusEffects.Count - 1; i >= 0; i--)
            {
                var statusEffect = statusEffects[i];
                if (statusEffect.Done)
                {
                    RemoveEffect(statusEffect);
                    statusEffects.RemoveAt(i);
                }
            }
        }

        private void RemoveEffect(StatusEffect statusEffect)
        {
            statusEffect.OnRemove();
            if(statusEffect is VisualEffect)
            {
                RemoveVFX((VisualEffect)statusEffect);
            }
        }

        private void RemoveVFX(VisualEffect vfx)
        {
            foreach(var renderer in defaultMaterials.Keys)
            {
                var materialProps = materialPropsPerRenderer[renderer];
                renderer.GetPropertyBlock(materialProps);
                vfx.ResetProperties(materialProps);
                renderer.SetPropertyBlock(materialProps);
            }
        }

        public void RemoveAllEffects()
        {
            foreach(var statusEffect in statusEffects)
            {
                RemoveEffect(statusEffect);
            }
            statusEffects.Clear();
        }
    }
}
