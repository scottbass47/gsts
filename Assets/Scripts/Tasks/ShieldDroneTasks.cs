using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDroneTasks : BasicTasks
{
    [SerializeField] private Transform feet;
    [SerializeField] private ShieldDroneStats stats;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private BarrelPosition[] barrelPositions;
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private GameObject muzzleFlashPrefab;
    private Dictionary<Direction, Vector3> barrelPosDict;

    [Task] public bool IsAttacking { get; set; }

    private ShieldDroneAttackDirection attackDirection;
    private DamageFilter damageFilter;
    private Shield shield;
    private bool shieldTransitioning;

    public override void Awake()
    {
        base.Awake();
        barrelPosDict = new Dictionary<Direction, Vector3>();

        foreach(var barrelPos in barrelPositions)
        {
            barrelPosDict.Add(barrelPos.Direction, barrelPos.Position.localPosition);
        }
    }

    public override void Start()
    {
        base.Start();
        ai.Pos = feet;

        var playerBody = GameManager.Instance.Player.GetComponent<Body>();
        ai.Target = playerBody.CenterFeet;

        var health = GetComponent<Health>();
        health.Amount = stats.Health;

        attackDirection = GetComponentInChildren<ShieldDroneAttackDirection>();
        damageFilter = GetComponent<DamageFilter>();
        damageFilter.IsInvulnerable = true;

        shield = shieldObject.GetComponent<Shield>();
        shield.ShieldTime = 0.5f;

        enemyStats = stats;
        Speed = stats.Speed;
        TurningVelocity = float.MaxValue;
        pathParameters = new PathParameters(false, 0.05f);
    }

    [Task]
    public void StartAttack()
    {
        IsAttacking = true;
        Task.current.Succeed();
    }

    [Task]
    public void EndAttack()
    {
        IsAttacking = false;
        Task.current.Succeed();
    }

    [Task]
    public void DropShield()
    {
        if(Task.current.isStarting && shield.ShieldState == ShieldState.Dropped)
        {
            Task.current.Succeed();
            return;
        }
        damageFilter.IsInvulnerable = false;
        shieldTransitioning = true;
        shield.TransitionShield(false, () => 
        {
            shieldObject.SetActive(false);
            shieldTransitioning = false;
        });
        Task.current.Succeed();
    }

    [Task]
    public void CastShield()
    {
        if(Task.current.isStarting && shield.ShieldState == ShieldState.Casted)
        {
            Task.current.Succeed();
            return;
        }
        damageFilter.IsInvulnerable = true;
        shieldTransitioning = true;
        shieldObject.SetActive(true);
        shield.TransitionShield(true, () => 
        {
            shieldTransitioning = false;
        });
        Task.current.Succeed();
    }

    [Task]
    public void WaitShield()
    {
        if (!shieldTransitioning)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void Shoot()
    {
        var barrelPos = barrelPosDict[attackDirection.Direction];
        barrelPos.x = movement.FacingRight ? barrelPos.x : -barrelPos.x;

        var bullets = stats.NumBullets;
        var centerAngle = GetAngle(attackDirection.Direction, movement.FacingRight);
        var deltaAngle = stats.SpreadAngle / (float)bullets;
        var startAngle = centerAngle - stats.SpreadAngle * 0.5f;

        for(int i = 0; i < bullets; i++)
        {
            var minAngle = startAngle + i * deltaAngle; 
            var maxAngle = minAngle + deltaAngle; 
            var angle = Random.Range(minAngle, maxAngle); 
            var obj = Instantiate(bulletPrefab);
            obj.transform.position = transform.position + barrelPos;
            var bullet = obj.GetComponent<Bullet>();
            bullet.Speed = Random.Range(stats.MinBulletSpeed, stats.MaxBulletSpeed);
            bullet.RotateTransform = false;
            bullet.Shoot(angle * Mathf.Deg2Rad);
        }
        var muzzleFlashObj = Instantiate(muzzleFlashPrefab);
        var muzzleFlash = muzzleFlashObj.GetComponent<MuzzleFlash>();
        muzzleFlash.InitializeFlash(transform.position + barrelPos, centerAngle, false, 0);
        Task.current.Succeed();
    }

    private float GetAngle(Direction dir, bool facingRight)
    {
        switch (dir)
        {
            case Direction.Back:
                return 90;
            case Direction.Back45:
                return facingRight ? 45 : 135;
            case Direction.Side:
                return facingRight ? 0 : 180;
            case Direction.Front45:
                return facingRight ? -45 : -135;
            case Direction.Front:
                return -90;
        }
        return 0;
    }
}

public enum Direction
{
    Front,
    Front45,
    Side,
    Back45,
    Back
}


[System.Serializable]
public class BarrelPosition
{
    public Direction Direction;
    public Transform Position;
}
