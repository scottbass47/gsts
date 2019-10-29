using Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float Angle { get; set; } 

    private float speed;
    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    private float damage;
    public float Damage
    {
        get => damage;
        set => damage = value;
    }

    private float knockbackAmount;
    public float KnockbackAmount
    {
        get => knockbackAmount;
        set => knockbackAmount = value;
    }

    public bool RotateTransform { get; set; } = true;
    private Vector2 dir;

    [Header("Collision")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private float radius;

    [Header("Rotation")]
    [SerializeField] private Transform rotate;

    public void ShootDegrees(float angle)
    {
        ShootRadians(angle * Mathf.Deg2Rad);
    }

    public void Shoot(Vector2 dir)
    {
        ShootRadians(Mathf.Atan2(dir.y, dir.x));
    }

    public void ShootRadians(float angle)
    {
        this.Angle = angle;
        this.dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        if (RotateTransform) rotate.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
    }

    private void Update()
    {
        var dist = MoveBullet();
        CheckCollision(dist);
    }

    private void CheckCollision(float distTraveled)
    {
        var hits = Physics2D.CircleCastAll(transform.position, radius, dir, distTraveled, wallMask | targetMask);
        if(hits.Length > 0)
        {
            var first = hits[0];
            OnCollide(first.collider, first.point, first.normal);
        }
    }

    private float MoveBullet()
    {
        var dist = Speed * Time.deltaTime;
        transform.Translate(dir * dist);
        return dist;
    }

    private void OnCollide(Collider2D collider, Vector2 collisionPoint, Vector2 normal)
    {
        GameObject other = collider.gameObject;
        bool damageDealt = DamageManager.DealDamage(other, Damage);

        var effectHandler = other.GetComponentInParent<EffectHandler>();
        effectHandler?.AddEffect(new KnockbackEffect(0.2f, dir, KnockbackAmount));
        
        if(Physics.IsInLayerMask(other.layer, wallMask))
        {
            GetComponent<CreateBulletImpact>()?.Create(collisionPoint, normal);
        }
        Destroy(gameObject);
    }
}
