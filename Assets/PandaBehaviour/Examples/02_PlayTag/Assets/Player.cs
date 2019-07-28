using UnityEngine;
using System.Collections;

namespace Panda.Examples.PlayTag
{
    public class Player : MonoBehaviour
    {

        Camera cam;
        public float extend = 2.0f;

        // Use this for initialization
        void Start()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            // Place the object under the mouse position
            var mp   = Input.mousePosition;
            mp.z = -cam.worldToCameraMatrix.MultiplyPoint(transform.position).z;
            var mpWs = cam.ScreenToWorldPoint(mp);
            mpWs.y = 0.0f; // Keep the player on the floor.

            var clipped = mpWs;
            // keep the player on the playground
            clipped.x = Mathf.Clamp(clipped.x , -extend, extend);
            clipped.z = Mathf.Clamp(clipped.z , -extend, extend);

            if( Vector3.Distance(clipped, mpWs) < 1e-3 )
                this.transform.position = clipped;
        }
    }
}
