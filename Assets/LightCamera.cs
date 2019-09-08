using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCamera : MonoBehaviour
{
    [SerializeField] private Transform gameCamera;

    private void Start()
    {
        GetComponent<Camera>().orthographicSize = GameSettings.Settings.OrthographicZoom;
    }

    private void LateUpdate()
    {
        transform.position = gameCamera.position;
    }
}
