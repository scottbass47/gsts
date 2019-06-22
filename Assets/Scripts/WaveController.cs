using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    private int currentWave;
    [SerializeField] private WaveConfig waveConfig;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private LevelScript levelScript;
    [SerializeField] private EnemySpawner enemySpawner;

    private List<GameObject> portals;
    private int alive;
    private EventManager events;
    private GameObject door;

    private void Start()
    {
        events = GameManager.Instance.Events;

        events.AddListener<WaveStarted>(OnWaveStart);

        portals = new List<GameObject>();
        foreach(var spawn in levelScript.PortalSpawns)
        {
            var portal = Instantiate(portalPrefab, spawn, Quaternion.identity);
            portal.SetActive(false);
            portals.Add(portal);
        }

        door = levelScript.Door;
        door.GetComponent<DoorController>().OnDoorInteract += () =>
        {
            events.FireEvent(new WaveStarted{ WaveNum = currentWave + 1 });
        };
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

        foreach (var portal in portals) portal.SetActive(true);

        events.FireEvent(new WaveEnemyChange { EnemiesLeft = totalEnemies });

        int i = 0;
        while(i < totalEnemies)
        {
            yield return StartCoroutine(SpawnBatch(subWave.EnemyData, i, totalEnemies));
            i += portals.Count;
        }

        foreach (var portal in portals) portal.SetActive(false);

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
