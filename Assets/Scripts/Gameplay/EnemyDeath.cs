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
        FacingRight = enemy.GetComponent<IMovement>().FacingRight;

    }

    public void AnimationFinished()
    {
        StartCoroutine(DeathFade(gameObject));
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
