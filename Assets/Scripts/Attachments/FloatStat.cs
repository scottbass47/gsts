using UnityEngine;
using System.Collections;

namespace Attachments
{
    public class FloatStat : Stat<float>
    {
        public override float Value => BaseValue * Multiplier;

        public FloatStat(float baseValue) : base(baseValue)
        {
        }

        public static implicit operator float(FloatStat stat) => stat.Value;
    }
}
