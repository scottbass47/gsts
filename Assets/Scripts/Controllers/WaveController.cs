using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;

    private int currentWave;
    public int CurrentWave => currentWave;

    private List<GameObject> portals => levelScript.PortalObjects;
    private int alive;
    private EventManager events;
    private GameObject door;
    private WaveConfig waveConfig;
    private LevelScript levelScript;

    private void Start()
    {
        events = GameManager.Instance.Events;

        events.AddListener<WaveStarted>(this.gameObject, OnWaveStart);
    }

    public void SetWaveConfig(WaveConfig config)
    {
        waveConfig = config;
    }

    public void SetLevel(LevelScript levelScript)
    {
        this.levelScript = levelScript;
    }

    public void StartNextWave()
    {
        events.FireEvent(new WaveStarted { WaveNum = currentWave + 1 });
    }

    private void OnWaveStart(WaveStarted waveData)
    {
        StartCoroutine(SpawnWave(waveConfig.Waves[currentWave]));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        foreach(var subWave in wave.SubWaves)
        {
            yield return StartCoroutine(SpawnSubWave(subWave));
        }
        events.FireEvent(new WaveEnded());
        currentWave++;
    }

    private IEnumerator SpawnSubWave(SubWave subWave)
    {
        alive = 0;
        int totalEnemies = 0;
        foreach(var d in subWave.EnemyData)
        {
            totalEnemies += d.Amount;
        }

        bool portalsOpen = false;
        levelScript.OpenPortals(() => 
        {
            portalsOpen = true;
        });

        yield return new WaitUntil(() => portalsOpen);

        events.FireEvent(new WaveEnemyChange { EnemiesLeft = totalEnemies });

        int i = 0;
        while(i < totalEnemies)
        {
            yield return StartCoroutine(SpawnBatch(subWave.EnemyData, i, totalEnemies));
            i += portals.Count;
        }

        levelScript.ClosePortals();

        while (alive > 0) yield return null;
    }

    private IEnumerator SpawnBatch(EnemyData[] data, int index, int totalEnemies)
    {
        for(int i = 0; index < totalEnemies && i < portals.Count; i++, index++)
        {
            var type = GetEnemyType(data, index);
            var enemy = enemySpawner.Spawn(type);
            enemy.transform.position = portals[i].transform.position;
            alive++;
            enemy.GetComponent<Death>().OnDeath += () =>
            {
                alive--;
                events.FireEvent(new WaveEnemyChange { EnemiesLeft = alive });
            };
        }
        yield return new WaitForSeconds(1f);
    }

    private EnemyType GetEnemyType(EnemyData[] data, int index)
    {
        int i = 0;
        while(index > data[i].Amount)
        {
            index -= data[i].Amount;
            i++;
        }
        return data[i].EnemyType;
    }
    
}
