using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	
	private void Update () {
        var player = GameManager.Instance.Player;
        if (player == null) return;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = player.transform.position;
        transform.position = (mouse + playerPos * 2) / 3;
	}
}
