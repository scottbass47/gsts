using UnityEngine;
using UnityEditor;

namespace Attachments
{
    public class IntStat : Stat<int>
    {
        public override int Value => (int)(BaseValue * Multiplier);

        public IntStat(int baseValue) : base(baseValue)
        {
        }

        public static implicit operator int(IntStat stat) => stat.Value;
    }
}