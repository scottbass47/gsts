using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineRenderer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialProps;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        materialProps = new MaterialPropertyBlock();
    }

    private void LateUpdate()
    {
        spriteRenderer.GetPropertyBlock(materialProps);
        materialProps.SetFloat("_OutlinesOn", 1.0f);
        spriteRenderer.SetPropertyBlock(materialProps);
    }
}
