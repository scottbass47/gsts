using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float lerpSpeed;

    private int PPU;
    private CameraManager cameraManager => CameraManager.Instance;

    private void Start()
    {
        PPU = GameSettings.Settings.PPU;
    }

    private void Update () {
        var player = GameManager.Instance.Player;
        if (player == null) return;

        Vector3 mouse = Mouse.WorldPos;
        Vector3 playerPos = player.transform.position;

        var pos = transform.position;
        pos = Vector3.Lerp(transform.position, (mouse + playerPos * 2) / 3, Time.deltaTime * lerpSpeed);

        cameraManager.GameCameraPos = new Vector2(pos.x, pos.y);

        pos.x = Mathf.Floor(pos.x * PPU) / PPU;
        pos.y = Mathf.Floor(pos.y * PPU) / PPU;

        transform.position = pos;

        cameraManager.GameCameraPosFloored = new Vector2(pos.x, pos.y);
	}
}
