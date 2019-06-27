using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// How To Use:
// ------------
// 1. Attach this script to the object that you want to rotate (it must have a SpriteRenderer attached to it)
// 2. Call SetRotate(degrees) to rotate the sprite
// 3. If you use the SetRotate method with a pivot, use the referenced Vector3 to translate your sprite after the rotation
//    by adding the result to the sprite's transform.localPosition

[RequireComponent(typeof(SpriteRenderer))]
public class PixelRotate : MonoBehaviour {

    public Color Outline;
    public Vector2 PivotCenter { get; private set; } // Represents the center of the sprite when rotating

    private ContourTracer tracer;
    private bool usingTracer = false;
    private SpriteRenderer spriteRenderer;
    private Texture2D referenceTexture; // The 8x scaled image used for sampling during the rotation
    private Texture2D spriteTexture; // The final image that gets set on the sprite
    private float Angle { get; set; } // I don't understand C# lol

    // Width and Height in pixels of original sprite
    // We can use this for rotating around a pivot
    private int originalWidth;
    private int originalHeight;
    private int xPad;
    private int yPad;

    // Used when scaling
    private Rect scaledTexBounds;

    // Use this for initialization
    void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null) return;
        UpdateTexture(true);
    }

    // Call this after updating the sprite being used by the sprite renderer
    public void UpdateTexture(bool addPadding)
    { 
        Sprite original = spriteRenderer.sprite;

        int xoff = (int)original.rect.x;
        int yoff = (int)original.rect.y;

        int totalWidth = (int)original.texture.width;
        int totalHeight = (int)original.texture.height;
        int spriteWidth = (int)original.rect.width;
        int spriteHeight = (int)original.rect.height;

        originalWidth = spriteWidth;
        originalHeight = spriteHeight;
        PivotCenter = new Vector2(originalWidth / 2, originalHeight / 2);

        // Get temporary render texture to get access to sprite pixels
        RenderTexture tmp = RenderTexture.GetTemporary(totalWidth, totalHeight, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(original.texture, tmp);
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = tmp;

        if(addPadding)
        {
            // Calculate diagonal to ensure corners aren't cuttoff when rotating
            float diag = Mathf.Sqrt(spriteWidth * spriteWidth + spriteHeight * spriteHeight);
            int iDiag = Mathf.RoundToInt(diag + 0.5f); // Round up
            iDiag += 2; // This is so that the contour tracer doesn't try to draw lines that are out of bounds

            // Make iDiag even to ensure centering works
            if (iDiag % 2 == 1) iDiag++;

            // Represent the lower left corner of where the original sprite sits on the padded texture
            xPad = (iDiag - originalWidth) / 2;
            yPad = (iDiag - originalHeight) / 2;

            referenceTexture = new Texture2D(iDiag, iDiag);
        }
        else
        {
            referenceTexture = new Texture2D(originalWidth, originalHeight);
            xPad = 0;
            yPad = 0;
        }
        referenceTexture.filterMode = FilterMode.Point;

        // Set background to transparent
        Color transparent = new Color(0, 0, 0, 0);
        Color[] colors = referenceTexture.GetPixels();
        for(int i = 0; i < colors.Length; i++)
        {
            colors[i] = transparent;
        }
        referenceTexture.SetPixels(colors);

        // Draw the sprite to the reference texture
        referenceTexture.ReadPixels(original.rect, referenceTexture.width / 2 - spriteWidth / 2, referenceTexture.height / 2 - spriteHeight / 2);
        referenceTexture.Apply();

        RenderTexture.active = old;
        RenderTexture.ReleaseTemporary(tmp);

        // The texture that's attached to the sprite. Copy the reference texture (unscaled) to the sprite texturer
        spriteTexture = new Texture2D(referenceTexture.width, referenceTexture.height);
        spriteTexture.filterMode = FilterMode.Point;
        spriteTexture.SetPixels32(referenceTexture.GetPixels32());
        spriteTexture.Apply();

        var piv = original.pivot + new Vector2(xPad, yPad);
        piv.x /= referenceTexture.width;
        piv.y /= referenceTexture.height;

        // The sprite is now no longer attached to a spritesheet and is instead the isolated SpriteTexture
        spriteRenderer.sprite = Sprite.Create(
            spriteTexture,
            new Rect(0, 0, spriteTexture.width, spriteTexture.height),
            piv,
            16
        );

        // Scale for a total of 8x (note: this only needs to be done once, so no performance is lost. The only
        // issue is memory, but for small sprites this isn't an issue)
        scaledTexBounds = new Rect(xPad, yPad, originalWidth + 1, originalHeight + 1);
        Scale2x();
        Scale2x();
        Scale2x();

        if (Outline != default(Color))
        {
            tracer = new ContourTracer(spriteTexture, Outline);
            usingTracer = true;
        }
    }

    // Rotates the sprite about its center by the specified number of degrees
    public void SetRotate(float degrees)
    {
        Vector3 trash = new Vector3();
        SetRotate(degrees, new Vector2(originalWidth / 2, originalHeight / 2), out trash);
    }

    // Pivot represents a PIXEL position on the ORIGINAL sprite
    // Out Vector local represents the translation in transform local space
    // Add local to your sprite transform after the rotation is finished
    public void SetRotate(float degrees, Vector2 pivot, out Vector3 local)
    {
        /*if (true)
        {
            local = Vector3.zero;
            return;
        }*/
        Angle = degrees;
        degrees = (int)(degrees / 2) * 2;


        // Negate the degrees to get a counter-clockwise rotation
        // This algorithm rotates clockwise by default, so we 
        // need a negative angle to rotate CCW properly.
        float radians = -Mathf.Deg2Rad * degrees;

        int width = spriteTexture.width;
        int height = spriteTexture.height;
        float xcenter = width / 2f;
        float ycenter = height / 2f;

        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        Color32[] pixels = referenceTexture.GetPixels32();
        Color32[] newPixels = spriteTexture.GetPixels32();

        Color32 transparent = new Color32(0, 0, 0, 0);
        
        // Checking for the best offset is too slow, so instead we use 4 in both the x and y as the rotation offset
        int xOff = 4;
        int yOff = 4;

        if (usingTracer)
        {
            HashSet<Vector2> secondPassPoints = new HashSet<Vector2>();
            for (int i = 0; i < newPixels.Length; i++) newPixels[i] = transparent;
            tracer.DrawLines(degrees, newPixels);

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    Vector2 rot = MathUtils.RotatePixel(x, y, xcenter, ycenter, cos, sin);

                    int xx = (int)rot.x;
                    int yy = (int)rot.y;

                    if (xx < 0 || xx >= width || yy < 0 || yy >= height) continue;
                    if (newPixels[x + y * width] == Outline) continue;


                    Color32 col = pixels[xx * 8 + xOff + ((yy * 8 + yOff) * width * 8)];
                    if (col != Outline)
                    {
                        newPixels[x + y * width] = col;
                        //continue;
                    }
                    else
                    {
                        secondPassPoints.Add(new Vector2(x, y));
                    }
                    continue;

                    /*int dx = 1;
                    int dy = 0;

                    // length of current segment
                    int segmentLength = 1;

                    // current position (x, y) and how much of current segment we passed
                    int x0 = xOff;
                    int y0 = yOff;
                    int segmentPassed = 0;
                    for (int n = 0; n < 64; n++)
                    {
                        col = pixels[xx * 8 + x0 + ((yy * 8 + y0) * width * 8)];
                        if (col != Outline)
                        {
                            newPixels[x + y * width] = col;
                            break;
                        }

                        // make a step, add 'direction' vector (dx, dy) to current position (x, y)
                        x0 += dx;
                        y0 += dy;
                        segmentPassed++;

                        if (segmentPassed == segmentLength)
                        {
                            // done with current segment
                            segmentPassed = 0;

                            // 'rotate' directions
                            int buffer = dx;
                            dx = -dy;
                            dy = buffer;

                            // increase segment length if necessary
                            if (dy == 0)
                            {
                                segmentLength++;
                            }
                        }
                    }*/
                }
            }

            foreach (Vector2 point in secondPassPoints)
            {
                List<Vector2> l = new List<Vector2>
                {
                    new Vector2(point.x - 1, point.y - 1),
                    new Vector2(point.x, point.y - 1),
                    new Vector2(point.x + 1, point.y - 1),
                    new Vector2(point.x - 1, point.y),
                    new Vector2(point.x + 1, point.y),
                    new Vector2(point.x - 1, point.y + 1),
                    new Vector2(point.x, point.y + 1),
                    new Vector2(point.x + 1, point.y + 1),
                };

                Dictionary<Color32, int> count = new Dictionary<Color32, int>();

                int most = 0;
                Color32 bestCol = default(Color32);
                int numOutline = 0;

                foreach (Vector2 neighbor in l)
                {
                    int nx = (int)neighbor.x;
                    int ny = (int)neighbor.y;
                    Color32 col = newPixels[nx + ny * width];
                    if (col == Outline)
                    {
                        numOutline++;
                        continue;
                    }
                    if(!count.ContainsKey(col))
                    {
                        count.Add(col, 0);
                    }
                    count[col]++;

                    if(count[col] > most || (count[col] >= most && bestCol == Color.clear))
                    {
                        most = count[col];
                        bestCol = col;
                    }
                }
                int x = (int)point.x;
                int y = (int)point.y;

                if(bestCol == Color.clear && numOutline > most)
                {
                    Color32 c = default(Color32);
                    foreach(Color32 key in count.Keys)
                    {
                        if (key == Outline || key == Color.clear) continue;
                        c = key;
                        break;
                    }
                    newPixels[x + y * width] = c;
                }
                else
                {
                    newPixels[x + y * width] = bestCol;
                }

                /*
                Vector2 random = l[Random.Range(0, l.Count)];
                while(secondPassPoints.Contains(random) || (newPixels[(int)random.x + (int)random.y * width] == Outline))
                {
                    l.Remove(random);
                    random = l[Random.Range(0, l.Count)];
                }
                int xx = (int)random.x;
                int yy = (int)random.y;
                newPixels[x + y * width] = newPixels[xx + yy * width];
                newPixels[x + y * width] = Color.white;
                */
            }
        }
        else
        {

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Make sure all pixels have a color
                    newPixels[x + y * width] = new Color(0, 0, 0, 0);

                    // Rotate around center point
                    int xx = (int)(((x - xcenter + 0.5f) * cos + (y - ycenter + 0.5f) * -sin) + xcenter);
                    int yy = (int)(((x - xcenter + 0.5f) * sin + (y - ycenter + 0.5f) * cos) + ycenter);

                    // Make sure the rotated point falls within the bounds of the texture
                    // Note: Because we padded the original texture with transparent borders, 
                    // no pixel on the original sprite will fall outside the bounds of the texture. 
                    if (xx >= 0 && xx < width && yy >= 0 && yy < height)
                    {
                        newPixels[x + y * width] = pixels[xx * 8 + xOff + ((yy * 8 + yOff) * width * 8)];
                    }
                }
            }
        }

        // Apply the pixels and reset the sprite
        spriteTexture.SetPixels32(newPixels);
        spriteTexture.Apply();

        var piv = spriteRenderer.sprite.pivot;
        piv.x /= spriteTexture.width;
        piv.y /= spriteTexture.height;
        spriteRenderer.sprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), piv, 16);

        // Pivot adjustment
        int xPiv = (int)pivot.x;
        int yPiv = (int)pivot.y;

        // Translate to padded texture
        xPiv += xPad;
        yPiv += yPad;

        Sprite sprite = spriteRenderer.sprite;

        // Where the pivot is in sprite coords
        float xPos = (xPiv / (float)width) * sprite.bounds.extents.x * 2 - sprite.bounds.extents.x;
        float yPos = (yPiv / (float)height) * sprite.bounds.extents.y * 2 - sprite.bounds.extents.y;

        // Negate back to original angle (we need true CCW rotation which is why we negate the angle again)
        radians = -radians;
        cos = Mathf.Cos(radians);
        sin = Mathf.Sin(radians);

        // See where the rotation takes the pivot
        int xRot = (int)(((xPiv - xcenter + 0.5f) * cos + (yPiv - ycenter + 0.5f) * -sin) + xcenter);
        int yRot = (int)(((xPiv - xcenter + 0.5f) * sin + (yPiv - ycenter + 0.5f) * cos) + ycenter);

        // Piv coords now transformed into sprite coords
        float xSprite = (xRot / (float)width) * sprite.bounds.extents.x * 2 - sprite.bounds.extents.x;
        float ySprite = (yRot / (float)height) * sprite.bounds.extents.y * 2 - sprite.bounds.extents.y;
        
        // Return the difference between where the pivot should be and where it ended up
        local = new Vector3(xPos - xSprite, yPos - ySprite, 0);
    }

    public float GetAngle()
    {
        return Angle;
    }

    // Scaling algorithm used in RotSprite 
    // See link below for documentation on how this algorithm works
    // https://github.com/alteredgenome/grafx2/issues/385
    private void Scale2x()
    {
        int width = referenceTexture.width;
        int height = referenceTexture.height;

        Texture2D scaledTex = new Texture2D(width * 2, height * 2);
        scaledTex.filterMode = FilterMode.Point;
        Color32[] colors = referenceTexture.GetPixels32();
        Color32[] scaledCols = scaledTex.GetPixels32();

        int scaledWidth = width * 2;

        Color32 transparent = new Color32(0, 0, 0, 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int x1 = x * 2;
                int x2 = x1 + 1;
                int y1 = y * 2;
                int y2 = y1 + 1;
                
                // If we're on a pixel that's apart of the padding, then we don't have to calculate the scale
                if (x < scaledTexBounds.x - 1 || x > scaledTexBounds.x + scaledTexBounds.width ||
                    y < scaledTexBounds.y - 1 || y > scaledTexBounds.y + scaledTexBounds.height)
                {
                    // Set 4 corresponding pixels to blank
                    scaledCols[x1 + y1 * scaledWidth] = new Color32(0, 0, 0, 0);
                    scaledCols[x2 + y1 * scaledWidth] = new Color32(0, 0, 0, 0);
                    scaledCols[x1 + y2 * scaledWidth] = new Color32(0, 0, 0, 0);
                    scaledCols[x2 + y2 * scaledWidth] = new Color32(0, 0, 0, 0);
                    continue;
                }

                // Obtain 9 neighbor pixels
                // cA cB cC
                // cD cE cF
                // cG cH cI

                bool left = x - 1 >= 0;
                bool right = x + 1 < width;
                bool bottom = y - 1 >= 0;
                bool top = y + 1 < height;

                Color32 cA = top && left ? colors[x - 1 + (y + 1) * width] : transparent;
                Color32 cB = top ? colors[x + (y + 1) * width] : transparent;
                Color32 cC = top && right ? colors[x + 1 + (y + 1) * width] : transparent;
                Color32 cD = left ? colors[x - 1 +  y * width] : transparent;
                Color32 cE = colors[x + y * width];
                Color32 cF = right ? colors[x + 1 + y * width] : transparent;
                Color32 cG = bottom && left ? colors[x - 1 + (y - 1) * width] : transparent;
                Color32 cH = bottom ? colors[x + (y - 1) * width] : transparent;
                Color32 cI = bottom && right ? colors[x + 1 + (y - 1) * width] : transparent;

                // Outputs
                // oA oB
                // oC oD
                Color32 oA;
                Color32 oB;
                Color32 oC;
                Color32 oD;

                if (different(cD, cF, cE) && different(cB, cH, cE) &&
                    ((similar(cE, cD, cE) || similar(cE, cH, cE) || similar(cE, cF, cE) || similar(cE, cB, cE) ||
                    ((different(cA, cI, cE) || similar(cE, cG, cE) || similar(cE, cC, cE)) &&
                    (different(cG, cC, cE) || similar(cE, cI, cE) || similar(cE, cA, cE))))))
                {
                    oA = ((similar(cB, cD, cE) && (different(cE, cA, cE) && (different(cE, cA, cE) || different(cE, cI, cE) || different(cB, cC, cE) || different(cD, cG, cE)))) ? cB : cE);
                    oB = ((similar(cB, cF, cE) && (different(cE, cC, cE) && (different(cE, cC, cE) || different(cE, cG, cE) || different(cF, cI, cE) || different(cB, cA, cE)))) ? cF : cE);
                    oC = ((similar(cD, cH, cE) && (different(cE, cG, cE) && (different(cE, cG, cE) || different(cE, cC, cE) || different(cD, cA, cE) || different(cH, cI, cE)))) ? cD : cE);
                    oD = ((similar(cH, cF, cE) && (different(cE, cI, cE) && (different(cE, cI, cE) || different(cE, cA, cE) || different(cH, cG, cE) || different(cF, cC, cE)))) ? cH : cE);

                }
                else
                {
                    oA = cE;
                    oB = cE;
                    oC = cE;
                    oD = cE;
                }

                
                scaledCols[x1 + y2 * scaledWidth] = oA; // oA
                scaledCols[x2 + y2 * scaledWidth] = oB; // oB
                scaledCols[x1 + y1 * scaledWidth] = oC; // oC
                scaledCols[x2 + y1 * scaledWidth] = oD; // oD
            }
        }

        scaledTex.SetPixels32(scaledCols);
        scaledTex.Apply();

        referenceTexture = scaledTex;
        scaledTexBounds.x *= 2;
        scaledTexBounds.y *= 2;
        scaledTexBounds.width *= 2;
        scaledTexBounds.height *= 2;
    }

    private float distance(Color32 c1, Color32 c2)
    {
        // IF the colors are the same, their distance is 0
        if (eq(c1,c2)) return 0;
        return Mathf.Abs(c1.r - c2.r) + Mathf.Abs(c1.g - c2.g) + Mathf.Abs(c1.b - c2.b);
    }

    private bool similar(Color32 c1, Color32 c2, Color32 reference)
    {
        if (eq(c1, c2)) return true;
        float d12 = distance(c1, c2);
        float d1r = distance(c1, reference);
        float d2r = distance(c2, reference);
        return d12 <= d1r && d12 <= d2r;
    }

    private bool different(Color32 c1, Color32 c2, Color32 reference)
    {
        return !similar(c1, c2, reference);
    }

    private bool eq(Color32 c1, Color32 c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b;
    }

    // Tex is the 8x scaled texture
    // TOO SLOW TOO SLOW TOO SLOW TOO SLOW
    // 
    // This algorithm calculates the best rotation offsets to use on the scaled image
    // by reducing the squared error of rotated pixels and their neighbor's colors
    private Vector2 bestOffset(Texture2D tex, float radians)
    {
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        int xcenter = tex.width / 2;
        int ycenter = tex.height / 2;

        Color32[] colors = tex.GetPixels32();

        float xBest = 0;
        float yBest = 0;
        float lowestError = float.MaxValue;
        for(int xOff = 0; xOff < 8; xOff++)
        {
            for(int yOff = 0; yOff < 8; yOff++)
            {
                float error = 0;
                
                // Loop through pixels and get squared error
                for(int x = xOff; x < tex.width; x+=8)
                {
                    for(int y = yOff; y < tex.height; y+=8)
                    {
                        int xx = (int)((x - xcenter) * cos + (y - ycenter) * -sin) + xcenter;
                        int yy = (int)((x - xcenter) * sin + (y - ycenter) * cos) + ycenter;

                        if (xx < 0 || xx >= tex.width || yy < 0 || yy >= tex.height) continue;

                        error += neighborSquaredError(xx, yy, tex);
                    }
                }

                // If the accumulated error is less than the best lowest error, then we found better rotation offsets
                if(error < lowestError)
                {
                    lowestError = error;
                    xBest = xOff;
                    yBest = yOff;
                }
            }
        }

        return new Vector2(xBest, yBest);
    }
    
    private float neighborSquaredError(int x, int y, Texture2D tex)
    {
        Color32[] colors = tex.GetPixels32();
        Color32 col = colors[x + y * tex.width];

        float error = 0;

        // Left Pixel
        int xx = x - 1;
        int yy = y;

        if(xx >= 0)
        {
            error += sqrError(col, colors[xx + yy * tex.width]);
        }

        // Right Pixel
        xx = x + 1;

        if(xx < tex.width)
        {
            error += sqrError(col, colors[xx + yy * tex.width]);
        }

        // Top Pixel
        xx = x;
        yy = y + 1;

        if (yy < tex.height)
        {
            error += sqrError(col, colors[xx + yy * tex.width]);
        }

        // Bottom Pixel
        yy = y - 1;

        if (yy >= 0)
        {
            error += sqrError(col, colors[xx + yy * tex.width]);
        }
        return error;
    }

    private float sqrError(Color32 c1, Color32 c2)
    {
        float r = c1.r - c2.r;
        float g = c1.g - c2.g;
        float b = c1.b - c2.b;
        float a = c1.a - c2.a;
        return r * r + g * g + b * b + a * a;
    }
}
