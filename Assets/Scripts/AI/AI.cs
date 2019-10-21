using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;
using Panda;
using Effects;
using static Panda.BehaviourTree;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Death))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Physics))]
[RequireComponent(typeof(BasicTasks))]
[RequireComponent(typeof(PandaBehaviour))]
[RequireComponent(typeof(DamageFilter))]
public class AI : MonoBehaviour
{
    private Transform target;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    [SerializeField] private EnemyType enemyType;
    public EnemyType EnemyType => enemyType;

    [SerializeField] private EnemyStats enemyStats;
    public EnemyStats EnemyStats => enemyStats;

    [SerializeField] private Transform pos;
    public Transform Pos
    {
        get => pos;
    }

    [SerializeField] private CircleCollider2D feetHitbox;
    public CircleCollider2D FeetHitbox
    {
        get => feetHitbox;
    }


    //public LevelData Level => GameManager.Instance.level;
    public LevelScript Level => GameManager.Instance.LevelManager.LevelScript;

    //private Physics physics;
    //public Physics Physics => physics;

    private Path path;
    public Path Path
    {
        get => path;
        set => path = value;
    }

    private Coroutine flash;
    private bool flashing = false;

    [SerializeField] private GameObject deathPrefab;
    [SerializeField] private bool drawPath = false;

    private PandaBehaviour pandaBehavior;
    private EffectHandler statusEffectHandler;

    private bool stunned;
    public bool Stunned
    {
        get => stunned;
        set
        {
            stunned = value;
            if (stunned)
            {
                pandaBehavior.enabled = false;
            }
            else
            {
                pandaBehavior.enabled = true;
            }
        }
    }

    private void Awake()
    {
        // Add myself to be tracked
        GameManager.Instance.GameState.AddEnemy(gameObject);
    }

    private void Start()
    {
        GetComponent<Health>().Amount = EnemyStats.Health;

        var death = GetComponent<Death>();
        death.DestroyOnDeath = false;
        death.OnDeath += () =>
        {
            var deathObj = Instantiate(deathPrefab);
            var enemyDeath = deathObj.GetComponent<EnemyDeath>();
            enemyDeath.Initialize(gameObject);
            SoundManager.PlaySound(Sounds.EnemyFleshHitFatal);
            Destroy(gameObject);
        };

        pandaBehavior = GetComponent<PandaBehaviour>();
        statusEffectHandler = GetComponent<EffectHandler>();
    }

    public void Flash(float duration)
    {
        statusEffectHandler.AddEffect(new FlashEffect(duration));
    }

    private void OnDrawGizmos()
    {
        if(path != null && drawPath)
        {
            path.DrawWithGizmos();
            var currPos = transform.position;
            for (int i = path.CurrentWaypointIndex; i < path.Waypoints.Length; i++)
            {
                Debug.DrawLine(currPos, path.Waypoints[i], Color.red);
                currPos = path.Waypoints[i];
            }
        }
    }
}
