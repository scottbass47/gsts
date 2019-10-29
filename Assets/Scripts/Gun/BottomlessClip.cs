using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns
{
    public class BottomlessClip : Clip
    {
        public int BulletsLeftInClip => 0;
        public int ClipSize
        {
            get => 0;
            set { } 
        }

        public void DecrementBullets()
        {
        }

        public bool IsEmpty()
        {
            return false;
        }

        public void ReloadBullets()
        {
        }
    }
}
