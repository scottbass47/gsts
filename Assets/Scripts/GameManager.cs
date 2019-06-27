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
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    private GameObject chest;
    private List<GameObject> enemies;

    public Level level { get; private set; }
    public EventManager Events { get; private set; }

    private GameObject player;
    public GameObject Player => player;

    [SerializeField] private bool enemySpawnDebug;
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

        map = Instantiate(map, Vector3.zero, Quaternion.identity) as GameObject;
        map.SetActive(false);

        enemies = new List<GameObject>();

        DontDestroyOnLoad(gameObject);

        SetupLevel();
	}

    public void Start()
    {
        player = Instantiate(playerPrefab, new Vector3(10, 0, 0), Quaternion.identity);
        if(EnemySpawnDebug) Instantiate(enemyPrefab, new Vector3(10, -10, 0), Quaternion.identity);
        Events.FireEvent(new PlayerSpawn { Player = player });
    }

    public void SetupLevel()
    {
        Tilemap walls = null;

        foreach(var tm in map.GetComponentsInChildren<Tilemap>())
        {
            if (tm.tag == "Wall") walls = tm;
        }
        walls.CompressBounds();

        // Use floor dimensions to determine size of map
        Level level = new Level();
        level.Width = walls.size.x;
        level.Height = walls.size.y;
        level.map = new bool[level.Width, level.Height];
        level.origin = new Vector2(walls.cellBounds.xMin, walls.cellBounds.yMin);

        int colRef = walls.cellBounds.xMin;
        int rowRef = walls.cellBounds.yMin;
        for(int row = 0; row < level.Height; row++)
        {
            for(int col = 0; col < level.Width; col++)
            {
                if (walls.GetTile(new Vector3Int(col + colRef, row + rowRef, 0)) != null)
                {
                    level.map[col, row] = true;
                }
            }
        }
        this.level = level;

        //var sb = new StringBuilder();
        //for(int row = level.Height - 1; row >= 0; row--)
        //{
        //    for(int col = 0; col < level.Width; col++)
        //    {
        //        if (level.map[row,col])
        //        {
        //            sb.Append("*");
        //        }
        //        else
        //        {
        //            sb.Append("-");
        //        }
        //    }
        //    sb.Append("\n");
        //}
        //Debug.Log(sb);
    }

    //private void OnDrawGizmos()
    //{
    //    if (level == null) return;
    //    for (int row = level.Height - 1; row >= 0; row--)
    //    {
    //        for (int col = 0; col < level.Width; col++)
    //        {
    //            if (level.map[col, row])
    //            {
    //                Gizmos.color = Color.black;
    //                var vec = level.TileToWorld(new Coord(col, row));
    //                Gizmos.DrawCube(vec + new Vector2(0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f));
    //            }
    //        }
    //    }

    //}

    //public void RestartLevel()
    //{
    //    GameObject fade = Instantiate(screenFade, Vector3.zero, Quaternion.identity) as GameObject;
    //    fade.GetComponent<ScreenFadeController>().SetTime(2);

    //    if (chest != null)
    //    {
    //        Destroy(chest);
    //    }

    //    // Get rid of old enemies (could be more efficient)
    //    foreach(GameObject enemy in enemies)
    //    {
    //        if(enemy != null) Destroy(enemy);
    //    }
    //    enemies.Clear();

    //    levelGen.GenerateMap();
    //    Level level = levelGen.Level;
    //    this.level = level;

    //    map.SetActive(true);

    //    Tilemap floor = map.transform.Find("Floor").GetComponent<Tilemap>();
    //    Tilemap walls = map.transform.Find("Walls").GetComponent<Tilemap>();

    //    floor.ClearAllTiles();
    //    walls.ClearAllTiles();

    //    Vector3Int size = new Vector3Int(level.Width, level.Height, 1);

    //    floor.size = size;
    //    walls.size = size;

    //    floor.cellBounds.SetMinMax(Vector3Int.zero, new Vector3Int(level.Width, level.Height, 1));
    //    walls.cellBounds.SetMinMax(Vector3Int.zero, new Vector3Int(level.Width, level.Height, 1));

    //    for(int x = 0; x < level.Width; x++)
    //    {
    //        for(int y = 0; y < level.Height; y++)
    //        {
    //            if(level.map[x, y])
    //            {
    //                if(level.IsTopWall(x, y))
    //                {
    //                    floor.SetTile(new Vector3Int(x, y, 0), wallTile);
    //                }
    //                else
    //                {
    //                    walls.SetTile(new Vector3Int(x, y, 0), wallTile);
    //                }
    //            }
    //            else
    //            {
    //                floor.SetTile(new Vector3Int(x, y, 0), floorTile);
    //            }
    //        }
    //    }

    //    player.transform.position = new Vector3(level.playerSpawn.x, level.playerSpawn.y, 0);

    //    levelExit.SetActive(true);
    //    levelExit.transform.position = new Vector3(level.exit.x, level.exit.y, 0);

    //    chest = Instantiate(chestPrefab, Vector3.zero, Quaternion.identity) as GameObject;
    //    chest.transform.position = new Vector3(level.chest.x, level.chest.y, 0);

    //    Vector2 center = new Vector2(0.5f, 0.5f);
    //    foreach(Vector2 enemySpawn in level.enemySpawns)
    //    {
    //        GameObject e = Instantiate(enemy, enemySpawn + center, Quaternion.identity) as GameObject;
    //        EnemyController controller = e.GetComponent<EnemyController>();
    //        controller.Init();
    //        controller.player = player;
    //        e.transform.position = e.transform.position + (e.transform.position - new Vector3(controller.HitboxCenter.x, controller.HitboxCenter.y, 0));
    //        enemies.Add(e);
    //    }

    //    PolygonCollider2D collider = map.AddComponent<PolygonCollider2D>();
    //    collider.SetPath(0, new Vector2[]
    //    {
    //        new Vector2(0, 0),
    //        new Vector2(0, level.Height),
    //        new Vector2(level.Width, level.Height),
    //        new Vector2(level.Width, 0)
    //    });
    //    collider.isTrigger = true;

    //    vcam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = collider;
    //}

    // Update is called once per frame
    //void Update () {

    //}
}
