using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Random = UnityEngine.Random;

class ContourTracer
{
    public Color32 Outline;

    private HashSet<Line> lines;
    private HashSet<Line> unitDiagonals;
    private Dictionary<Vector2, HashSet<Line>> pointMap;
    private HashSet<Vector2> visited;
    private HashSet<Vector2> tInters;
    private HashSet<Vector2> corners;

    private Texture2D rot;
    private Color32[] spritePixels; // Original sprite pixels, DONT MODIFY
    private int spriteWidth;
    private int spriteHeight;
    private bool drawScaled = false;

    public ContourTracer(Texture2D tex, Color32 outline)
    {
        rot = tex;
        Outline = outline;

        lines = new HashSet<Line>();
        unitDiagonals = new HashSet<Line>();
        pointMap = new Dictionary<Vector2, HashSet<Line>>();
        visited = new HashSet<Vector2>();
        tInters = new HashSet<Vector2>();
        corners = new HashSet<Vector2>();

        spritePixels = rot.GetPixels32();
        spriteWidth = rot.width;
        spriteHeight = rot.height;

        CreateLines();
        ReduceLines();

        if(drawScaled)
        {
            Texture2D scaled = new Texture2D(spriteWidth * 4, spriteHeight * 4);
            Color32[] pix = scaled.GetPixels32();

            for (int i = 0; i < pix.Length; i++) pix[i] = Color.white;

            foreach(Line line in lines)
            {
                int x1 = (int)(line.P1.x * 4 + 2);
                int y1 = (int)(line.P1.y * 4 + 2);
                int x2 = (int)(line.P2.x * 4 + 2);
                int y2 = (int)(line.P2.y * 4 + 2);

                if(x1 == x2)
                {
                    if (y1 < y2)
                    {
                        y1 += 1;
                        y2 -= 1;
                    }
                    else
                    {
                        y1 -= 1;
                        y2 += 1;
                    }
                }

                if (y1 == y2)
                {
                    if (x1 < x2)
                    {
                        x1 += 1;
                        x2 -= 1;
                    }
                    else
                    {
                        x1 -= 1;
                        x2 += 1;
                    }
                }

                Color32 random = new Color32(Convert.ToByte(Random.Range(0, 255)), Convert.ToByte(Random.Range(0,255)), Convert.ToByte(Random.Range(0,255)), 255);
                DrawLine(x1, y1, x2, y2, scaled.width, random, pix);
            }

            scaled.SetPixels32(pix);
            scaled.Apply();

            System.IO.File.WriteAllBytes("C:/Users/Scott/Desktop/test.png", scaled.EncodeToPNG());
        }
    }

    /*public void Update()
    {
        Vector2 player = transform.position;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = mouse - player;
        float angle = Mathf.Atan2(diff.y, diff.x);

        text.text = "Degrees: " + (angle * Mathf.Rad2Deg).ToString();

        DrawLines(angle * Mathf.Rad2Deg);
    }*/

    // Fills in the pixels (pixels passed in must be the same size as the Texture passed in the constructor)
    public void DrawLines(float degrees, Color32[] pixels)
    {
        degrees = (int)(degrees / 1) * 1;

        float radians = Mathf.Deg2Rad * degrees;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        float xcenter = spriteWidth / 2f;
        float ycenter = spriteHeight / 2f;

        foreach (Line line in lines)
        {
            Vector2 p1 = line.P1;
            Vector2 p2 = line.P2;

            Vector2 rot1 = MathUtils.RotatePixel((int)p1.x, (int)p1.y, xcenter, ycenter, cos, sin);
            Vector2 rot2 = MathUtils.RotatePixel((int)p2.x, (int)p2.y, xcenter, ycenter, cos, sin);

            DrawLine((int)rot1.x, (int)rot1.y, (int)rot2.x, (int)rot2.y, spriteWidth, Outline, pixels);
        }

        // @Hack Neighborhood() uses the reference to spritePixels, so we have to point it to the rotated pixel array before using
        Color32[] save = spritePixels;
        spritePixels = pixels;
        foreach(Vector2 intersection in tInters)
        {
            Vector2 point = MathUtils.RotatePixel((int)intersection.x, (int)intersection.y, xcenter, ycenter, cos, sin);
            int x = (int)point.x;
            int y = (int)point.y;
            //pixels[x + y * spriteWidth] = Color.red;

            bool[] nb = Neighborhood(x, y, true);
            int groups = 0;
            bool edge = nb[0];
            for (int i = 0; i < nb.Length; i++)
            {
                if(nb[i] != edge)
                {
                    edge = nb[i];
                    if(i == 7 && nb[0] == nb[7])
                    {
                        break;
                    } 
                    groups++;
                }
            }

            if (groups <= 2) pixels[x + y * spriteWidth] = Color.clear;
        }
        
        foreach(Vector2 corner in corners)
        {
            Vector2 point = MathUtils.RotatePixel((int)corner.x, (int)corner.y, xcenter, ycenter, cos, sin);
            int x = (int)point.x;
            int y = (int)point.y;
            //pixels[x + y * spriteWidth] = Color.blue;

            int a = -1;
            int b = -1;
            int n = 0;
            bool[] nb = Neighborhood(x, y, true);
            for(int i = 0; i < nb.Length; i++)
            {
                if(nb[i])
                {
                    n++;
                    if(a == -1)
                    {
                        a = i;
                    }
                    else
                    {
                        b = i;
                    }
                }
            }
            bool together = (a + 1 == b) || (a == 0 && b == 7);
            if (n <= 1 || (n == 2 && together)) pixels[x + y * spriteWidth] = Color.clear;
        }
        spritePixels = save;

        /*tex.SetPixels32(pixels);
        tex.Apply();

        GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 16);*/

        //System.IO.File.WriteAllBytes("C:/Users/bobba/Desktop/geom" + degrees.ToString() + ".png", tex.EncodeToPNG());
    }

    private void CreateLines()
    {
        int w = spriteWidth;
        int h = spriteHeight;

        for(int y = 0; y < h; y++)
        {
            for(int x = 0; x < h; x++)
            {
                Color32 col = spritePixels[x + y * w];
                if (!Eq(col, Outline)) continue;

                Vector2 point = new Vector2(x, y);
                if (visited.Contains(point)) continue;
                AddVisitedPoint(point, true);

                // C1 C2 C3
                // C4    C5
                // C6 C7 C8
                bool c1 = IsEdgePixel(x - 1, y + 1);
                bool c2 = IsEdgePixel(x, y + 1);
                bool c3 = IsEdgePixel(x + 1, y + 1);
                bool c4 = IsEdgePixel(x - 1, y);
                bool c5 = IsEdgePixel(x + 1, y);
                bool c6 = IsEdgePixel(x - 1, y - 1);
                bool c7 = IsEdgePixel(x, y - 1);
                bool c8 = IsEdgePixel(x + 1, y - 1);

                // Convert to numbers
                int[] grid = new int[]
                {
                    c1 ? 1 : 0,
                    c2 ? 1 : 0,
                    c3 ? 1 : 0,
                    c4 ? 1 : 0,
                    c5 ? 1 : 0,
                    c6 ? 1 : 0,
                    c7 ? 1 : 0,
                    c8 ? 1 : 0,
                };

                bool expandCrowded = false;

                // Vertical Expansion

                // Going up
                int n = grid[0] + grid[1] + grid[2] + grid[3] + grid[4];
                int yTop = y;
                if(c2 && (expandCrowded || n < 4))
                {
                    for(int yy = y + 1; yy < h; yy++)
                    {
                        if (!IsEdgePixel(x, yy)) break;
                        if (IsEdgePixel(x, yy + 1)) AddVisitedPoint(new Vector2(x, yy));
                        yTop++;
                    }
                }

                // Going down
                n = grid[3] + grid[4] + grid[5] + grid[6] + grid[7];
                int yBottom = y;
                if (c7 && (expandCrowded || n < 4))
                {
                    for (int yy = y - 1; yy >= 0; yy--)
                    {
                        if (!IsEdgePixel(x, yy)) break;
                        yBottom--;
                        if (IsEdgePixel(x, yy - 1)) AddVisitedPoint(new Vector2(x, yy));
                    }
                }

                // Create line
                if(yTop != yBottom)
                {
                    AddLine(new Line(new Vector2(x, yBottom), new Vector2(x, yTop)));
                }

                // Horizontal Expansion

                // Going right
                n = grid[1] + grid[2] + grid[4] + grid[6] + grid[7];
                int xRight = x;
                if (c5 && (expandCrowded || n < 4))
                {
                    for (int xx = x + 1; xx < w; xx++)
                    {
                        if (!IsEdgePixel(xx, y)) break;
                        xRight++;
                        if(IsEdgePixel(xx + 1, y)) AddVisitedPoint(new Vector2(xx, y));
                    }
                }

                // Going left
                n = grid[0] + grid[1] + grid[3] + grid[5] + grid[6];
                int xLeft = x;
                if (c4 && (expandCrowded || n < 4))
                {
                    for (int xx = x - 1; xx >= 0; xx--)
                    {
                        if (!IsEdgePixel(xx, y)) break;
                        xLeft--;
                        if (IsEdgePixel(xx - 1, y)) AddVisitedPoint(new Vector2(xx, y));
                    }
                }

                // Create line
                if (xLeft != xRight)
                {
                    AddLine(new Line(new Vector2(xLeft, y), new Vector2(xRight, y)));
                }

                // Diagonals

                // Upper left to Lower Right
                Vector2 p1 = new Vector2(x, y);
                Vector2 p2 = new Vector2(x, y);
                while(IsEdgePixel(p1.x - 1, p1.y + 1) && !(IsEdgePixel(p1.x, p1.y + 1) || IsEdgePixel(p1.x - 1, p1.y)))
                {
                    p1.x--;
                    p1.y++;
                    AddVisitedPoint(new Vector2(p1.x, p1.y));
                }

                while (IsEdgePixel(p2.x + 1, p2.y - 1) && !(IsEdgePixel(p2.x, p2.y - 1) || IsEdgePixel(p2.x + 1, p2.y)))
                {
                    p2.x++;
                    p2.y--;
                    AddVisitedPoint(new Vector2(p2.x, p2.y));
                }

                if (p1 != p2)
                {
                    AddLine(new Line(p1, p2));
                    if (p1 != point) visited.Remove(p1);
                    if (p2 != point) visited.Remove(p2);
                }

                // Upper Right to Lower Left
                p1 = new Vector2(x, y);
                p2 = new Vector2(x, y);
                while (IsEdgePixel(p1.x + 1, p1.y + 1) && !(IsEdgePixel(p1.x, p1.y + 1) || IsEdgePixel(p1.x + 1, p1.y)))
                {
                    p1.x++;
                    p1.y++;
                    AddVisitedPoint(new Vector2(p1.x, p1.y));
                }

                while (IsEdgePixel(p2.x - 1, p2.y - 1) && !(IsEdgePixel(p2.x, p2.y - 1) || IsEdgePixel(p2.x - 1, p2.y)))
                {
                    p2.x--;
                    p2.y--;
                    AddVisitedPoint(new Vector2(p2.x, p2.y));
                }

                if (p1 != p2)
                {
                    AddLine(new Line(p1, p2));
                    if (p1 != point) visited.Remove(p1);
                    if (p2 != point) visited.Remove(p2);
                }
            }
        }
    }

    private void ReduceLines()
    {
        List<Line> toRemove = new List<Line>();
        List<Line> toAdd = new List<Line>();
        foreach (Line diag in unitDiagonals)
        {
            if (toRemove.Contains(diag)) continue;
            HashSet<Line> set1 = pointMap[diag.P1];
            HashSet<Line> set2 = pointMap[diag.P2];

            // 0 -> Nothing to do
            // 1 -> Vertical
            // 2 -> Horizontal
            int state = 0;
            Line l1 = default(Line);
            Line l2 = default(Line);

            Vector2 p1 = diag.P1; // Vertex shared with L1
            Vector2 p2 = diag.P2; // Vertex shared with L2

            // Determine if we're going to traverse vertically or horizontally
            foreach (Line line in set1)
            {
                if (line.IsVertical())
                {
                    foreach (Line l in set2)
                    {
                        if (l.IsVertical())
                        {
                            state = 1;
                            l1 = line;
                            l2 = l;
                            break;
                        }
                    }
                    if (state == 1) break;
                }

                if (line.IsHorizontal())
                {
                    foreach (Line l in set2)
                    {
                        if (l.IsHorizontal())
                        {
                            l1 = line;
                            l2 = l;
                            state = 2;
                            break;
                        }
                    }
                    if (state == 2) break;
                }
            }

            // If this diagonal doesn't connect two verticals or two horizontals, then there is nothing to do
            if (state == 0)
            {
                continue;
            }

            bool vertical = state == 1;

            Queue<Vector2> unvsitedPoints = new Queue<Vector2>();
            unvsitedPoints.Enqueue(l1.NonSharedPoint(diag));
            unvsitedPoints.Enqueue(l2.NonSharedPoint(diag));

            List<Line> continuousLine = new List<Line>
            {
                l1,
                l2,
                diag
            };

            float diagSlope = diag.Slope();

            while (unvsitedPoints.Count > 0)
            {
                Vector2 point = unvsitedPoints.Dequeue();
                foreach (Line l in pointMap[point])
                {
                    if (!continuousLine.Contains(l) && (((l.IsVertical() && vertical) || (l.IsHorizontal() && !vertical)) || AlmostEquals(diagSlope, l.Slope())))
                    {
                        continuousLine.Add(l);
                        unvsitedPoints.Enqueue(l.P1 == point ? l.P2 : l.P1);
                        break;
                    }
                }
            }

            // Sort by increasing x value
            continuousLine.Sort(delegate (Line a, Line b)
            {
                float x1 = a.P1.x;
                float x2 = a.P2.x;
                float x3 = b.P1.x;
                float x4 = b.P2.x;

                if ((x1 == x2 && x2 == x3 && x3 == x4) || (x1 == x3 && x2 == x4))
                {
                    return 0;
                }
                if (x1 < x3 || x1 < x4 || x2 < x3 || x2 < x4)
                {
                    return -1;
                }
                return 1;
            });

            for (int i = 0; i < continuousLine.Count; i++) 
            {
                Line line = continuousLine[i];
                if (!unitDiagonals.Contains(line)) continue;
                if(i == 0 || i == continuousLine.Count - 1)
                {
                    Line nextLine = continuousLine[i == 0 ? i + 1 : i - 1];
                    if (ContainsTInter(nextLine, true)) continue;
                    if (pointMap[line.SharedPoint(nextLine)].Count >= 3) continue; // If this point connects more than 2 lines, then we don't want to bend

                    continuousLine.RemoveRange(i == 0 ? i : i - 1, 2);
                    continuousLine.Insert(i == 0 ? i : i - 1, new Line(line.NonSharedPoint(nextLine), nextLine.NonSharedPoint(line)));
                    toRemove.Add(line);
                    toRemove.Add(nextLine);
                    i--;
                    continue;
                }

                Line start = continuousLine[i - 1];
                Line end = continuousLine[i + 1];

                if (ContainsTInter(start, true) || ContainsTInter(end, true)) continue;
                if (pointMap[line.SharedPoint(start)].Count >= 3 || pointMap[line.SharedPoint(end)].Count >= 3) continue;

                Vector2 midpoint = line.MidPoint();

                Line first = new Line(start.NonSharedPoint(line), midpoint);
                Line second = new Line(midpoint, end.NonSharedPoint(line));
                continuousLine.RemoveRange(i - 1, 3);
                continuousLine.Insert(i - 1, second);
                continuousLine.Insert(i - 1, first);
                toRemove.Add(start);
                toRemove.Add(line);
                toRemove.Add(end);
                i--; // Because we lost a line
            }
            //Smooth(continuousLine, ref toRemove);
            //Smooth(continuousLine, ref toRemove);

            foreach (Line line in continuousLine)
            {
                toAdd.Add(line);
            }

            /*float sqr1 = l1.SqrLength();
            float sqr2 = l2.SqrLength();

            // If the length of the two lines are not equal, combine the shorter line with the UD
            if(!AlmostEquals(sqr1, sqr2))
            {
                Line newLine = default(Line);
                if(l1.SqrLength() < l2.SqrLength())
                {
                    newLine = new Line(l1.P1 == p1 ? l1.P2 : l1.P1, p2);
                    RemoveLine(l1);
                }
                else
                {
                    newLine = new Line(l2.P1 == p2 ? l2.P2 : l2.P1, p1);
                    RemoveLine(l2);
                }
                AddLine(newLine);
                toRemove.Add(diag);
            }
            else
            {
                // Get the start and end points of the longer line
                Vector2 start = l1.P1 == p1 ? l1.P2 : l1.P1;
                Vector2 end   = l2.P1 == p2 ? l2.P2 : l2.P1;
                toRemove.Add(diag);
                toRemove.Add(l1);
                toRemove.Add(l2);

                float len = l1.SqrLength();
                float diagSlope = diag.Slope();

                Extend(l1, ref start, diagSlope, len, toRemove);
                Extend(l2, ref end, diagSlope, len, toRemove);

                AddLine(new Line(start, end));
            }*/
        }

        foreach(Line line in toAdd)
        {
            AddLine(line);
        }

        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            RemoveLine(toRemove[i]);
        }

    }

    private void Smooth(List<Line> lines, ref List<Line> toRemove)
    {
        if (lines.Count == 1) return;

        for(int i = 0; i < lines.Count - 1; i++)
        {
            Line first = lines[i];
            Line second = lines[i + 1];

            if (first.IsVertical() || first.IsHorizontal() || second.IsVertical() || second.IsHorizontal()) continue;

            Vector2 p1 = first.NonSharedPoint(second);
            Vector2 p2 = first.SharedPoint(second);
            Vector2 p3 = second.NonSharedPoint(first);

            Vector2 q0 = 0.75f * p1 + 0.25f * p2;
            Vector2 r0 = 0.25f * p1 + 0.75f * p2;
            Vector2 q1 = 0.75f * p2 + 0.25f * p3;
            Vector2 r1 = 0.25f * p2 + 0.75f * p3;

            Line l1 = new Line(p1, r0);
            Line l2 = new Line(r0, q1);
            Line l3 = new Line(q1, p3);

            toRemove.Add(first);
            toRemove.Add(second);

            lines.RemoveRange(i, 2);
            lines.Insert(i, l3);
            lines.Insert(i, l2);
            lines.Insert(i, l1);
            i++;
        }

    }

    private bool ContainsTInter(Line line, bool onlyConsiderMiddle)
    {
        foreach(Vector2 point in tInters)
        {
            if (line.Contains(point))
            {
                if (onlyConsiderMiddle && (line.P1 == point || line.P2 == point)) continue;
                return true;
            }
        }
        return false;
    }

    private void Extend(Line extend, ref Vector2 toExtend, float diagSlope, float len, List<Line> toRemove)
    {
        while (AlmostEquals(extend.SqrLength(), len))
        {
            // Get list of lines connected to start point, check for Unit Diagonal
            // If no unit diagonal exists, then there is nothing to do
            HashSet<Line> set = pointMap[toExtend];
            Line ud = default(Line);
            bool containsUD = false;
            foreach (Line l in set)
            {
                if (unitDiagonals.Contains(l) && AlmostEquals(l.Slope(), diagSlope))
                {
                    containsUD = true;
                    ud = l;
                    break;
                }
            }
            if (!containsUD) break;

            Vector2 point = ud.SharedPoint(extend);
            set = pointMap[point];

            bool found = false;
            Line old = default(Line);
            foreach (Line l in set)
            {
                if (l != extend && AlmostEquals(l.SqrLength(), len))
                {
                    old = extend;
                    extend = l;
                    found = true;
                    break;
                }
            }

            toRemove.Add(ud);
            toExtend = ud.NonSharedPoint(extend);

            // If we found a another line with the right slope, then we can iterate again
            if (found)
            {
                toRemove.Add(old);
                continue;
            }
            else
            {
                break;
            }
        }
    }

    // If CW is true, the neighborhood will be return in CW order
    // CW:
    // C1 C2 C3
    // C8    C4
    // C7 C6 C5
    // 
    // Normal:
    // C1 C2 C3
    // C4    C5
    // C6 C7 C8
    public bool[] Neighborhood(int x, int y, bool cw)
    {
        if(cw)
        {
            return new bool[]
            {
                IsEdgePixel(x - 1, y + 1),
                IsEdgePixel(x, y + 1),
                IsEdgePixel(x + 1, y + 1),
                IsEdgePixel(x + 1, y),
                IsEdgePixel(x + 1, y - 1),
                IsEdgePixel(x, y - 1),
                IsEdgePixel(x - 1, y - 1),
                IsEdgePixel(x - 1, y),
            };
        }
        else
        {
            return new bool[]
            {
                IsEdgePixel(x - 1, y + 1),
                IsEdgePixel(x, y + 1),
                IsEdgePixel(x + 1, y + 1),
                IsEdgePixel(x - 1, y),
                IsEdgePixel(x + 1, y),
                IsEdgePixel(x - 1, y - 1),
                IsEdgePixel(x, y - 1),
                IsEdgePixel(x + 1, y - 1),
            };
        }
    }

    private void AddLine(Line line)
    {
        lines.Add(line);

        // We have a unit diagonal line
        if((AlmostEquals(line.Slope(), 1f) || AlmostEquals(line.Slope(), -1f)) && AlmostEquals(line.SqrLength(), 2f))
        {
            unitDiagonals.Add(line);
        }
        
        AddLineInternal(line.P1, line);
        AddLineInternal(line.P2, line);
    }

    private void RemoveLine(Line line)
    {
        lines.Remove(line);
        unitDiagonals.Remove(line);
        if(pointMap.ContainsKey(line.P1)) pointMap[line.P1].Remove(line);
        if(pointMap.ContainsKey(line.P2)) pointMap[line.P2].Remove(line);
    }

    private void AddLineInternal(Vector2 point, Line line)
    {
        if(!pointMap.ContainsKey(point))
        {
            pointMap.Add(point, new HashSet<Line>());
        }
        pointMap[point].Add(line);
    }

    private void AddVisitedPoint(Vector2 point, Vector2 ignorePoint)
    {
        if (point == ignorePoint) return;
        AddVisitedPoint(point);
    }

    private void AddVisitedPoint(Vector2 point)
    {
        AddVisitedPoint(point, false);
    }

    private void AddVisitedPoint(Vector2 point, bool checkSpecial)
    {
        // Must be an intersection
        if(visited.Contains(point) || checkSpecial)
        {
            int x = (int)point.x;
            int y = (int)point.y;
            int n = 0;
            bool[] nb = Neighborhood(x, y, false);

            // If none of the corners are filled in, then it could be a special point
            if (!(nb[0] || nb[2] || nb[5] || nb[7]))
            {
                if (nb[1]) n++;
                if (nb[3]) n++;
                if (nb[6]) n++;
                if (nb[4]) n++;
                if(n == 3)
                {
                    tInters.Add(new Vector2(point.x, point.y));
                }
                else if(n == 2 && nb[1] != nb[6])
                {
                    corners.Add(new Vector2(point.x, point.y));
                }
            }
        }
        visited.Add(point);
    }

    private bool IsEdgePixel(float x, float y)
    {
        return IsEdgePixel((int)x, (int)y);
    }

    private bool IsEdgePixel(int x, int y)
    {
        return Out(x, y) ? false : Eq(spritePixels[x + y * spriteWidth], Outline);
    }

    private bool Out(int x, int y)
    {
        return x < 0 || x >= spriteWidth || y < 0 || y >= spriteHeight;
    }

    /*private void Moore()
    {
        int infinite = 10000;
        Texture2D tex = Sprite.texture;

        Color32[] pixels = tex.GetPixels32();
        Color32[] newPixels = new Color32[pixels.Length];
        pixels.CopyTo(newPixels, 0);

        int pathNum = 0;
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                Color32 col = pixels[x + y * tex.width];
                if (!Eq(col, Outline)) continue;

                Vector2 center = new Vector2(x, y);
                if (visited.Contains(center)) continue;

                List<Vector2> path = new List<Vector2>();
                Vector2 pathPrev = new Vector2(-1, -1); // Impossible
                Vector2 pathCurr = center; 
                path.Add(pathCurr);

                Vector2 p1 = center;
                Vector2 p2 = center;

                MoorePoint next = MoorePoint.P8;
                bool atStart = false;

                // When neighborhood is 0, that means we've checked all points around the neighborhood (aka we're at a lone pixel)
                int neighborhood = 8;

                while (!atStart && infinite > 0)
                {
                    infinite--;

                    next = GetNext(next);
                    Vector2 nextPoint = GetPoint(center, next);
                    neighborhood--;

                    // Don't go to points that are out of bounds or have already been visited
                    if (nextPoint.x < 0 || nextPoint.x >= tex.width || nextPoint.y < 0 || nextPoint.y >= tex.height) continue;
                    if (visited.Contains(nextPoint)) continue;

                    if (Eq(pixels[(int)nextPoint.x +(int)nextPoint.y * tex.width], Outline) || neighborhood <= 0)
                    {
                        MoorePoint prev = GetPrev(next);

                        pathPrev = pathCurr;
                        pathCurr = nextPoint;

                        // If we've already visited the nextPoint, and in the path there is a nextPoint where the prev point exists one index before it
                        // Then this path is complete
                        if((path.Contains(pathCurr) && path.IndexOf(pathCurr) > 0 && path[path.IndexOf(pathCurr) - 1] == pathPrev) || neighborhood <= 0)
                        {
                            Line line = new Line(p1, p2);

                            // Skip lines that are two pixels and slope one
                            //if (!((line.Slope() == -1 || line.Slope() == 1) && Mathf.Abs(line.P1.x - line.P2.x) <= 1))
                            //{
                                // Close off line, create new one
                                Debug.Log(string.Format("Line From {0} to {1}", p1, p2));
                                lines.Add(line);
                            //}
                            
                            Debug.Log("Path Complete!");
                            StringBuilder builder = new StringBuilder();
                            foreach(Vector2 vec in path)
                            {
                                builder.AppendFormat("{0}, ", vec);
                            }
                            Debug.Log(builder.ToString());
                            break;
                        }

                        Vector2 prevPoint = GetPoint(center, prev);
                        center = new Vector2(nextPoint.x, nextPoint.y);
                        next = RelativeMoore(center, prevPoint);

                        // Update path
                        path.Add(pathCurr);
                        neighborhood = 8; // Reset the neighborhood

                        bool retracing = path.Count > 2 && path[path.Count - 3] == path[path.Count - 1];

                        // Lines
                        if(p1 == p2)
                        {
                            p2 = center;
                        }
                        else
                        {
                            if(Match(p1, p2, center) && !retracing)
                            {
                                p2 = center; // Update the end point
                            }
                            else
                            {
                                Line line = new Line(p1, p2);

                                // Skip lines that are two pixels and slope one
                                //if (!((line.Slope() == -1 || line.Slope() == 1) && Mathf.Abs(line.P1.x - line.P2.x) <= 1))
                                //{
                                    // Close off line, create new one
                                    Debug.Log(string.Format("Line From {0} to {1}", p1, p2));
                                    lines.Add(line);
                                //}
                                //else
                                //{
                                    Debug.Log(string.Format("Skipping line from {0} to {1}", p1, p2));
                                //}

                                p1 = p2;
                                p2 = center;
                            }
                        }
                    }
                }

                // Updated visited set
                //Color32 random = new Color32(Convert.ToByte(Random.Range(0, 255)), Convert.ToByte(Random.Range(0,255)), Convert.ToByte(Random.Range(0,255)), 255);
                Color32 c;
                switch(pathNum)
                {
                    case 0:
                        c = Color.red; 
                        break;
                    case 2:
                        c = Color.blue;
                        break;
                    case 3:
                        c = Color.green;
                        break;
                    case 4:
                        c = Color.cyan;
                        break;
                    case 5:
                        c = Color.magenta;
                        break;
                    case 6:
                        c = Color.yellow;
                        break;
                    default:
                        c = Color.black;
                        break;
                }
                foreach (Vector2 item in path)
                {
                    newPixels[(int)item.x + (int)item.y * tex.width] = c;
                    visited.Add(item);
                }
                pathNum++;
            }
        }

        Texture2D output = new Texture2D(tex.width, tex.height);
        output.SetPixels32(newPixels);
        output.Apply();

        System.IO.File.WriteAllBytes("C:/Users/bobba/Desktop/output.png", output.EncodeToPNG());
    }*/

    private void DrawLine(int x, int y, int x2, int y2, int texWidth, Color32 color, Color32[] pixels)
    { 
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Math.Abs(w);
        int shortest = Math.Abs(h);
        if (!(longest > shortest))
        {
            longest = Math.Abs(h);
            shortest = Math.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            pixels[x + y * texWidth] = color;
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
    }

    // Returns true if the slope from p1 to p2 is the same as from p1 to p3
    private bool Match(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        bool p12Vert = p1.x == p2.x;
        bool p13Vert = p1.x == p3.x;

        // If both are vertical lines, then we're good
        if (p12Vert && p13Vert) return true;

        // If one is vertical, but not the other, then no bueno
        else if ((p12Vert && !p13Vert) || (!p12Vert && p13Vert)) return false;

        float p12Slope = (p1.y - p2.y) / (p1.x - p2.x);
        float p13Slope = (p1.y - p3.y) / (p1.x - p3.x);

        return Mathf.Abs(p12Slope - p13Slope) < float.Epsilon;
    }

    private bool Eq(Color32 c1, Color32 c2)
    {
        int tolerance = 1;
        return Math.Abs(c1.r - c2.r) <= tolerance &&
               Math.Abs(c1.g - c2.g) <= tolerance &&
               Math.Abs(c1.b - c2.b) <= tolerance &&
               Math.Abs(c1.a - c2.a) <= tolerance;
    }

    /*
    private MoorePoint GetNext(MoorePoint p)
    {
        int index = (int)p;
        index++;
        if (index > 7) index = 0;
        return (MoorePoint)index;
    }

    private MoorePoint GetPrev(MoorePoint p)
    {
        int index = (int)p;
        index--;
        if (index < 0) index = 7;
        return (MoorePoint)index;
    }

    private MoorePoint RelativeMoore(Vector2 center, Vector2 neighborhood)
    {
        return GetMoore(neighborhood - center);
    }

    private MoorePoint GetMoore(Vector2 p)
    {
        foreach (MoorePoint moore in Enum.GetValues(typeof(MoorePoint)))
        {
            if (p == GetPoint(moore)) return moore;
        }
        return MoorePoint.P1;
    }

    private Vector2 GetPoint(Vector2 center, MoorePoint moore)
    {
        Vector2 moorePoint = GetPoint(moore);
        return center + moorePoint;
    }

    // Neighborhood point relative to center
    private Vector2 GetPoint(MoorePoint p)
    {
        switch (p)
        {
            case MoorePoint.P1:
                return new Vector2(-1, 1);
            case MoorePoint.P2:
                return new Vector2(0, 1);
            case MoorePoint.P3:
                return new Vector2(1, 1);
            case MoorePoint.P4:
                return new Vector2(1, 0);
            case MoorePoint.P5:
                return new Vector2(1, -1);
            case MoorePoint.P6:
                return new Vector2(0, -1);
            case MoorePoint.P7:
                return new Vector2(-1, -1);
            case MoorePoint.P8:
                return new Vector2(-1, 0);
        }
        return new Vector2();
    }

    // P1 P2 P3
    // P8 C  P4
    // P7 P6 P5
    private enum MoorePoint
    {
        P1,
        P2,
        P3,
        P4,
        P5,
        P6,
        P7,
        P8
    }*/

    public struct Line
    {
        public Vector2 P1;
        public Vector2 P2;

        public Line(Vector2 p1, Vector2 p2)
        {
            P1 = new Vector2(p1.x, p1.y);
            P2 = new Vector2(p2.x, p2.y);
        }

        public float SqrLength()
        {
            return (P1.x - P2.x) * (P1.x - P2.x) + (P1.y - P2.y) * (P1.y - P2.y);
        }

        public float Slope()
        {
            if (P1.x == P2.x) return float.PositiveInfinity;
            return (P1.y - P2.y) / (P1.x - P2.x);
        }

        public bool IsVertical()
        {
            return AlmostEquals(P1.x, P2.x);
        }

        public bool IsHorizontal()
        {
            return AlmostEquals(P1.y, P2.y);
        }

        public Vector2 SharedPoint(Line line)
        {
            if (P1 == line.P1 || P1 == line.P2) return P1;
            if (P2 == line.P1 || P2 == line.P2) return P2;
            return default(Vector2);
        }

        public Vector2 NonSharedPoint(Line line)
        {
            if (P1 != line.P1 && P1 != line.P2) return P1;
            if (P2 != line.P1 && P2 != line.P2) return P2;
            return default(Vector2);
        }

        public Vector2 MidPoint()
        {
            return (P1 + P2) / 2;
        }

        public bool Contains(Vector2 point)
        {
            if (IsVertical())
            {
                if (point.x != P1.x) return false;
                return (point.y <= P1.y && point.y >= P2.y) || (point.y <= P2.y && point.y >= P1.y);
            }

            float x1 = Math.Min(P1.x, P2.x);
            float x2 = Math.Max(P1.x, P2.x);

            if (point.x < x1 || point.x > x2) return false;

            // y - y1 = m(x-x1)
            // y = mx - mx1 + y1
            float m = Slope();
            float x = P1.x;
            float y = P1.y;
            return AlmostEquals((m * point.x - m * x + y), point.y);
        }

        public override bool Equals(object obj)
        {
            return obj is Line && this == (Line)obj;
        }

        public override int GetHashCode()
        {
            return P1.GetHashCode() ^ P2.GetHashCode();
        }

        public static bool operator ==(Line x, Line y)
        {
            return (x.P1 == y.P1 && x.P2 == y.P2) || (x.P1 == y.P2 && x.P2 == y.P1);
        }

        public static bool operator !=(Line x, Line y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return string.Format("{0} to {1}", P1, P2);
        }
    }

    private static bool AlmostEquals(float f1, float f2)
    {
        return Math.Abs(f1 - f2) < 0.00001f;
    }

}
