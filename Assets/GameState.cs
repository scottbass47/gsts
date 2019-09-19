using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keeps track of game object references
public class GameState : MonoBehaviour
{
    private GameObject player;
    public GameObject Player => player;

    private List<GameObject> enemies;
    public List<GameObject> Enemies => enemies;

    private void Awake()
    {
        enemies = new List<GameObject>();
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
        enemy.GetComponent<Death>().OnDeath += () =>
        {
            enemies.Remove(enemy);
        };
    }
}
