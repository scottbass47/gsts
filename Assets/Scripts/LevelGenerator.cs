using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour {

    private int Width = 64;
    private int Height = 64;
    [Range(0.0f, 1.0f)]
    public float StartAliveChance = 0.45f;
    //public int DeathLimit = 3;
    //public int BirthLimit = 3;
    public int Iterations = 8;
    public int NumEnemies = 10;

    private bool[,] cellMap;
    private List<Region> regions;
    private List<Connection> connections;

    private HashSet<Coord> usedSpawns;
    private Coord playerSpawn;
    private Coord levelExit;
    private Coord treasureSpawn;
    private List<Coord> enemySpawns;

    public Level Level { get; private set; }

    // Use this for initialization
    void Start () {
        //GenerateMap();
    }

    public void GenerateMap()
    {
        // Get seed
        int seed = (int)System.DateTime.Now.Ticks;
        Random.InitState(seed);
        print("Seed: " + seed);

        usedSpawns = new HashSet<Coord>();
        InitMap();

        for(int i = 0; i < Iterations; i++)
        {
            cellMap = SimulateStep(cellMap);
        }
        
        connections = new List<Connection>();

        FindAllRegions();
        List<Room> rooms = CleanupMap();
        rooms.Sort();
        rooms[0].IsMainRoom = true;
        rooms[0].AccessibleFromMainRoom = true;

        ConnectRooms(rooms);
        DrawConnections();

        // Open up cooridors and delete extraneous walls
        HashSet<Coord> toDelete = ExpandCorridors();

        foreach (Coord coord in toDelete)
        {
            if (coord.tx == 0 || coord.ty == 0 || coord.tx == Width - 1 || coord.ty == Height - 1) continue;
            cellMap[coord.tx, coord.ty] = false;
        }
        DeleteStickingOutWalls();

        PickPlayerSpawn();
        usedSpawns.Add(playerSpawn);

        List<Coord> spawns = FindPotentialSpawns(playerSpawn);
        PickOtherSpawns(spawns);
        usedSpawns.Add(levelExit);
        usedSpawns.Add(treasureSpawn);

        enemySpawns = PickEnemySpawns();
        foreach (Coord spawn in enemySpawns) usedSpawns.Add(spawn);

        Texture2D tex = new Texture2D(Width, Height);
        tex.filterMode = FilterMode.Point;
        Color32[] pixels = tex.GetPixels32();

        for (int j = 0; j < pixels.Length; j++)
        {
            pixels[j] = cellMap[j % Width, j / Height] ? new Color32(68, 51, 51, 255) : new Color32(51, 85, 170, 255);
        }

        /*for(int i = 0; i < spawns.Count; i++)
        {
            Coord coord = spawns[i];
            pixels[coord.tx + coord.ty * Width] = new Color32(255, 0, 0, Convert.ToByte(255 - i * 6));

        }*/
        /*foreach (Coord coord in spawns)
        {
            pixels[coord.tx + coord.ty * Width] = Color.red;
        }*/

        pixels[playerSpawn.tx + playerSpawn.ty * Width] = Color.green;
        pixels[levelExit.tx + levelExit.ty * Width] = Color.magenta;
        pixels[treasureSpawn.tx + treasureSpawn.ty * Width] = Color.gray;

        foreach(Coord coord in enemySpawns)
        {
            pixels[coord.tx + coord.ty * Width] = Color.red;
        }

        /*foreach (Coord coord in toExpand)
        {
            pixels[coord.tx + coord.ty * Width] = Color.black;
        }*/


        foreach (Coord coord in toDelete)
        {
            if(cellMap[coord.tx, coord.ty])
            {
                //pixels[coord.tx + coord.ty * Width] = Color.magenta;
            }
        }

        /*foreach(Connection connection in connections)
        { 
            DrawUtils.DrawLine(connection.c1.tx, connection.c1.ty, connection.c2.tx, connection.c2.ty, Width, Color.green, pixels);
        }*/

        //for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        //foreach (Coord coord in DrawArc(Width / 2, Height / 2, 15, -135, 135)) pixels[coord.tx + coord.ty * Width] = Color.red;

        tex.SetPixels32(pixels);
        tex.Apply();

        //System.IO.File.WriteAllBytes("C:/Users/bobba/Desktop/out.png", tex.EncodeToPNG());
        Level = new Level();
        Level.Width = Width;
        Level.Height = Height;
        Level.map = cellMap;
        Level.playerSpawn = new Vector2(playerSpawn.tx, playerSpawn.ty);
        Level.exit = new Vector2(levelExit.tx, levelExit.ty);
        Level.chest = new Vector2(treasureSpawn.tx, treasureSpawn.ty);
        Level.enemySpawns = new List<Vector2>();
        foreach(Coord spawn in enemySpawns)
        {
            Level.enemySpawns.Add(new Vector2(spawn.tx, spawn.ty));
        }

        GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 8);
    }

    private void PrintMap(int i)
    {
        Texture2D tex = new Texture2D(Width, Height);
        Color32[] pixels = tex.GetPixels32();

        for (int j = 0; j < pixels.Length; j++)
        {
            pixels[j] = cellMap[j % Width, j / Height] ? new Color32(68, 51, 51, 255) : new Color32(51, 85, 170, 255);
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        System.IO.File.WriteAllBytes("C:/Users/Scott/Desktop/map" + i + ".png", tex.EncodeToPNG());
    }

    private void InitMap()
    {
        cellMap = new bool[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Edge tiles should be walls
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1) cellMap[x, y] = true;
                else cellMap[x, y] = Random.Range(0.0f, 1.0f) < StartAliveChance;
            }
        }
    }

    private bool[,] SimulateStep(bool[,] oldMap/*, bool round1*/)
    {
        bool[,] newMap = new bool[Width, Height];

        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                int neighbors = CountNeighbors(oldMap, x, y);

                if (oldMap[x, y]) newMap[x, y] = neighbors >= 4;
                else newMap[x, y] = neighbors >= 5 /*|| (round1 && neighbors <= 0)*/;
                
            }
        }
        return newMap;
    }

    private void DeleteStickingOutWalls()
    {
        for(int x = 1; x < Width - 1; x++)
        {
            for(int y = 1; y < Height - 1; y++)
            {
                if (!cellMap[x, y]) continue;

                bool[] nbs = new bool[]
                {
                    cellMap[x - 1, y + 1],
                    cellMap[x, y + 1],
                    cellMap[x + 1, y + 1],
                    cellMap[x + 1, y],
                    cellMap[x + 1, y - 1],
                    cellMap[x, y - 1],
                    cellMap[x - 1, y - 1],
                    cellMap[x - 1, y]
                };

                if(CountNeighbors(cellMap, x, y) == 3 && CountAdjacent(nbs) == 3) {
                    if((nbs[0] && nbs[1] && nbs[2]) ||
                        (nbs[2] && nbs[3] && nbs[4]) ||
                        (nbs[4] && nbs[5] && nbs[6]) ||
                        (nbs[6] && nbs[7] && nbs[0]))
                    {
                        cellMap[x, y] = false;
                    }
                }
            }
        }
    }

    private int CountNeighbors(bool[,] map, int x, int y)
    {
        int count = 0;
        for(int xx = x - 1; xx <= x + 1; xx++)
        {
            for (int yy = y - 1; yy <= y + 1; yy++)
            {
                if (xx == x && yy == y) continue;
                if (xx < 0 || yy < 0 || xx >= Width || yy >= Width) count++;
                else count += map[xx, yy] ? 1 : 0;
            }
        }
        return count;
    }

    public void FindAllRegions()
    {
        regions = new List<Region>();

        HashSet<Coord> visited = new HashSet<Coord>();

        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                Coord coord = new Coord(x, y);
                if (visited.Contains(coord)) continue;

                Region region = new Region();
                region.walls = cellMap[coord.tx, coord.ty];

                FloodFill(region, coord, visited);
                regions.Add(region);
            }
        }
    }

    private void FloodFill(Region region, Coord start, HashSet<Coord> visited)
    {
        Queue<Coord> toCheck = new Queue<Coord>();
        toCheck.Enqueue(start);

        while(toCheck.Count > 0)
        {
            Coord coord = toCheck.Dequeue();
            region.coords.Add(coord);
            visited.Add(coord);

            Coord c0 = default(Coord);

            for(int i = 0; i < 4; i++)
            {
                if (i == 0) c0 = new Coord(coord.tx + 1, coord.ty);
                if (i == 1) c0 = new Coord(coord.tx - 1, coord.ty);
                if (i == 2) c0 = new Coord(coord.tx, coord.ty + 1);
                if (i == 3) c0 = new Coord(coord.tx, coord.ty - 1);
                if (!OOB(c0.tx, c0.ty) && !visited.Contains(c0) && !toCheck.Contains(c0) && cellMap[c0.tx, c0.ty] == region.walls) toCheck.Enqueue(c0);
            }
        }
    }

    private bool OOB(int x, int y)
    {
        return x < 0 || y < 0 || x >= Width || y >= Height;
    }

    private List<Room> CleanupMap()
    {
        List<Room> rooms = new List<Room>();
        int wallThreshold = 50;
        int floorThreshold = 50;
        foreach (Region region in regions)
        {
            // Flip the region's type if it's under the threshold
            // This gets rid of small islands of floors/walls
            if((region.walls && region.coords.Count < wallThreshold) || (!region.walls && region.coords.Count < floorThreshold))
            {
                foreach(Coord coord in region.coords)
                {
                    cellMap[coord.tx, coord.ty] = !cellMap[coord.tx, coord.ty];
                }
            }
            else if(!region.walls)
            {
                rooms.Add(new Room(region, cellMap));
            }
        }
        return rooms;
    }

    private void ConnectRooms(List<Room> rooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if(forceAccessibilityFromMainRoom)
        {
            foreach(Room room in rooms)
            {
                if(room.AccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
            if (roomListA.Count == 0) return;
        }
        else
        {
            roomListA = rooms;
            roomListB = rooms;
        }

        bool found = false;
        Coord bestCoord1 = default(Coord);
        Coord bestCoord2 = default(Coord);
        Room room1 = null;
        Room room2 = null;
        int best = int.MaxValue;

        foreach (Room r1 in roomListA)
        {
            if(!forceAccessibilityFromMainRoom)
            {
                found = false;
                best = int.MaxValue;
                if (r1.connectedRooms.Count > 0) continue;
            }

            foreach (Room r2 in roomListB)
            {
                if (r1 == r2 || r1.IsConnected(r2))
                {
                    continue;
                }

                Coord c1;
                Coord c2;
                int dist = FindBestConnection(r1, r2, out c1, out c2);
                if (dist < best)
                {
                    best = dist;
                    bestCoord1 = c1;
                    bestCoord2 = c2;
                    found = true;
                    room1 = r1;
                    room2 = r2;
                }
            }

            if (!forceAccessibilityFromMainRoom && found)
            {
                MakePassage(r1, room2, bestCoord1, bestCoord2);
            }
        }
        if(forceAccessibilityFromMainRoom && found)
        {
            MakePassage(room1, room2, bestCoord1, bestCoord2);
            ConnectRooms(rooms, true);
        }

        if(!forceAccessibilityFromMainRoom)
        {
            ConnectRooms(rooms, true);
        }
    }

    private int FindBestConnection(Room r1, Room r2, out Coord bestCoord1, out Coord bestCoord2)
    {
        int best = int.MaxValue;
        bestCoord1 = default(Coord);
        bestCoord2 = default(Coord);

        foreach(Coord c1 in r1.edgeTiles)
        {
            foreach(Coord c2 in r2.edgeTiles)
            {
                int dist = (c1.tx - c2.tx) * (c1.tx - c2.tx) + (c1.ty - c2.ty) * (c1.ty - c2.ty);
                if(dist < best)
                {
                    best = dist;
                    bestCoord1 = c1;
                    bestCoord2 = c2;
                }
            }
        }
        return best;
    }

    private void MakePassage(Room r1, Room r2, Coord c1, Coord c2)
    {
        Room.ConnectRooms(r1, r2);
        connections.Add(new Connection(c1, c2));
    }

    public void DrawConnections()
    {
        foreach(Connection connection in connections)
        {
            List<Vector2> points = DrawUtils.DrawLine(connection.c1.tx, connection.c1.ty, connection.c2.tx, connection.c2.ty);
            foreach(Vector2 point in points)
            {
                DrawCircle(new Coord((int)point.x, (int)point.y), 2);
            }
        }
    }

    private void DrawCircle(Coord c, int r)
    {
        for(int x = -r; x <= r; x++)
        {
            for(int y = -r; y <= r; y++)
            {
                if(x * x + y * y <= r * r)
                {
                    int xx = x + c.tx;
                    int yy = y + c.ty;
                    if(!OOB(xx, yy) && !(xx == 0 || xx == Width - 1 || yy == 0 || yy == Height - 1))
                    {
                        cellMap[xx, yy] = false;
                    }
                }
            }
        }
    }
	
    private void DrawSquare(int x, int y, int w, int h)
    {
        for(int xx = x; xx < x + w; xx++)
        {
            for(int yy = y; yy < y + h; yy++)
            {
                if (!OOB(xx, yy) && !(xx == 0 || xx == Width - 1 || yy == 0 || yy == Height - 1))
                {
                    cellMap[xx, yy] = false;
                }
            }
        }
    }

    // Angles from and to are in CW direction
    // Angles must be between -180 and 180
    // Ex: from - 45, to - 0 would fill a 45 deg arc in first quadrant
    private List<Coord> DrawArc(int x, int y, int r, float from, float to)
    {
        List<Coord> pixels = new List<Coord>();
        from *= Mathf.Deg2Rad;
        to *= Mathf.Deg2Rad;

        bool add360 = from <= to;
        if(add360)
        {
            from += Mathf.PI * 2;
        }

        for (int xx = -r; xx <= r; xx++)
        {
            for (int yy = -r; yy <= r; yy++)
            {
                if (xx * xx + yy * yy - 1 <= r * r)
                {
                    float ang = Mathf.Atan2(yy, xx);
                    if (add360 && ang < 0) ang += Mathf.PI * 2;
                    if(to <= ang && ang <= from)
                    {
                        // Draw pixel
                        pixels.Add(new Coord(x + xx, y + yy));
                    }
                }
            }
        }
        return pixels;
    }

    private HashSet<Coord> ExpandCorridors()
    {
        HashSet<Coord> toDelete = new HashSet<Coord>();
        HashSet<Coord> visited = new HashSet<Coord>();
        bool prevWall = true;

        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                Coord start = new Coord(x, y);

                // If you were coming in from a floor tile and now you hit a wall, start the tracing
                if(!prevWall && cellMap[x, y] && !visited.Contains(start))
                {
                    visited.Add(start);
                    toDelete.UnionWith(Trace(start, 7, visited));
                }
                prevWall = cellMap[x, y];
            }
        }
        return toDelete;
    }

    private HashSet<Coord> Trace(Coord start, int mooreIndex, HashSet<Coord> visited)
    {
        HashSet<Coord> toDelete = new HashSet<Coord>();
        bool atStart = false;
        Coord center = new Coord(start.tx, start.ty);
        Coord[] mooreNb = Neighborhood(center);
        List<Coord> path = new List<Coord>();
        path.Add(center);
        Coord pathPrev = center;
        int prevMooreIndex = mooreIndex;

        while(!atStart)
        {
            Coord next = mooreNb[mooreIndex];

            if(cellMap[next.tx, next.ty])
            {
                Coord prev = mooreNb[mooreIndex - 1 < 0 ? 7 : mooreIndex - 1];
                pathPrev = center;
                center = next;
                mooreNb = Neighborhood(center);
                mooreIndex = GetMooreIndex(mooreNb, prev);
                prevMooreIndex = mooreIndex;
                visited.Add(center);

                if (path.Contains(center) && path.IndexOf(center) > 0 && path[path.IndexOf(center) - 1] == pathPrev) {
                    return toDelete;
                }

                path.Add(center);
            }
            else
            {
                if(prevMooreIndex != mooreIndex && !toDelete.Contains(center))
                {
                    float from = GetAngle(prevMooreIndex);
                    float to = GetAngle(mooreIndex);
                    List<Coord> coords = DrawArc(center.tx, center.ty, 3, from, to);
                    coords.Remove(center);
                    toDelete.UnionWith(coords);
                }
                /*int dx = 0;
                int dy = 0;
                switch (mooreIndex)
                {
                    case 0:
                        dx = -1;
                        dy = 1;
                        break;
                    case 1:
                        dy = 1;
                        break;
                    case 2:
                        dx = 1;
                        dy = 1;
                        break;
                    case 3:
                        dx = 1;
                        break;
                    case 4:
                        dx = 1;
                        dy = -1;
                        break;
                    case 5:
                        dy = -1;
                        break;
                    case 6:
                        dx = -1;
                        dy = -1;
                        break;
                    case 7:
                        dx = -1;
                        break;
                }

                int i = 0;
                int amount = dx == 0 || dy == 0 ? 2 : 2;
                Coord c = new Coord(next.tx, next.ty);
                do
                {
                    if(!IsMapEdge(c.tx, c.ty)) toDelete.Add(c);
                    c.tx += dx;
                    c.ty += dy;
                    i++;
                } while (i < amount);*/
                prevMooreIndex = mooreIndex;
                mooreIndex = (mooreIndex + 1) % 8;
            }
        }
        return toDelete;
    }

    private float GetAngle(int mooreIndex)
    {
        switch(mooreIndex)
        {
            case 0: return 135;
            case 1: return 90;
            case 2: return 45;
            case 3: return 0;
            case 4: return -45;
            case 5: return -90;
            case 6: return -135;
            case 7: return 180;
        }
        return 0;
    }

    private int GetMooreIndex(Coord[] nb, Coord c)
    {
        for(int i = 0; i < nb.Length; i++)
        {
            if (nb[i] == c) return i;
        }
        return -1;
    }

    // 1 2 3
    // 8   4
    // 7 6 5
    private Coord[] Neighborhood(Coord coord)
    {
        return new Coord[]
        {
            new Coord(coord.tx - 1, coord.ty + 1),
            new Coord(coord.tx, coord.ty + 1),
            new Coord(coord.tx + 1, coord.ty + 1),
            new Coord(coord.tx + 1, coord.ty),
            new Coord(coord.tx + 1, coord.ty - 1),
            new Coord(coord.tx, coord.ty - 1),
            new Coord(coord.tx - 1, coord.ty - 1),
            new Coord(coord.tx - 1, coord.ty),
        };
    }

    private void PickPlayerSpawn()
    {
        int r = Width / 3;
        int xcenter = Width / 2;
        int ycenter = Height / 2;

        playerSpawn = default(Coord);

        bool looking = true;
        do
        {
            playerSpawn = new Coord(Random.Range(0, Width), Random.Range(0, Height));
            if(!cellMap[playerSpawn.tx, playerSpawn.ty])
            {
                int xx = playerSpawn.tx - xcenter;
                int yy = playerSpawn.ty - ycenter;

                if(xx * xx + yy * yy > r * r && CountNeighbors(cellMap, playerSpawn.tx, playerSpawn.ty) == 0)
                {
                    looking = false;
                }

            }
        } while (looking);
    }

    private void PickOtherSpawns(List<Coord> spawns)
    {
        // First pick level exit
        int index = Random.Range(0, spawns.Count / 5);
        levelExit = spawns[spawns.Count - index - 1];
        spawns.RemoveAt(index);

        for(int i = spawns.Count - 1; i >= 0; i--)
        {
            Coord coord = spawns[i];
            if (Mathf.Abs(coord.tx - levelExit.tx) <= 2 && Mathf.Abs(coord.ty - levelExit.ty) <= 2)
            {
                spawns.RemoveAt(i);
            }
        }

        // Treasure
        if(spawns.Count == 0)
        {
            print("No spawn points for treasure.");
            return;
        }
        index = Random.Range(0, spawns.Count);
        treasureSpawn = spawns[spawns.Count - index - 1];
        spawns.RemoveAt(index);
    }

    private List<Coord> PickEnemySpawns()
    {
        List<Coord> ret = new List<Coord>();
        float safeBubbleRadius = 15; // No enemy spawns within 10 tiles of player
        int px = playerSpawn.tx;
        int py = playerSpawn.ty;
        List<Coord> potentialSpawns = Level.FindCoords(playerSpawn, coord =>
        {
            int x = coord.tx;
            int y = coord.ty;
            float sqrDist = (px - x) * (px - x) + (py - y) * (py - y);
            return CountNeighbors(cellMap, x, y) == 0 && sqrDist > safeBubbleRadius * safeBubbleRadius && !usedSpawns.Contains(coord);
        }, cellMap);

        for(int i = 0; i < NumEnemies; i++)
        {
            int idx = Random.Range(0, potentialSpawns.Count);
            ret.Add(potentialSpawns[idx]);
            potentialSpawns.RemoveAt(idx);
        }

        return ret;
    }

    /*private List<Coord> PickLevelExit(Coord start)
    {
        // Brushfire out
        List<Coord> potentialLevelExits = FindPotentialSpawns(start);
        int size = potentialLevelExits.Count;
        int range = size / 5;
        int rand = Random.Range(0, range);
        levelExit = potentialLevelExits[size - rand - 1];

        return potentialLevelExits;
    }*/

    

    private List<Coord> FindPotentialSpawns(Coord playerSpawn)
    {
        return Level.FindCoords(playerSpawn, ValidExit, cellMap);
    }

    private bool ValidExit(Coord coord)
    {
        if (CountNeighbors(cellMap, coord.tx, coord.ty) != 0) return false;
        if (coord == playerSpawn) return false;

        int range = 3;
        int diagRange = 3;
        int count = 0; // How many sides are within range of a wall

        bool[] nbs = NeighborhoodRangeCheck(coord, range, diagRange);
        foreach (bool b in nbs) count += b ? 1 : 0;

        return CountAdjacent(nbs) >= 4 && count >= 6;
    }

    private bool TunnelPoint(Coord coord)
    {
        int range = 2;
        //if (CountNeighbors(cellMap, coord.tx, coord.ty) != 0) return false;

        bool[] nbs = NeighborhoodRangeCheck(coord, range, range - 1);
        if (CountAdjacent(nbs) >= 3) return false;

        return (nbs[0] && nbs[4]) ||
            (nbs[1] && nbs[5]) ||
            (nbs[2] && nbs[6]) ||
            (nbs[3] && nbs[7]);

        /*return (RangeCheck(coord, 1, 0, range) && RangeCheck(coord, -1, 0, range)) ||
               (RangeCheck(coord, 0, 1, range) && RangeCheck(coord, 0, -1, range)) ||
               (RangeCheck(coord, 1, 1, range) && RangeCheck(coord, -1, -1, range)) ||
               (RangeCheck(coord, 1, -1, range) && RangeCheck(coord, -1, 1, range));*/
    }

    // 1 2 3
    // 8   4
    // 7 6 5
    private bool[] NeighborhoodRangeCheck(Coord coord, int adjRange, int diagRange)
    {
        return new bool[]
        {
            RangeCheck(coord, -1, 1, diagRange),
            RangeCheck(coord, 0, 1, adjRange),
            RangeCheck(coord, 1, 1, diagRange),
            RangeCheck(coord, 1, 0, adjRange),
            RangeCheck(coord, 1, -1, diagRange),
            RangeCheck(coord, 0, -1, adjRange),
            RangeCheck(coord, -1, -1, diagRange),
            RangeCheck(coord, -1, 0, adjRange),

        };
    }

    private int CountAdjacent(bool[] neighborhood)
    {
        int most = 0;
        int curr = 0;

        bool head = false;
        int front = 0;
        int back = 0;

        for(int i = 0; i < neighborhood.Length; i++)
        {
            if (neighborhood[i])
            {
                curr++;
                if (i == 0) head = true;
                if(i == 7)
                {
                    back = curr;
                    most = Math.Max(most, back);
                }
            }
            else
            {
                most = Math.Max(most, curr);
                if (head)
                {
                    front = curr;
                    head = false;
                }
                curr = 0;
            }
        }
        if (most == 8) return most;

        most = Math.Max(most, back + front);
        return most;
    }

    private bool RangeCheck(Coord coord, int dx, int dy, int range)
    {
        int xx = coord.tx + dx;
        int yy = coord.ty + dy;
        int i = 1;
        do
        {
            if (cellMap[xx, yy])
            {
                return true;
            }
            xx += dx;
            yy += dy;
            i++;
        }
        while (i <= range);

        return false;
    }

    private bool IsMapEdge(int x, int y)
    {
        return x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
    }

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Z))
        {
            GenerateMap();
        }
	}

    class Room : IComparable<Room>
    {
        public Region region;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int Size;
        public bool AccessibleFromMainRoom;
        public bool IsMainRoom;

        public Room(Region region, bool[,] cellMap)
        {
            this.region = region;
            Size = region.coords.Count;

            connectedRooms = new List<Room>();
            edgeTiles = new List<Coord>();

            // We don't have to do a bounds check because floor tiles are NEVER on the border
            foreach(Coord tile in region.coords)
            {
                if(cellMap[tile.tx + 1, tile.ty] ||
                    cellMap[tile.tx - 1, tile.ty] ||
                    cellMap[tile.tx, tile.ty + 1] ||
                    cellMap[tile.tx, tile.ty - 1])
                {
                    edgeTiles.Add(tile);
                }
            }
        }

        public void SetAccessibleFromMainRoom()
        {
            if(!AccessibleFromMainRoom)
            {
                AccessibleFromMainRoom = true;
                foreach(Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room r1, Room r2)
        {
            if(r1.AccessibleFromMainRoom)
            {
                r2.SetAccessibleFromMainRoom();
            }
            else if(r2.AccessibleFromMainRoom)
            {
                r1.SetAccessibleFromMainRoom();
            }
            r1.connectedRooms.Add(r2);
            r2.connectedRooms.Add(r1);
        }

        public bool IsConnected(Room other)
        {
            return connectedRooms.Contains(other);
        }

        public int CompareTo(Room other)
        {
            return other.Size.CompareTo(Size);
        }
    }

    class Region
    {
        public List<Coord> coords { get; private set; }
        public bool walls;

        public Region()
        {
            coords = new List<Coord>();
            walls = false;
        }
    }

    struct Connection
    {
        public Coord c1;
        public Coord c2;

        public Connection(Coord c1, Coord c2)
        {
            this.c1 = c1;
            this.c2 = c2;
        }
    }
}

public struct Coord
{
    public int tx;
    public int ty;

    public Coord(int x, int y)
    {
        tx = x;
        ty = y;
    }

    public Coord(Vector2 position) : this((int)position.x, (int)position.y) {}

    public override bool Equals(object obj)
    {
        return obj is Coord && this == (Coord)obj;
    }
    public override int GetHashCode()
    {
        return tx.GetHashCode() ^ ty.GetHashCode();
    }
    public static bool operator ==(Coord x, Coord y)
    {
        return x.tx == y.tx && x.ty == y.ty;
    }
    public static bool operator !=(Coord x, Coord y)
    {
        return !(x == y);
    }

    public static Coord operator -(Coord x, Coord y)
    {
        return new Coord(x.tx - y.tx, x.ty - y.ty);
    }

    public static Coord operator +(Coord x, Coord y)
    {
        return new Coord(x.tx + y.tx, x.ty + y.ty);
    }

    public static explicit operator Vector2(Coord coord)
    {
        return new Vector2(coord.tx, coord.ty);
    }
}

public class Level
{
    public int Width;
    public int Height;
    public bool[,] map;
    public Vector2 origin;
    public Vector2 playerSpawn;
    public Vector2 exit;
    public Vector2 chest;
    public List<Vector2> enemySpawns;
    
    public Level()
    {
    }

    public PathFinder GetPathFinder()
    {
        return new PathFinder(this);
    }

    public bool IsTopWall(int x, int y)
    {
        if (y == Height - 1) return false;
        return map[x, y] && !map[x, y + 1];
    }

    public bool Contains(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public Coord WorldToTile(Vector2 world)
    {
        return new Coord(world - origin);
    }

    public Vector2 TileToWorld(Coord tile)
    {
        return new Vector2(tile.tx, tile.ty) + origin; 
    }

    public Vector2 WorldToLevel(Vector2 world)
    {
        return world - origin;
    }

    public Vector2 LevelToWorld(Vector2 level)
    {
        return level + origin;
    }

    // Performs a brushfire expansion keeping track of all coords that pass the specified filter. Stops after it finds
    // the specified amount (if not specified, the brushfire will go over the entire map
    //
    // NOTE: The start point must be a floor tile, not a wall
    public static List<Coord> FindCoords(Coord start, Func<Coord, bool> filter, bool[,] map, int amount = int.MaxValue)
    {
        List<Coord> coords = new List<Coord>();
        HashSet<Coord> visited = new HashSet<Coord>();
        Queue<Coord> toVisit = new Queue<Coord>();
        toVisit.Enqueue(start);

        int width = map.GetLength(1);
        int height = map.GetLength(0);
        int count = 0;

        while (toVisit.Count > 0 && count < amount)
        {
            Coord coord = toVisit.Dequeue();
            if (filter(coord))
            {
                coords.Add(coord);
                count++;
            }
            visited.Add(coord);

            for (int xx = coord.tx - 1; xx <= coord.tx + 1; xx++)
            {
                for (int yy = coord.ty - 1; yy <= coord.ty + 1; yy++)
                {
                    // Out of bounds
                    if (xx < 0 || xx > width - 1 || yy < 0 || yy > height - 1) continue;

                    // If you're on one of the four adjacent squares (not the center)
                    if (!(xx == coord.tx && yy == coord.ty) && (xx == coord.tx || yy == coord.ty) && !map[xx, yy])
                    {
                        Coord c = new Coord(xx, yy);
                        if (!(visited.Contains(c) || toVisit.Contains(c)))
                        {
                            toVisit.Enqueue(c);
                        }
                    }
                }
            }
        }
        return coords;
    }
}