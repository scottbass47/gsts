using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRenderer : MonoBehaviour
{
    [SerializeField] private int width = 384;
    [SerializeField] private int height = 216;

    public static int GAME_WIDTH = 384;
    public static int GAME_HEIGHT = 216;
    public static float RATIO => GAME_WIDTH / (float)GAME_HEIGHT;

    [SerializeField]
    private Material material;

    private void Awake()
    {
        GAME_WIDTH = width;
        GAME_HEIGHT = height;

        Camera.main.targetTexture = new RenderTexture(GAME_WIDTH, GAME_HEIGHT, 24)
        {
            filterMode = FilterMode.Point,
            hideFlags = HideFlags.DontSave
        };
    }

    private void Start()
    {
        material.SetFloat("PixelSnap", 1.0f);
    }

    private void OnGUI()
    {
        if (!Event.current.type.Equals(EventType.Repaint)) return;

        GL.Clear(true, true, Color.black);

        var rect = GetRect(true);

        rect.width = Mathf.FloorToInt(rect.width / GAME_WIDTH) * GAME_WIDTH;
        rect.height = Mathf.FloorToInt(rect.height / GAME_HEIGHT) * GAME_HEIGHT;

        rect.x = (Screen.width - rect.width) * 0.5f;
        rect.y = (Screen.height - rect.height) * 0.5f;

        Graphics.DrawTexture(rect, Camera.main.targetTexture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, Color.white, material);
    } 

    private Rect GetRect(bool scaleToFit)
    {
        var rect = new Rect(0, 0, Screen.width, Screen.height);

        if (scaleToFit)
        {
            float ratio = Screen.width / (float)Screen.height;

            if (ratio > RATIO)
            {
                rect.width = rect.height * RATIO;
            }
            else
            {
                rect.height = rect.width / RATIO;
            }
            rect.x = (Screen.width - rect.width) * 0.5f;
            rect.y = (Screen.height - rect.height) * 0.5f;
        }

        return rect;
    }

}
