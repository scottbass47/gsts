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
    [Header("Prefabs/References")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameState gameState;

    [Header("Wave")]
    [SerializeField] private WaveController waveController;
    [SerializeField] private WaveConfig waveConfig;

    [Header("Debug Enemy")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private bool enemySpawnDebug;

    [Header("Other")]
    [SerializeField] private Settings settings;
    [SerializeField] private CinemachineVirtualCamera vcam;
    public CinemachineVirtualCamera Vcam => vcam;

    public LevelManager LevelManager => levelManager;
    public EventManager Events { get; private set; }
    public GameState GameState => gameState;

    private GameObject player;
    public GameObject Player => player;

    public bool EnemySpawnDebug => enemySpawnDebug;

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
        GameSettings.Settings = settings;

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
        if (EnemySpawnDebug)
        {
            var enemy = enemySpawner.Spawn(enemyType);
            enemy.transform.position = new Vector3(10, -10, 0); 
        }
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
