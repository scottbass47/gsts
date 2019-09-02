using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimationOnceThenDestroy : MonoBehaviour
{
    private Sprite[] animation;
    public Sprite[] Animation
    {
        set
        {
            animation = value;
            elapsed = 0;
        }
    }

    private SpriteRenderer spriteRenderer;
    private float elapsed;
    [SerializeField] private int frameRate = 10;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartAnimation()
    {
        StartCoroutine(AnimationCoroutine());
    }

    private IEnumerator AnimationCoroutine()
    {
        while (true)
        {
            elapsed += Time.deltaTime;

            var currentFrame = (int)(elapsed * frameRate);
            if(currentFrame >= animation.Length)
            {
                Destroy(gameObject);
                yield break;
            }
            spriteRenderer.sprite = animation[currentFrame];
            yield return null;
        }
    }
}
    
