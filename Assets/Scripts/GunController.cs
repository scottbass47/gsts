using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public bool Flipped { get; set; }
    private float aimAngle;
    private float angleThreshold = 15; // Used for over-aiming
    private bool aimingRight;
    private Vector3 offset;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        offset = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Angle in [0, 360)
    public void SetAimAngle(float angle)
    {
        bool right = RightSide(angle);
        aimAngle = angle;

        bool shouldFlip = false;
        if (aimingRight && !right)
        {
            shouldFlip = angle > 90 + angleThreshold && angle < 270 - angleThreshold;
        }
        else if (!aimingRight && right)
        {
            shouldFlip = angle < 90 - angleThreshold || angle > 270 + angleThreshold;
        }

        bool flipped = shouldFlip == aimingRight;

        Vector3 translate = Vector3.zero;
        transform.eulerAngles = new Vector3(0, 0, flipped ? angle + 180 : angle);
        //pixelRotate.SetRotate(flipped ? 180 - angle : angle, HandlePivot + pixelRotate.PivotCenter, out translate);

        spriteRenderer.flipX = flipped;
        //transform.localPosition = localPos + translate;

        if (flipped)
        {
            transform.localPosition = new Vector3(-offset.x, offset.y, 0);
        }
        else
        {
            transform.localPosition = offset;
        }
        if (shouldFlip) aimingRight = !aimingRight;

        Flipped = flipped;
    }

    private bool RightSide(float angle)
    {
        return angle >= 270 || angle <= 90;
    }
}
