using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;

public class AIController : MonoBehaviour
{
    private IBehaviourTreeNode tree;

    public IBehaviourTreeNode Tree
    {
        get => tree;
        set => tree = value;
    }

    private Transform target;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    private Transform feet;

    public Transform Pos
    {
        get => feet;
        set => feet = value;
    }

    //public LevelData Level => GameManager.Instance.level;
    public LevelScript Level => GameManager.Instance.LevelManager.LevelScript;

    private Movement movement;
    public Movement Movement => movement;

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

    //private PathFinder pathFinder;
    //public PathFinder PathFinder => pathFinder;

    private Vector2 moveDir;
    public Vector2 MoveDir
    {
        get => moveDir;
        set => moveDir = value.normalized; 
    }

    private Coroutine flash;
    private bool flashing = false;

    [SerializeField] private DeathBehavior onDeath;
    [SerializeField] private bool drawPath = false;
    
    private void Start()
    {
        movement = GetComponent<Movement>();
        //pathFinder = Level.GetPathFinder();

        var death = GetComponent<Death>();
        death.DestroyOnDeath = false;
        death.OnDeath += () =>
        {
            onDeath.OnDeath(death);
        };
    }

    private void Update()
    {
       Tree?.Tick(new TimeData(Time.deltaTime)); 
    }

    public void Move(float speed)
    {
        movement.AddForce(moveDir * speed);
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
