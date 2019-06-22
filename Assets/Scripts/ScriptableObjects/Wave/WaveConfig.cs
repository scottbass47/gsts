using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

[CreateAssetMenu(menuName = "Game/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] private List<Wave> waves;

    public List<Wave> Waves => waves;
}

[System.Serializable]
public class Wave 
{
    [SerializeField] private SubWave[] subWaves; 
    public SubWave[] SubWaves => subWaves; 
}

[System.Serializable]
public class SubWave
{
    [SerializeField] private EnemyData[] data;
    public EnemyData[] EnemyData => data; 
}

[System.Serializable]
public class EnemyData
{
    [SerializeField] private EnemyType type;
    [SerializeField] private int amount;

    public EnemyType EnemyType => type;
    public int Amount => amount;
}
