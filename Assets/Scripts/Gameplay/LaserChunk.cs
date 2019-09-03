using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserChunk : MonoBehaviour
{
    private Material material;

    private void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    public void Rotate(float degrees)
    {
        material.SetFloat("_Rotation", degrees);
    }
}
