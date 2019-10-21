using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attachments
{
    public abstract class Stat<T>
    {
        abstract public T Value { get; }

        public T BaseValue { get; set; }

        private float multiplier = 1;
        public float Multiplier => multiplier;

        public Stat(T baseValue)
        {
            BaseValue = baseValue;
        }

        public void ChangeMultiplier(float deltaPercent)
        {
            multiplier += deltaPercent;
        }
    }
}
