using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns
{
    public interface GunStats
    {
        float Spread { get; }
        float Damage { get; }
        float BulletSpeed { get; }
        float FireRate { get; }
        float KnockbackAmount { get; }
        bool IsSemiAuto { get; }
    }
}
