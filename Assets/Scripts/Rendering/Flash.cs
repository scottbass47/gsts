using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    private static Material flashMaterial;

    // Start is called before the first frame update
    private void Start()
    {
        flashMaterial = Materials.Instance.FlashMaterial;
    }

    public void StartFlash(float duration)
    {
        StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        Material[] old = new Material[renderers.Length];
        for(int i = 0; i < renderers.Length; i++)
        {
            var render = renderers[i];
            old[i] = render.material;
            render.material = flashMaterial;
            render.material.SetColor("_BlinkColor", Color.white);
        }
        yield return new WaitForSeconds(duration);

        for(int i = 0; i < renderers.Length; i++)
        {
            var render = renderers[i];
            render.material = old[i];
        }
    }


}
