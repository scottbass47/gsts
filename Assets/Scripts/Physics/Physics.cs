using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Physics : MonoBehaviour
{
    private Rigidbody2D body;
    private Vector2 netForce;
    private Movement movement;

    public float Weight
    {
        get;
        set;
    } = 1;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
        netForce = Vector2.zero;
    }

    public void AddForce(Vector2 force)
    {
        netForce += force; 
    }

    private void Update()
    {
        if(movement != null)
        {
            AddForce(movement.MoveDir * movement.MoveSpeed);
        }

        body.velocity = netForce;
        netForce = Vector2.zero;
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}

