using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSnap : MonoBehaviour
{
    [SerializeField] private Transform transform;
    private int PPU;

    private void Start()
    {
        PPU = GameSettings.Settings.PPU;
    }

    private void LateUpdate()
    {
        var pos = transform.position;

        pos.x = Mathf.Floor(pos.x * PPU) / PPU;
        pos.y = Mathf.Floor(pos.y * PPU) / PPU;

        transform.position = pos;
    }

}
