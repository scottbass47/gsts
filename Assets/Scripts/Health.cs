using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public float Amount; 

    private void Update()
    {
        if (Amount < 0) Destroy(gameObject);
    }
}
