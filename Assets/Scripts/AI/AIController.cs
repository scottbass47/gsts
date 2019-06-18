﻿using System.Collections;
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

    public Level Level => GameManager.instance.level;

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
    
    private void Start()
    {
        movement = GetComponent<Movement>();
        pathFinder = Level.GetPathFinder();
    }

    private void Update()
    {
       Tree?.Tick(new TimeData(Time.deltaTime)); 
    }
}