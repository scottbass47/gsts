using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelScript : MonoBehaviour
{
    [SerializeField] private bool showSpawns;
    [SerializeField] private Vector3[] portalSpawns;
    public Vector3[] PortalSpawns => portalSpawns;

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

    public LevelData LevelData { get; set; }

    [SerializeField] private GameObject portalPrefab;
    private List<GameObject> portals;
    public List<GameObject> Portals => portals;

    [SerializeField] private CompositeCollider2D levelBoundary;
    public CompositeCollider2D LevelBoundary => levelBoundary;

    private void Awake()
    {
        portals = new List<GameObject>();
    }

    private void Start()
    {
        foreach(var spawn in PortalSpawns)
        {
            var portal = Instantiate(portalPrefab, transform, false);
            portal.transform.localPosition = spawn;
            portal.SetActive(false);
            portals.Add(portal);
        }
    }

    public void OpenPortals()
    {
        foreach (var portal in portals) portal.SetActive(true);
    }

    public void ClosePortals()
    {
        foreach (var portal in portals) portal.SetActive(false);
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
        FixShadows();
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

    public void FixShadows()
    {
        for(int x = floorDecor.cellBounds.xMin; x < floorDecor.cellBounds.xMax; x++)
        {
            for(int y = floorDecor.cellBounds.yMin; y < floorDecor.cellBounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                if (wallDecor.GetTile(pos) != null) continue;

                var wall = wallDecor.GetTile(pos + new Vector3Int(0,1,0));
                var floor = floorDecor.GetTile(new Vector3Int(x, y, 0));
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
        if (!showSpawns) return;
        foreach(var spawn in portalSpawns)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position + spawn, 0.5f);
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
