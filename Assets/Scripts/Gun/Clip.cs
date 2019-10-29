using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns
{
    public interface Clip
    {
        int BulletsLeftInClip { get; }
        int ClipSize { get; set; }

        void DecrementBullets();
        void ReloadBullets();
        bool IsEmpty();
    }
}
