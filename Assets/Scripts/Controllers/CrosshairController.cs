using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour {

    // Update is called once per frame
    void Update () {
        transform.position = Mouse.WorldPos;
        Cursor.visible = false;
	}
}
