using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


[CustomGridBrush(false, true, false, "Inner Wall Brush")]
[CreateAssetMenu(fileName = "Inner Wall Brush", menuName = "Brushes/Inner Wall Brush")]
public class InnerWallsBrush : UnityEditor.Tilemaps.GridBrush
{
    [SerializeField] private TileBase tileWalls;
    [SerializeField] private TileBase tileCeiling;

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget == null) return;

        var tilemap = brushTarget.GetComponent<Tilemap>();
        if (tilemap == null) return;

        var twoDown = new Vector3Int(position.x, position.y - 2, position.z);
        var twoUp = new Vector3Int(position.x, position.y + 2, position.z);
        var tile = tileWalls;

        if(tilemap.GetTile(twoDown) != null)
        {
            tile = tileCeiling; 
        }
        if(tilemap.GetTile(twoUp) != null && tilemap.GetTile(twoUp) == tileWalls)
        {
            tilemap.SetTile(twoUp, tileCeiling);
        }

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
}

