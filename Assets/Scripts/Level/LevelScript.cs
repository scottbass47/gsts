using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class LevelScript : MonoBehaviour
{
    [SerializeField] private bool showSpawns;
    [SerializeField] private bool createWallSortingGroups = true;

    [SerializeField] private GameObject door;
    public GameObject Door => door;

    [HideInInspector] public TileBase[] floorTiles;
    [HideInInspector] public TileBase[] shadowTiles;

    public Tilemap floorDecor;
    public Tilemap wallDecor;
    public Tilemap computers;
    public Tilemap wallCollision;
    public Tilemap projectileCollision;

    [SerializeField] private TileBase wallCollisionTile;
    public TileBase WallCollisionTile => wallCollisionTile;

    [SerializeField] private TileBase projectileCollisionTile;
    public TileBase ProjectileCollisionTile => projectileCollisionTile;

    [SerializeField] private Transform levelEnter;
    [SerializeField] private Transform levelExit;

    public Transform LevelEnter => levelEnter;
    public Transform LevelExit => levelExit;

    public GameObject levelBranchPrefab;

    //public LevelData LevelData { get; set; }

    [SerializeField] private GameObject portalPrefab;
    private List<GameObject> portalObjects;
    public List<GameObject> PortalObjects => portalObjects;

    [SerializeField] private CompositeCollider2D levelBoundary;
    public CompositeCollider2D LevelBoundary => levelBoundary;

    public PathRequestManager PathRequestManager { get; private set; }
    public LevelGrid Grid { get; private set; }

    [SerializeField] private int portalCount = 3;
    private float maxPortalDist;
    private float portalDist
    {
        get
        {
            return maxPortalDist * 0.75f / (portalLocations.Count + 1);
        }
    }
    private List<Vector3> potentialPortalSpawnLocations;
    private List<Vector3> portalLocations;

    private void Awake()
    {
        portalObjects = new List<GameObject>();
        PathRequestManager = GetComponent<PathRequestManager>();
        Grid = GetComponent<LevelGrid>();
        potentialPortalSpawnLocations = new List<Vector3>();
        portalLocations = new List<Vector3>();
        if(createWallSortingGroups) CreateWallSortingGroups();
    }

    private void Start()
    {
        for (int i = 0; i < portalCount; i++)
        {
            var portal = Instantiate(portalPrefab, transform, false);
            portal.SetActive(false);
            portalObjects.Add(portal);
        }
        StartCoroutine(DelayPortalSetup());
        //SetupPotentialPortalLocations();
        //StartCoroutine(SpawnPortals());
    }

    private Tilemap CreateTilemap(Transform parent, string name, Vector3 origin)
    {
        var sortingGroupObj = new GameObject($"Sorting{name}");
        var sortingGroup = sortingGroupObj.AddComponent<SortingGroup>();

        var tilemapObj = new GameObject(name);
        var tilemap = tilemapObj.AddComponent<Tilemap>();
        tilemap.tileAnchor = new Vector3(0.5f, 0, 0);
        tilemapObj.AddComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Chunk;

        sortingGroupObj.transform.parent = parent;
        sortingGroupObj.transform.localPosition = origin;
        tilemapObj.transform.parent = sortingGroupObj.transform;
        tilemapObj.transform.localPosition = Vector3.zero;// -origin;
        return tilemap;
    }

    private Vector3Int GetTilemapBottomLeft(Tilemap tilemap)
    {
        var parentPos = tilemap.transform.parent.localPosition;
        return new Vector3Int(Mathf.FloorToInt(parentPos.x), Mathf.FloorToInt(parentPos.y), 0);
    }

    private void CreateWallSortingGroups()
    {
        var parent = wallDecor.transform.parent;
        wallDecor.CompressBounds();
        var bounds = wallDecor.cellBounds;
        var outerWalls = CreateTilemap(parent, "OuterWalls", bounds.min);

        Tilemap currTilemap = null;
        Tilemap highestTilemap = null;
        var lastPoint = Vector3Int.zero;
        bool started = false;
        int wallNum = 0;

        var tilemaps = new List<Tilemap>();
    
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            started = false;
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            { 
                var pos = new Vector3Int(x, y, 0);

                // On the outside
                if (pos.x == bounds.xMin || pos.x == bounds.xMax - 1 || pos.y == bounds.yMin || pos.y == bounds.yMax - 1)
                {
                    outerWalls.SetTile(pos - GetTilemapBottomLeft(outerWalls), wallDecor.GetTile(pos));
                    continue;
                }

                var tile = wallDecor.GetTile(pos);
                if (tile == null)
                {
                    started = false;
                    continue;
                }

                if (!started)
                {
                    started = true;
                    lastPoint = pos;
                    bool found = false;
                    foreach(var tilemap in tilemaps)
                    {
                        var bottomRight = new Vector3Int(tilemap.cellBounds.xMax, tilemap.cellBounds.yMin, 0);
                        if(bottomRight + GetTilemapBottomLeft(tilemap) == pos)
                        {
                            currTilemap = tilemap;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        currTilemap = CreateTilemap(parent, $"Wall{wallNum++}", pos);
                        currTilemap.origin = Vector3Int.zero;
                        highestTilemap = (highestTilemap == null || highestTilemap.origin.y < pos.y) ? currTilemap : highestTilemap;
                        tilemaps.Add(currTilemap);
                    }
                }

                if(pos.y - lastPoint.y <= 1)
                {
                    currTilemap.SetTile(pos - GetTilemapBottomLeft(currTilemap), tile);
                    lastPoint = pos;
                }
                else
                {
                    started = false;
                }
            }
        }
        var backwallParent = highestTilemap.transform.parent;
        computers.transform.parent = backwallParent;
        door.transform.parent = backwallParent;
        wallDecor.gameObject.SetActive(false);
    }

    private List<Vector3> failedPortalLocations = new List<Vector3>();

    private IEnumerator DelayPortalSetup()
    {
        yield return null;
        SetupPotentialPortalLocations();
    }

    private void SetupPotentialPortalLocations()
    {
        for (int x = floorDecor.cellBounds.xMin; x < floorDecor.cellBounds.xMax; x++)
        {
            for (int y = floorDecor.cellBounds.yMin; y < floorDecor.cellBounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                var localPosition = floorDecor.CellToLocal(pos) + new Vector3(0.5f, 0.5f, 0);

                if (floorDecor.GetTile(pos) == null || wallDecor.GetTile(pos) != null) continue;

                var worldPosition = floorDecor.LocalToWorld(localPosition);
                var result = Physics2D.OverlapCircle(worldPosition, 1f, LayerMask.GetMask("Wall"));

                if(result != null)
                {
                    failedPortalLocations.Add(worldPosition);
                }
                else
                {
                    potentialPortalSpawnLocations.Add(localPosition);
                }
                //if (result == null)
                //{
                //    potentialPortalSpawnLocations.Add(localPosition);
                //}
            }
        }

        // The max distance between portals is roughly the distance between the first and last portal
        // since we're traversing the nodes from corner to corner
        maxPortalDist = (potentialPortalSpawnLocations[0] - potentialPortalSpawnLocations[potentialPortalSpawnLocations.Count - 1]).magnitude;
    }

    public bool IsValid(Vector3 worldPosition, float radius)
    {
        if (!Grid.NodeFromWorldPoint(worldPosition).Walkable) return false;
        return Physics2D.OverlapCircle(worldPosition, radius, LayerMask.GetMask("Wall"));
    }

    public void GetPortalLocations()
    {
        portalLocations.Clear();
        var potentialSpawns = new List<Vector3>(potentialPortalSpawnLocations);
        while(portalLocations.Count < portalCount)
        {
            var index = Random.Range(0, potentialSpawns.Count);
            var spawn = potentialSpawns[index];
            potentialSpawns.RemoveAt(index);

            bool valid = true;
            foreach(var s in portalLocations)
            {
                var sqrDist = (s - spawn).sqrMagnitude;
                if(sqrDist < portalDist * portalDist)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                portalLocations.Add(spawn);
            }
        }
    }

    // Debug method
    private IEnumerator SpawnPortals()
    {
        while (true)
        {
            bool opened = false;
            OpenPortals(() => { opened = true; });
            yield return new WaitUntil(() => opened);
            ClosePortals();
            yield return new WaitForSeconds(1.5f);
        }
    }

    public void OpenPortals(Action portalsOpenedCallback)
    {
        GetPortalLocations();
        StartCoroutine(OpenPortalsRoutine(portalsOpenedCallback));
    }

    private IEnumerator OpenPortalsRoutine(Action callback)
    {
        for (int i = 0; i < portalCount; i++)
        {
            var portalObj = portalObjects[i];
            portalObj.transform.localPosition = portalLocations[i];
            portalObj.SetActive(true);

            // Only care about the callback of the last portal to be opened
            // because that determines when all portals are ready to spawn enemies
            portalObj.GetComponent<Portal>().Open(i == portalCount - 1 ? callback : null);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ClosePortals()
    {
        foreach (var portal in portalObjects)
        {
            portal.GetComponent<Portal>().Close(() =>
            {
                portal.SetActive(false);
            });
        }
    }

    public void SetupLevelChange()
    {
        var tilemaps = GetComponentsInChildren<Tilemap>();
        var doorPos = door.transform.position;
        for (float x = doorPos.x - 1; x <= doorPos.x + 1; x++)
        {
            for (float y = doorPos.y; y <= doorPos.y + 2; y++)
            {
                var pos = new Vector3(x, y, 0);
                foreach (var tilemap in tilemaps) ClearTile(pos, tilemap);
            }
        }

        // Upper corners
        foreach (var tilemap in tilemaps)
        {
            var c1 = doorPos + new Vector3(2, 2, 0);
            var c2 = doorPos + new Vector3(-2, 2, 0);
            ClearTile(c1, tilemap);
            ClearTile(c2, tilemap);
        }

        door.SetActive(false);

        // Toggle shadows under door
        var xMin = (int)(doorPos.x - 1);
        var yMin = (int)(doorPos.y - 1);
        var shadowBounds = new BoundsInt(xMin, yMin, 0, 3, 1, 1);
        ToggleShadows(shadowBounds);
    }

    private TileGroup bottomTiles;

    public void OpenBottom()
    {
        var levelEnterPos = levelEnter.transform.position - new Vector3(0, 0.5f, 0);
        bottomTiles = new TileGroup(GetComponentsInChildren<Tilemap>());
        bottomTiles.SetBounds(
            (int)(levelEnterPos.x - 2),
            (int)levelEnterPos.y,
            (int)(levelEnterPos.x + 2), 
            (int)levelEnterPos.y
        );
        bottomTiles.Clear();
    }

    public void CloseBottom()
    {
        bottomTiles.Reset();
    }

    private void ClearTile(Vector3 pos, Tilemap tilemap)
    {
        tilemap.SetTile(tilemap.WorldToCell(pos), null);
    }

    private void ToggleShadow(Vector3Int worldPos)
    {
        var cellPos = floorDecor.WorldToCell(worldPos);
        var floor = floorDecor.GetTile(cellPos);
        for (int i = 0; i < shadowTiles.Length; i++)
        {
            var shadowTile = shadowTiles[i];
            var floorTile = floorTiles[i];
            if (shadowTile == floor)
            {
                floorDecor.SetTile(cellPos, floorTiles[i]);
                break;
            }
            else if (floorTile == floor)
            {
                floorDecor.SetTile(cellPos, shadowTiles[i]);
                break;
            }
        }
    }

    private void ToggleShadows(BoundsInt bounds)
    {
        foreach(var pos in bounds.allPositionsWithin)
        {
            ToggleShadow(pos);
        }
    }

    public void FixShadows()
    {
        for(int x = floorDecor.cellBounds.xMin; x < floorDecor.cellBounds.xMax; x++)
        {
            for(int y = floorDecor.cellBounds.yMin; y < floorDecor.cellBounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                if (wallDecor.GetTile(pos) != null) continue;

                var wall = wallDecor.GetTile(pos + Vector3Int.up);
                var floor = floorDecor.GetTile(pos);
                if(wall == null)
                {
                    for (int i = 0; i < shadowTiles.Length; i++)
                    {
                        var shadowTile = shadowTiles[i];
                        if (shadowTile == floor) floorDecor.SetTile(pos, floorTiles[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < floorTiles.Length; i++)
                    {
                        var floorTile = floorTiles[i];
                        if (floorTile == floor) floorDecor.SetTile(pos, shadowTiles[i]);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showSpawns || potentialPortalSpawnLocations == null) return;
        //foreach(var spawn in portalSpawns)
        //{
        //    Gizmos.color = Color.cyan;
        //    Gizmos.DrawSphere(transform.position + spawn, 0.5f);
        //} 
        foreach (var spawn in potentialPortalSpawnLocations)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position + spawn, 0.3f);
        }
        foreach (var location in failedPortalLocations)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(location, 1f);
        }
        foreach (var location in portalLocations)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + location, 1f);
        }
    }
}

public class TileGroup
{
    private int xMin;
    private int yMin;
    private int xMax;
    private int yMax;

    private Tilemap[] tilemaps;
    private TileBase[, ,] tiles;

    public TileGroup(Tilemap[] tilemaps)
    {
        this.tilemaps = tilemaps;
    }

    public void SetBounds(int xMin, int yMin, int xMax, int yMax)
    {
        if (xMin > xMax || yMin > yMax)
        {
            Debug.Log("Invalid bounds.");
            return;
        }
        this.xMin = xMin;
        this.yMin = yMin;
        this.xMax = xMax;
        this.yMax = yMax;

        tiles = new TileBase[tilemaps.Length, xMax - xMin + 1, yMax - yMin + 1];
    }

    public void Clear()
    {
        for(int x = xMin; x <= xMax; x++)
        {
            for(int y = yMin; y <= yMax; y++)
            {
                var pos = new Vector3(x, y, 0);
                for(int i = 0; i < tilemaps.Length; i++)
                {
                    tiles[i, x - xMin, y - yMin] = ClearTile(pos, tilemaps[i]);
                }
            }
        }
    }

    public void Reset()
    {
        for(int x = xMin; x <= xMax; x++)
        {
            for(int y = yMin; y <= yMax; y++)
            {
                var pos = new Vector3(x, y, 0);
                for(int i = 0; i < tilemaps.Length; i++)
                {
                    SetTile(pos, tilemaps[i], tiles[i, x - xMin, y - yMin]);
                }
            }
        }
    }

    private void SetTile(Vector3 pos, Tilemap tilemap, TileBase tile)
    {
        var cell = tilemap.WorldToCell(pos);
        tilemap.SetTile(cell, tile);
    }

    private TileBase ClearTile(Vector3 pos, Tilemap tilemap)
    {
        var cell = tilemap.WorldToCell(pos);
        var old = tilemap.GetTile(cell);
        tilemap.SetTile(cell, null);
        return old;
    }
}
