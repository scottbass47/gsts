using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    public static CameraManager Instance => instance;

    [SerializeField] private Camera gameCamera;
    public Camera GameCam => gameCamera;

    [SerializeField] private Camera hudCamera;
    public Camera HudCam => hudCamera;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        hudCamera.orthographicSize = GameSettings.Settings.HudOrthographicZoom;
    }

    public Vector2 GameCameraPosFloored { get; set; }
    public Vector2 GameCameraPos { get; set; }

}
