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
    public Tilemap wallCollision;
    public Tilemap projectileCollision;

    [SerializeField] private TileBase wallCollisionTile;
    public TileBase WallCollisionTile => wallCollisionTile;

    [SerializeField] private TileBase projectileCollisionTile;
    public TileBase ProjectileCollisionTile => projectileCollisionTile;

    private void OnDrawGizmos()
    {
        if (!showSpawns) return;
        foreach(var spawn in portalSpawns)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(spawn, 0.5f);
        } 
    }
}
