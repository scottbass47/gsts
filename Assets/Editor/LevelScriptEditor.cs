using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(LevelScript))]
public class LevelScriptEditor : Editor
{
    private LevelScript levelScript => target as LevelScript;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        int count = EditorGUILayout.DelayedIntField("Number of Sprites", levelScript.floorTiles != null ? levelScript.floorTiles.Length : 0);
        if (count < 0)
            count = 0;
        if (levelScript.floorTiles == null || levelScript.floorTiles.Length != count)
        {
            Array.Resize<TileBase>(ref levelScript.floorTiles, count);
            Array.Resize<TileBase>(ref levelScript.shadowTiles, count);
        }

        if (count == 0)
            return;

        EditorGUILayout.LabelField("Add floor tiles and corresponding shadow tiles.");
        EditorGUILayout.Space();

        for (int i = 0; i < count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            levelScript.floorTiles[i] = (TileBase)EditorGUILayout.ObjectField("Tile " + (i + 1), levelScript.floorTiles[i], typeof(TileBase), false, null);
            levelScript.shadowTiles[i] = (TileBase)EditorGUILayout.ObjectField("Shadow " + (i + 1), levelScript.shadowTiles[i], typeof(TileBase), false, null);
            EditorGUILayout.EndHorizontal();
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(levelScript);

        if(GUILayout.Button("Add Shadows"))
        {
            levelScript.FixShadows();
        }

        if(GUILayout.Button("Add Collision"))
        {
            SetupCollision();
        }
    }


    private void SetupCollision()
    {
        var walls = levelScript.wallDecor;
        var floors = levelScript.floorDecor;
        var projectileCollision = levelScript.projectileCollision;
        var wallCollision = levelScript.wallCollision;

        for (int x = walls.cellBounds.xMin; x < walls.cellBounds.xMax; x++)
        {
            for (int y = walls.cellBounds.yMin; y < walls.cellBounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                if (walls.GetTile(pos) == null)
                {
                    wallCollision.SetTile(pos, null);
                    projectileCollision.SetTile(pos, null);
                    continue;
                }

                var down = walls.GetTile(pos + Vector3Int.down);
                var up = walls.GetTile(pos + Vector3Int.up);

                bool topWall = up == null && down != null;
                bool bottomWall = up != null && down == null;

                wallCollision.SetTile(pos, topWall ? null : levelScript.WallCollisionTile);
                projectileCollision.SetTile(pos, bottomWall || topWall ? null : levelScript.ProjectileCollisionTile);
            }
        }
    }
}
