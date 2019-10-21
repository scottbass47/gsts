using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public bool FacingRight
    {
        set
        {
            spriteRenderer.flipX = !value && flippable;           
        }
    }

    [SerializeField] private bool flippable;
    [SerializeField] private bool flashOnDeath;
    [SerializeField] private bool fadeOnDeath;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(GameObject enemy)
    {
        transform.position = enemy.transform.position;
        FacingRight = enemy.GetComponent<Movement>().FacingRight;
        if (flashOnDeath)
        {
            enemy.GetComponent<AI>().Flash(0.2f);
        }
    }

    public void AnimationFinished()
    {
        if (fadeOnDeath)
        {
            StartCoroutine(DeathFade(gameObject));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DeathFade(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<SpriteRenderer>();
        float duration = 1.5f;

        float t = 0;
        while(t < duration)
        {
            t += Time.deltaTime;
            foreach(var renderer in renderers)
            {
                renderer.color = new Color(1, 1, 1, 1 - t / duration);
            }
            yield return null;
        }
        Destroy(go);
    }
}
