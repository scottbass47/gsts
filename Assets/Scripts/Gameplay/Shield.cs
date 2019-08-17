using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float ShieldTime { get; set; }

    private ShieldState shieldState;
    public ShieldState ShieldState => shieldState;

    private Material material;

    private void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    public void TransitionShield(bool cast, Action transitionOverCallback)
    {
        StartCoroutine(ShieldRoutine(cast, transitionOverCallback));
    }

    private IEnumerator ShieldRoutine(bool cast, Action callback)
    {
        shieldState = ShieldState.Transitioning;
        float elapsed = 0;

        while(elapsed < ShieldTime)
        {
            elapsed += Time.deltaTime;
            var pct = elapsed / ShieldTime;
            material.SetFloat("_PercentOn", Mathf.Clamp01(cast ? 1 - pct : pct));
            yield return null;
        }
        shieldState = cast ? ShieldState.Casted : ShieldState.Dropped;
        callback.Invoke();
    }
}

public enum ShieldState
{
    Casted,
    Dropped, 
    Transitioning
}
