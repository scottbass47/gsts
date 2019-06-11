using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float Angle = 0f;
    public float Speed = 10;
    public float Damage = 20;

    private Rigidbody2D rb2d;

	// Use this for initialization
	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
	}

    // Angle in radians
    public void Shoot(float angle)
    {
        this.Angle = angle;

        rb2d.velocity = new Vector2(Speed * Mathf.Cos(angle), Speed * Mathf.Sin(angle));
        transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);
    }
	
    // Update is called once per frame
    void Update () {
    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        Debug.Log($"{other.tag}");

        Health health = other.GetComponent<Health>();
        if(health != null)
        {
            health.Amount -= Damage;
        }

        Destroy(gameObject);
    }
}
