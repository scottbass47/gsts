using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private GameObject player;

	// Use this for initialization
	private void Start () {
        GameManager.Instance.Events.AddListener<PlayerSpawn>((obj) => 
        {
            player = obj.Player;
        });
	}
	
	private void Update () {
        if (player == null) return;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = player.transform.position;
        transform.position = (mouse + playerPos * 2) / 3;
	}
}
