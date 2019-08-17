using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDroneAttackDirection : MonoBehaviour
{
    public Direction Direction { get; private set; }

    public void EventCallback(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > 0.5)
        {
            switch (evt.intParameter)
            {
                case 0: Direction = Direction.Front; break;
                case 1: Direction = Direction.Front45; break;
                case 2: Direction = Direction.Side; break;
                case 3: Direction = Direction.Back45; break;
                case 4: Direction = Direction.Back; break;
            }
        }
    }
}
