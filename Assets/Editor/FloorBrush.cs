using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


[CustomGridBrush(false, true, false, "Floor Brush")]
[CreateAssetMenu(fileName = "Floor Brush", menuName = "Brushes/Floor Brush")]
public class FloorBrush : UnityEditor.Tilemaps.GridBrush
{
    [SerializeField] private WeightedTile[] floorTiles;

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget == null) return;

        var tilemap = brushTarget.GetComponent<Tilemap>();
        if (tilemap == null) return;

        var tile = floorTiles[UnityEngine.Random.Range(0, floorTiles.Length)].Tile;

        tilemap.SetTile(position, tile);
    }
    public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
    {
        for (int x = position.xMin; x < position.xMax; x++)
        {
            for (int y = position.yMin; y < position.yMax; y++)
            {
                Paint(gridLayout, brushTarget, new Vector3Int(x, y, position.z));
            }
        }
    }
    

    [System.Serializable]
    public class WeightedTile
    {
        [SerializeField] private float weight;
        [SerializeField] private TileBase tile;

        public float Weight => weight;
        public TileBase Tile => tile;
    }
}

