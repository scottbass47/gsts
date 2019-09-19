using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Teleporter")]
public class TeleporterStats : EnemyStats
{
    [Header("General")]
    [SerializeField] private float health;

    [Header("Teleport")]
    [SerializeField] private IntRange enemiesToTelport;
    [SerializeField] private FloatRange cooldown;
    [SerializeField] private float minTeleportDistance;
    [SerializeField] private float animationDelay;

    public override float Health => health;

    public IntRange EnemiesToTelport => enemiesToTelport;
    public FloatRange Cooldown => cooldown;
    public float MinTeleportDistance => minTeleportDistance;
    public float AnimationDelay => animationDelay;
}
