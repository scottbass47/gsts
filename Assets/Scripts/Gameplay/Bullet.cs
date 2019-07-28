﻿using System.Collections;
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

    private Rigidbody2D rb2d;
    private Vector2 dir;

    private float speed;
    private float damage;
    private float knockbackAmount;

	// Use this for initialization
	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
	}

    // Angle in radians
    public void Shoot(float angle)
    {
        this.Angle = angle;

        dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        rb2d.velocity = Speed * dir;
        transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);
    }

    public void Shoot(Vector2 dir)
    {
        this.dir = dir;
        dir.Normalize();
        rb2d.velocity = Speed * dir;
        if(RotateTransform) transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        DamageManager.DealDamage(other, Damage);

        var physics = other.GetComponentInParent<Physics>();
        physics?.ApplyKnockback(dir, 0.2f, KnockbackAmount);

        Destroy(gameObject);
    }
}