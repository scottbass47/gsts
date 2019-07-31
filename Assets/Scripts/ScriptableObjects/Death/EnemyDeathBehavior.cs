using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Enemy/Death Behavior")]
public class EnemyDeathBehavior : DeathBehavior
{
    public override void OnDeath(Death death)
    {
        var go = death.gameObject;

        death.DestroyOnDeath = false;
        go.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        var allComps = go.gameObject.GetComponentsInChildren<Behaviour>();
        foreach(var comp in allComps)
        {
            var type = comp.GetType();
            if (type == typeof(Transform) ||
               type == typeof(Animator) ||
               type == typeof(SortingGroup) ||
               type == typeof(SpriteRenderer)) continue;
            comp.enabled = false;
        }
        go.GetComponentInChildren<Animator>().SetTrigger("death");
        death.OnDeathAnimFinish += () =>
        {
            death.StartCoroutine(DeathFade(go));
        };
        SoundManager.PlaySound(Sounds.EnemyFleshHitFatal);
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
