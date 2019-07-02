using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Cinemachine;
using System.Text;

public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;

    // Prefabs and References
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private WaveController waveController;
    [SerializeField] private WaveConfig waveConfig;

    public LevelManager LevelManager => levelManager;
    public LevelData level => levelManager.LevelData;
    public EventManager Events { get; private set; }

    private GameObject player;
    public GameObject Player => player;

    [SerializeField] private bool enemySpawnDebug;
    public bool EnemySpawnDebug => enemySpawnDebug;

    [SerializeField] private CinemachineVirtualCamera vcam;
    public CinemachineVirtualCamera Vcam => vcam;

	// Use this for initialization
	void Awake () {
		if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        Events = GetComponent<EventManager>();

        DontDestroyOnLoad(gameObject);
	}

    public void Start()
    {
        levelManager.SetupLevel();

        waveController.SetWaveConfig(waveConfig);
        waveController.SetLevel(levelManager.LevelScript);

        var door = levelManager.LevelScript.Door;
        door.GetComponent<DoorController>().OnDoorInteract += OnDoorOpen;

        player = Instantiate(playerPrefab, new Vector3(10, 0, 0), Quaternion.identity);
        if(EnemySpawnDebug) Instantiate(enemyPrefab, new Vector3(10, -10, 0), Quaternion.identity);
        Events.FireEvent(new PlayerSpawn { Player = player });

        Events.AddListener<LevelChange>(this.gameObject, OnLevelChange);
    }

    private void OnDoorOpen()
    {
        // If we're not on the last level yet, and we're on a level changing wave, then change levels.
        if (levelManager.LevelIndex < waveConfig.LevelChanges.Length && 
            waveController.CurrentWave == waveConfig.LevelChanges[levelManager.LevelIndex]
        )
        {
            levelManager.NextLevel();
        }
        else
        {
            waveController.StartNextWave();
        }
    }

    private void OnLevelChange(LevelChange change)
    {
        var door = levelManager.LevelScript.Door;
        door.GetComponent<DoorController>().OnDoorInteract += OnDoorOpen;

        waveController.SetLevel(levelManager.LevelScript);
    }
}
