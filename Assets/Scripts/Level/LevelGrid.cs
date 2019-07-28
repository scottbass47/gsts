using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private LayerMask wallMask;
    private Tilemap floor;
    private Vector2 gridWorldSize;
    private LayerMask unwalkableMask;
    private float nodeRadius;
    private Node[,] grid;

    public int MaxSize => grid.Length;
    public int GridSizeX => grid.GetLength(0);
    public int GridSizeY => grid.GetLength(1);

    private void Start()
    {
        var levelScript = GetComponent<LevelScript>();
        CreateGrid(levelScript.floorDecor, levelScript.wallCollision);
    }

    public void CreateGrid(Tilemap floor, Tilemap walls)
    {
        this.floor = floor;
        floor.CompressBounds();
        var bounds = floor.cellBounds;

        grid = new Node[bounds.size.x, bounds.size.y];

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cellPos = new Vector3Int(x, y, 0);
                var worldPos = floor.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0);
                var result = Physics2D.OverlapBox(worldPos, Vector2.one * 2, 0, wallMask);
                var walkable = walls.GetTile(cellPos) == null;
                var xGrid = x - bounds.xMin;
                var yGrid = y - bounds.yMin;
                grid[xGrid, yGrid] = new Node(walkable, worldPos, xGrid, yGrid, result != null ? 5 : 0);
            }
        }
    }

    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.xGrid + x;
                int checkY = node.yGrid + y;

                if (checkX >= 0 && checkX < GridSizeX && checkY >= 0 && checkY < GridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node GetNode(int xGrid, int yGrid)
    {
        return OOB(xGrid, yGrid) ? null : grid[xGrid, yGrid];
    }

    private bool OOB(int x, int y)
    {
        return x < 0 || y < 0 || x >= GridSizeX || y >= GridSizeY;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        var cellPos = floor.WorldToCell(worldPosition);
        var bounds = floor.cellBounds;
        var x = cellPos.x - bounds.xMin;
        var y = cellPos.y - bounds.yMin;
        if (OOB(x,y)) return null;
        return grid[x, y];
    }

    public List<Node> GetNodesInRange(Node start, float range)
    {
        return GetNodesInMinMaxRange(start, 0, range);
    }

    public List<Node> GetNodesInMinMaxRange(Node start, float minRange, float maxRange)
    {
        return GetNodesBrushFire(start, node =>
        {
            if (!node.Walkable) return false;
            var xDiff = node.xGrid - start.xGrid;
            var yDiff = node.yGrid - start.yGrid;
            var distSqr = xDiff * xDiff + yDiff * yDiff;
            return distSqr <= maxRange * maxRange && distSqr >= minRange * minRange;
            //return node.BrushFireDist >= minRange && node.BrushFireDist <= maxRange;
        }, onlyExpandWalkable:true,
            maxBrushFireDist:2*Mathf.FloorToInt(maxRange)
        );
    }

    public List<Node> GetNodesBrushFire(Node start, Predicate<Node> pred, bool onlyExpandWalkable=false, int maxBrushFireDist=int.MaxValue)
    {
        if (start == null) return null;
        
        var nodes = new List<Node>();
        var visited = new HashSet<Node>();
        var toVisit = new Queue<Node>();
        start.BrushFireDist = 0;
        toVisit.Enqueue(start);

        while (toVisit.Count > 0)
        {
            var node = toVisit.Dequeue();
            var filter = pred(node);
            if (filter)
            {
                nodes.Add(node);
            }
            visited.Add(node);

            for (int xx = node.xGrid - 1; xx <= node.xGrid + 1; xx++)
            {
                for (int yy = node.yGrid - 1; yy <= node.yGrid + 1; yy++)
                {
                    if (xx != node.xGrid && yy != node.yGrid) continue;

                    var neighbor = GetNode(xx, yy);

                    if (neighbor == null || visited.Contains(neighbor) || toVisit.Contains(neighbor)) continue;
                    if (onlyExpandWalkable && !neighbor.Walkable) continue;

                    neighbor.BrushFireDist = node.BrushFireDist + 1;
                    if(neighbor.BrushFireDist <= maxBrushFireDist) toVisit.Enqueue(neighbor);
                }
            }
        }
        return nodes;
    }

    //private void OnDrawGizmos()
    //{
    //    if (path != null) {
    //        foreach (Node n in path) {
    //            Gizmos.color = Color.black;
    //            Gizmos.DrawCube(n.WorldPosition, Vector3.one * 0.9f);
    //        }
    //    }
    //    else if(grid != null)
    //    {
    //        foreach(var node in grid)
    //        {
    //            var color = node.Walkable ? Color.white : Color.red;
    //            Gizmos.color = color;
    //            Gizmos.DrawCube(node.WorldPosition, Vector3.one * 0.9f);
    //        }
    //    }
    //}

}
