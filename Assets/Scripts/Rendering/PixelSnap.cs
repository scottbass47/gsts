using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSnap : MonoBehaviour
{
    [SerializeField] private Transform transform;
    private int PPU = 16;

    private void LateUpdate()
    {
        var pos = transform.position;

        //pos.x = Mathf.Floor(pos.x * ScreenRenderer.GAME_WIDTH) / ScreenRenderer.GAME_WIDTH;
        //pos.y = Mathf.Floor(pos.y * ScreenRenderer.GAME_HEIGHT) / ScreenRenderer.GAME_HEIGHT;

        pos.x = Mathf.Floor(pos.x * PPU) / PPU;
        pos.y = Mathf.Floor(pos.y * PPU) / PPU;

        transform.position = pos;
    }

}
