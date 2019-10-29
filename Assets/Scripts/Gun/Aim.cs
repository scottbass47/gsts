using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns
{
    public interface Aim
    {
        GunPositioning GunPositioning { set; }

        float AimAngle { get; }
        bool Flipped { get; }

        void AimAt(Vector2 target);

        void AimAlong(Vector2 direction);
    }
}
