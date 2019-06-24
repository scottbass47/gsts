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

    public Level Level => GameManager.Instance.level;

    private Movement movement;
    public Movement Movement => movement;

    private Path path;
    public Path Path
    {
        get => path;
        set => path = value;
    } 

    private PathFinder pathFinder;
    public PathFinder PathFinder => pathFinder;

    private Vector2 moveDir;
    public Vector2 MoveDir
    {
        get => moveDir;
        set => moveDir = value.normalized; 
    }

    private Coroutine flash;
    private bool flashing = false;

    [SerializeField] private DeathBehavior onDeath;
    
    private void Start()
    {
        movement = GetComponent<Movement>();
        pathFinder = Level.GetPathFinder();

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
}
