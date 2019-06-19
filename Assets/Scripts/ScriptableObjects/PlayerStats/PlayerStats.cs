using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Stats")]
public class PlayerStats : ScriptableObject
{
    [SerializeField] private int maxHealth;
    [SerializeField] private float moveSpeed;

    public int MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
}
