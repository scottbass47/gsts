using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;
using Panda;

[RequireComponent(typeof(IMovement))]
[RequireComponent(typeof(Death))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Physics))]
[RequireComponent(typeof(BasicTasks))]
[RequireComponent(typeof(PandaBehaviour))]
[RequireComponent(typeof(DamageFilter))]
public class AIController : MonoBehaviour
{
    private Transform target;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    [SerializeField] private Transform pos;

    public Transform Pos
    {
        get => pos;
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

    private bool pendingPath;
    public bool PendingPath
    {
        get => pendingPath;
        set => pendingPath = value;
    }

    private bool pathFailed;
    public bool PathFailed
    {
        get => pathFailed;
        set => pathFailed = value;
    }

    private Coroutine flash;
    private bool flashing = false;

    [SerializeField] private DeathBehavior onDeath;
    [SerializeField] private GameObject deathPrefab;
    [SerializeField] private bool drawPath = false;

    private void Start()
    {
        var death = GetComponent<Death>();
        death.DestroyOnDeath = false;
        death.OnDeath += () =>
        {
            var deathObj = Instantiate(deathPrefab);
            var enemyDeath = deathObj.GetComponent<EnemyDeath>();
            enemyDeath.Initialize(gameObject);
            SoundManager.PlaySound(Sounds.EnemyFleshHitFatal);
            Destroy(gameObject);
            //onDeath.OnDeath(death);
        };
    }

    public void Flash(float duration)
    {
        if (flashing) return;
        flashing = true;
        StartCoroutine(DrawUtils.Flash(gameObject, duration, flashFinished));
    }

    private void flashFinished()
    {
        flashing = false;
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
