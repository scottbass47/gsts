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

        var gameTexture = CreateRenderTexture(settings.ExtendedWidth, settings.ExtendedHeight);
        var hudTexture = CreateRenderTexture(settings.GameWidth, settings.GameHeight);
        material.SetTexture("_GameTex", gameTexture);
        material.SetTexture("_HudTex", hudTexture);

        var camMan = CameraManager.Instance; 
        camMan.GameCam.targetTexture = gameTexture;
        camMan.HudCam.targetTexture = hudTexture;
    }

    private RenderTexture CreateRenderTexture(int width, int height)
    {
        return new RenderTexture(width, height, 24)
        {
            filterMode = FilterMode.Point,
            hideFlags = HideFlags.DontSave
        };
    }

    private void OnGUI()
    {
        if (!Event.current.type.Equals(EventType.Repaint)) return;

        var camMan = CameraManager.Instance;

        GL.Clear(true, true, Color.black);

        var rect = GetRect();

        if(settings.ScaleMode == ScaleMode.PerfectFit)
        {
            rect.width = Mathf.FloorToInt(rect.width / settings.GameWidth) * settings.GameWidth;
            rect.height = Mathf.FloorToInt(rect.height / settings.GameHeight) * settings.GameHeight;

            rect.x = (Screen.width - rect.width) * 0.5f;
            rect.y = (Screen.height - rect.height) * 0.5f;
        }
        var xOff = (settings.ExtendedWidth - settings.GameWidth) / (float)settings.ExtendedWidth * 0.5f;
        var yOff = (settings.ExtendedHeight - settings.GameHeight) / (float)settings.ExtendedHeight * 0.5f;
        var sourceRect = new Vector4(xOff, yOff, 1 - xOff, 1 - yOff);
        var gameTexture = camMan.GameCam.targetTexture;

        var shift = Vector2.zero;
        if(settings.SubpixelMovement)
        {
            shift = (camMan.GameCameraPos - camMan.GameCameraPosFloored) * settings.PPU / (float)settings.ExtendedWidth;
        }

        material.SetVector("_GameRect", sourceRect);
        material.SetVector("_GameShift", shift);

        Graphics.DrawTexture(rect, gameTexture, new Rect(0,0,1,1), 0, 0, 0, 0, Color.white, material);
    } 

    private Rect GetRect()
    {
        var rect = new Rect(0, 0, Screen.width, Screen.height);

        if (settings.ScaleMode != ScaleMode.Stretch)
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
