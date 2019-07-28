using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class MathUtils
{

    // Rotates about a point, rotating each pixel from its center
    public static Vector2 RotatePixel(int x, int y, float xcenter, float ycenter, float cos, float sin)
    {
        return new Vector2(
            ((x - xcenter + 0.5f) * cos - (y - ycenter + 0.5f) * sin) + xcenter,
            ((x - xcenter + 0.5f) * sin + (y - ycenter + 0.5f) * cos) + ycenter
        );
    }

    // Returns an angle between 0 and 360
    public static float ToPositiveDegrees(float degrees)
    {
        return degrees - 360 * Mathf.Floor(degrees / 360f);
    }

}

