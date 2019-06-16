using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour {

    public Sprite handFront;
    public Sprite handBack;

    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = handFront;
	}

    public void UpdateTexture(bool front, bool flip)
    {
        spriteRenderer.sprite = front ? handFront : handBack;
        spriteRenderer.flipX = flip;

        if ((flip && transform.localPosition.x > 0) || (!flip && transform.localPosition.x < 0))
        {
            transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, 0);
        }
    }
}
