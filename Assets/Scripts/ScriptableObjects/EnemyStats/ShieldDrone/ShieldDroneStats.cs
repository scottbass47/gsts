using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Shield Drone")]
public class ShieldDroneStats : ScriptableObject
{
    [SerializeField] private float speed;
    [SerializeField] private float health;

    public float Speed => speed;
    public float Health => health;
}
