using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRenderer : MonoBehaviour
{
    [SerializeField]
    private Material material;

    private Settings settings;

    private void Start()
    {
        settings = GameSettings.Settings;
        material.SetFloat("PixelSnap", 1.0f);

        Camera.main.targetTexture = new RenderTexture(settings.GameWidth, settings.GameHeight, 24)
        {
            filterMode = FilterMode.Point,
            hideFlags = HideFlags.DontSave
        };
    }

    private void OnGUI()
    {
        if (!Event.current.type.Equals(EventType.Repaint)) return;

        GL.Clear(true, true, Color.black);

        var rect = GetRect(true);

        rect.width = Mathf.FloorToInt(rect.width / settings.GameWidth) * settings.GameWidth;
        rect.height = Mathf.FloorToInt(rect.height / settings.GameHeight) * settings.GameHeight;

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

            if (ratio > settings.ResolutionRatio)
            {
                rect.width = rect.height * settings.ResolutionRatio;
            }
            else
            {
                rect.height = rect.width / settings.ResolutionRatio;
            }
            rect.x = (Screen.width - rect.width) * 0.5f;
            rect.y = (Screen.height - rect.height) * 0.5f;
        }

        return rect;
    }

}
