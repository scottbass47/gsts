using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float elapsed;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int frameRate = 10;

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
    
