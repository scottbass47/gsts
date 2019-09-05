using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float lerpSpeed;

	private void Update () {
        var player = GameManager.Instance.Player;
        if (player == null) return;
        Vector3 mouse = Mouse.WorldPos;
        Vector3 playerPos = player.transform.position;
        transform.position = Vector3.Lerp(transform.position, (mouse + playerPos * 2) / 3, Time.deltaTime * lerpSpeed);
	}
}
