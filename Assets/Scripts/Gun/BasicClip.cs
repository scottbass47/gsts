using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns
{
    public class BasicClip : Clip
    {
        private int bulletsLeft;
        public int BulletsLeftInClip => bulletsLeft;

        private int clipSize;
        public int ClipSize
        {
            get => clipSize;
            set
            {
                if (value > 0)
                {
                    this.clipSize = value;
                }
            }
        }

        public BasicClip(int clipSize)
        {
            ClipSize = clipSize;
        }

        public void DecrementBullets()
        {
            bulletsLeft--;
        }

        public bool IsEmpty()
        {
            return BulletsLeftInClip <= 0;
        }

        public void ReloadBullets()
        {
            bulletsLeft = ClipSize;
        }
    }

}
