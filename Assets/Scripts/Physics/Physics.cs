using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Physics : MonoBehaviour
{
    private Rigidbody2D body;
    private Vector2 netForce;
    private bool applyingKnockback;
    private IMovement movement;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        movement = GetComponent<IMovement>();
        netForce = Vector2.zero;
    }

    public void AddForce(Vector2 force)
    {
        netForce += force; 
    }

    private void Update()
    {
        body.velocity = netForce;
        if(movement != null)
        {
            body.velocity += movement.MoveDir * movement.MoveSpeed;
        }
        netForce = Vector2.zero;
    }

    public void ApplyKnockback(Vector2 dir, float duration, float force)
    {
        if (applyingKnockback) return;
        StartCoroutine(KnockbackCoroutine(dir, duration, force));
    }

    private IEnumerator KnockbackCoroutine(Vector2 dir, float duration, float force)
    {
        applyingKnockback = true;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            AddForce(force * dir);
            yield return null;
        }
        applyingKnockback = false;        
    }
}
