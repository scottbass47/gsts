using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float Angle { get; set; } 

    public float Speed
    {
        get => speed;
        set => speed = value;
    }
    public float Damage
    {
        get => damage;
        set => damage = value;
    }
    public float KnockbackAmount
    {
        get => knockbackAmount;
        set => knockbackAmount = value;
    }

    public bool RotateTransform { get; set; } = true;

    private Vector2 dir;

    private float speed;
    private float damage;
    private float knockbackAmount;

    [Header("Collision")]
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float radius;

    [Header("Bullet Impact")]
    [SerializeField] private GameObject bulletImpactPrefab;
    [SerializeField] private Sprite[] bulletImpactAnimation;
    [SerializeField] private bool useImpact;
    [SerializeField] private Vector2 impactOffset;

    [Header("Rotation")]
    [SerializeField] private Transform rotate;

    // Angle in radians
    public void Shoot(float angle)
    {
        this.Angle = angle;

        dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        if (RotateTransform) rotate.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
    }

    public void Shoot(Vector2 dir)
    {
        this.dir = dir;
        dir.Normalize();
        if(RotateTransform) rotate.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    private void Update()
    {
        var dist = Speed * Time.deltaTime;
        transform.Translate(dir * dist);
        var hits = Physics2D.CircleCastAll(transform.position, radius, dir, dist, hitMask);
        if(hits.Length > 0)
        {
            var first = hits[0];
            OnCollide(first.collider, first.point, first.normal);
        }
    }

    private void OnCollide(Collider2D collider, Vector2 collisionPoint, Vector2 normal)
    {
        GameObject other = collider.gameObject;
        bool damageDealt = DamageManager.DealDamage(other, Damage);

        var physics = other.GetComponentInParent<Physics>();
        physics?.ApplyKnockback(dir, 0.2f, KnockbackAmount);
        
        if(useImpact && collider.tag == "WallsCollision")
        {
            var obj = Instantiate(bulletImpactPrefab);
            var transform = obj.GetComponent<BulletImpact>().BulletRotate;
            obj.transform.position = collisionPoint;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg);
            obj.transform.position += transform.rotation * impactOffset;
            var anim = obj.GetComponent<AnimationOnceThenDestroy>();
            anim.Animation = bulletImpactAnimation;
            anim.StartAnimation();
        }
        Destroy(gameObject);
    }
}
