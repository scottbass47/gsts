using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GunShaderTest : MonoBehaviour
{
    private SpriteRenderer renderer;
    public Material rotateMaterial;
    public Material outlineMaterial;
    public Texture2D mainTex;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        //renderer.materials = new Material[] 
        //{
        //    rotateMaterial,
        //    outlineMaterial
        //};

        //rotateMaterial.SetFloat("_Resolution", 20);
        //rotateMaterial.SetTexture("_MainTex", mainTex);

        //outlineMaterial.SetFloat("_Resolution", 20);
        //outlineMaterial.SetTexture("_MainTex", mainTex);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Debug.Log("test");
        outlineMaterial.SetFloat("_Resolution", 20);
        outlineMaterial.SetColor("_OutlineColor", Color.white);
        Graphics.Blit(source, destination, outlineMaterial);
    }
}
