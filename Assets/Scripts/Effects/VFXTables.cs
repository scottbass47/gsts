using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public static class VFXPriorityTable 
    {
        private static Dictionary<EffectType, int> table = new Dictionary<EffectType, int>()
        {
            { EffectType.Flash, 200 },
            { EffectType.Stun, 100 },
        };

        public static int GetPriority(EffectType type)
        {
            int priority = 0;
            table.TryGetValue(type, out priority);
            return priority;
        }
    }
}
