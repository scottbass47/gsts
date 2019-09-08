using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    private Material material;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private Transform flashOrigin;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void InitializeFlash(Vector3 pos, float degrees, bool flip, int orderInLayer)
    {
        gameObject.SetActive(true);
        var rotation = Quaternion.Euler(0, 0, degrees);
        var rotatedOrigin = rotation * flashOrigin.localPosition;
        rotatedOrigin.x = flip ? -rotatedOrigin.x : rotatedOrigin.x;
        transform.position = pos - rotatedOrigin;
        material.SetFloat("_Rotation", degrees);
        spriteRenderer.sortingOrder = orderInLayer;
        spriteRenderer.flipX = flip;
    }

    public void FlashCompleted()
    {
        Destroy(gameObject);
    }
}
