using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPrefab[] prefabs;

    private Dictionary<EnemyType, GameObject> prefabTable;

    private void Awake()
    {
        prefabTable = new Dictionary<EnemyType, GameObject>();
        foreach(var pref in prefabs)
        {
            prefabTable.Add(pref.Type, pref.Prefab);
        }
    }

    public GameObject Spawn(EnemyType type)
    {
        var obj = Instantiate(prefabTable[type]);
        obj.GetComponent<AI>().Target = GameManager.Instance.Player.GetComponent<Body>().CenterFeet;
        return obj;
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
