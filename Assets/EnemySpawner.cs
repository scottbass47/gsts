using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPrefab[] prefabs;

    public GameObject Spawn(EnemyType type)
    {
        foreach(var pref in prefabs)
        {
            if (pref.Type == type) return Instantiate(pref.Prefab);
        }
        return null;
    }
}

[System.Serializable]
public class EnemyPrefab
{
    [SerializeField] private EnemyType type;
    [SerializeField] private GameObject prefab;

    public EnemyType Type => type;
    public GameObject Prefab => prefab;
}
