using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Text;

public class PathFinder {

    private Level level;
	
    public PathFinder(Level level)
    {
        this.level = level;
    }

    public Path GetPath(int xFrom, int yFrom, int xTo, int yTo)
    {
        if (level == null || !level.Contains(xFrom, yFrom) || !level.Contains(xTo, yTo)) return null;

        bool[,] map = level.map;
        if (map[xFrom, yFrom] || map[xTo, yTo]) return null; // If either start or end are walls no path can be created

        if(xFrom == xTo && yFrom == yTo)
        {
            Path path = new Path();
            path.Add(new Coord(xFrom, yFrom));
            return path;
        }

        return AStar(new Coord(xFrom, yFrom), new Coord(xTo, yTo));
    }

    private Path AStar(Coord from, Coord to)
    {
        Vertex start = new Vertex(from);
        Vertex goal = new Vertex(to);
        start.Dist = 0;

        SimplePriorityQueue<Vertex> queue = new SimplePriorityQueue<Vertex>();
        queue.Enqueue(start, start.Dist);

        HashSet<Coord> visited = new HashSet<Coord>();
        Path path = new Path();

        int iterations = 0;
        while(queue.Count > 0)
        {
            iterations++;
            Vertex vert = queue.Dequeue();
            visited.Add(vert.Point);

            if(vert == goal)
            {
                path.Add(vert.Point);
                while(vert.prev != null)
                {
                    path.Add(vert.prev.Point);
                    vert = vert.prev;
                }
                path.Reverse();
                path.Reduce(level.map);
                path.CreateWaypoints();
                return path;
            }

            // Goes through all the unvisited, valid neighbors of this point
            foreach(Vertex v in Neighbors(vert, visited))
            {
                visited.Add(v.Point);
                float dist = vert.Dist + Dist(vert, v) + Dist(v, goal) + WallPenalty(v);

                // If  the queue contains one of the neighbors, check to see if we have a better path
                if (queue.Contains(v))
                {
                    // If there's a shorter path, update the existing vertex
                    if(queue.GetPriority(v) > dist)
                    {
                        Vertex existing = GetFromQueue(queue, v);
                        queue.UpdatePriority(existing, dist);
                        existing.prev = vert;
                        existing.Dist = dist;
                    }
                }
                else
                {
                    v.prev = vert;
                    v.Dist = dist;
                    queue.Enqueue(v, v.Dist);
                }
            }
        }
        return path;
    }

    private Vertex GetFromQueue(SimplePriorityQueue<Vertex> queue, Vertex vert)
    {
        foreach(Vertex v in queue)
        {
            if (v == vert) return v;
        }
        return null;
    }

    // Squared dist between two vertices
    private float Dist(Vertex v1, Vertex v2)
    {
        return (v1.Point.tx - v2.Point.tx) * (v1.Point.tx - v2.Point.tx) + (v1.Point.ty - v2.Point.ty) * (v1.Point.ty - v2.Point.ty);
    }

    private List<Vertex> Neighbors(Vertex vert, HashSet<Coord> visited)
    {
        List<Vertex> neighbors = new List<Vertex>();
        for(int x = vert.Point.tx - 1; x <= vert.Point.tx + 1; x++)
        {
            for (int y = vert.Point.ty - 1; y <= vert.Point.ty + 1; y++)
            {
                if (x != vert.Point.tx && y != vert.Point.ty) continue;

                Coord point = new Coord(x, y);
                if (vert.Point == point || visited.Contains(point) || level.map[point.tx, point.ty]) continue;
                neighbors.Add(new Vertex(point));
            }
        }
        return neighbors;
    }

    private float WallPenalty(Vertex vert)
    {
        float penalty = 0;
        float wallCost = 1000f;
        if (level.map[vert.Point.tx - 1, vert.Point.ty]) penalty += wallCost;
        if (level.map[vert.Point.tx + 1, vert.Point.ty]) penalty += wallCost;
        if (level.map[vert.Point.tx, vert.Point.ty - 1]) penalty += wallCost;
        if (level.map[vert.Point.tx, vert.Point.ty + 1]) penalty += wallCost;
        return penalty;
    }

    class Vertex
    {
        public Coord Point;
        public float Dist;
        public Vertex prev = null;

        public Vertex(Coord coord)
        {
            Point = coord;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType()) return false;
            Vertex other = (Vertex)obj;
            return Point == other.Point;
        }

        public override int GetHashCode()
        {
            return Point.GetHashCode();
        }

        public static bool operator ==(Vertex v1, Vertex v2)
        {
            if(object.ReferenceEquals(v1, null))
            {
                return object.ReferenceEquals(v2, null);
            }
            return v1.Equals(v2);
        }

        public static bool operator !=(Vertex v1, Vertex v2)
        {
            return !(v1 == v2);
        }

        public override string ToString()
        {
            return "Point - " + Point.ToString() + ", Dist - " + Dist;
        }
    }

}

public class Path
{
    public List<Coord> path;

    public Path()
    {
        path = new List<Coord>();
    }

    public void Add(Coord point)
    {
        path.Add(point);
    }

    public void Reverse()
    {
        path.Reverse();
    }

    public void CreateWaypoints()
    {
        for(int i = 0; i < path.Count - 2; i++)
        {
            Coord c0 = path[i];
            Coord c1 = path[i + 1];
            Coord c2 = path[i + 2];

            int a = c0.tx * (c1.ty- c2.ty) + c1.tx * (c2.ty - c0.ty) + c2.tx * (c0.ty - c1.ty);
            if (a == 0)
            {
                path.RemoveAt(i + 1);
                i--;
            }
        }
    }

    public void Reduce(bool[,] map)
    {
        for(int i = 0; i < path.Count - 2; i++)
        {
            Coord c0 = path[i];
            Coord c1 = path[i + 1];
            Coord c2 = path[i + 2];

            Coord diff = c0 - c2;

            // If we have a diagonal path with no walls on either side, we can simplify it to a straight line
            if(Mathf.Abs(diff.tx) == 1 && Mathf.Abs(diff.ty) == 1 && !(map[c0.tx, c2.ty] || map[c2.tx, c0.ty]))
            {
                path.RemoveAt(i + 1);
            }
        }
    }

    public bool OnPath(int x, int y)
    {
        foreach(Coord c in path)
        {
            if (c.tx == x && c.ty == y) return true;
        }
        return false;
    }

    public bool OnPath(Vector2 point)
    {
        return OnPath((int)point.x, (int)point.y);
    }

    public bool OnPath(Coord coord)
    {
        return OnPath(coord.tx, coord.ty);
    }

    public bool AtGoal(int x, int y)
    {
        Coord c = path[path.Count - 1];
        return c.tx == x && c.ty == y;
    }

    public bool AtGoal(Vector2 point) 
    {
        return AtGoal((int)point.x, (int)point.y);
    }

    public Coord GetNext(int x, int y)
    {
        int index = path.IndexOf(new Coord(x, y));
        if (index == -1 || index == path.Count - 1) return default(Coord);
        return path[index + 1];
    }

    public Coord GetNext(Coord curr)
    {
        return GetNext(curr.tx, curr.ty);
    }

    public Coord GetNext(Vector2 point)
    {
        return GetNext((int)point.x, (int)point.y);
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        foreach(Coord point in path)
        {
            builder.Append(point + " -> ");
        }
        return builder.ToString();
    }
}
