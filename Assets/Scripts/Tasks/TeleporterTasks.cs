using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterTasks : MonoBehaviour
{
    private AI ai;

    [Task]
    private bool isTeleporting;
    public bool IsTeleporting => isTeleporting;

    private GameState gameState;
    private LevelScript levelScript;

    private TeleporterStats tStats => (TeleporterStats)ai.EnemyStats;
    private List<GameObject> enemiesToTeleport;

    public void Awake()
    {
        enemiesToTeleport = new List<GameObject>();
    }

    public void Start()
    {
        ai = GetComponent<AI>();
        gameState = GameManager.Instance.GameState;
        levelScript = GameManager.Instance.LevelManager.LevelScript;
    }

    [Task]
    public void PickEnemies()
    {
        var validEnemies = gameState.Enemies.FindAll((enemy) =>
        {
            return enemy.GetComponent<AI>().EnemyType != EnemyType.Teleporter;
        });

        if(validEnemies.Count < tStats.EnemiesToTelport.Min)
        {
            Task.current.Fail();
            return;
        }

        var numEnemies = Random.Range(tStats.EnemiesToTelport.Min, Mathf.Min(tStats.EnemiesToTelport.Max, validEnemies.Count));
        enemiesToTeleport.Clear();

        for (int i = 0; i < numEnemies; i++)
        {
            int index = Random.Range(0, validEnemies.Count);
            enemiesToTeleport.Add(validEnemies[index]);
            validEnemies.RemoveAt(index);
        }

        Task.current.Succeed();
    }

    [Task]
    public void TeleportEnemies()
    {
        int maxTries = 1000;

        foreach(var enemy in enemiesToTeleport)
        {
            if (enemy == null) continue;

            var worldBounds = levelScript.Grid.WorldGridBounds;
            var ai = enemy.GetComponent<AI>();

            int tries = 0;
            while(tries < maxTries)
            {
                float x = Random.Range(worldBounds.min.x, worldBounds.max.x);
                float y = Random.Range(worldBounds.min.y, worldBounds.max.y);

                Vector3 pos = new Vector3(x, y);

                if (levelScript.IsValid(pos, ai.FeetHitbox.radius) && 
                    (pos - ai.Pos.position).magnitude > tStats.MinTeleportDistance)
                {
                    ai.transform.position = pos - ai.Pos.localPosition;
                    break;
                }
                tries++;
            }
        }
        Task.current.Succeed();
    }

    [Task]
    public void StartTeleporting()
    {
        isTeleporting = true;
        Task.current.Succeed();
    }

    [Task]
    public void EndTeleporting()
    {
        isTeleporting = false;
        Task.current.Succeed();
    }
}
