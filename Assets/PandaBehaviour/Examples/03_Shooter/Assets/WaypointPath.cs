using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Panda;


namespace Panda.Examples.Shooter
{


    public class WaypointPath : MonoBehaviour
    {
        public bool loop;
        public Transform[] waypoints
        {
            get
            {
                var wp = new List<Transform>();
                foreach( Transform c in this.transform)
                {
                    wp.Add(c);
                }
                return wp.ToArray();
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDrawGizmos()
        {
            DrawLines();
            DrawSpheres(0.1f);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            DrawLines();
            DrawSpheres(0.5f);
        }

        void DrawSpheres(float r)
        {
            var wp = waypoints;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawSphere(wp[i].position, r);
            }
        }

        void DrawLines()
        {
            var wp = waypoints;
            for (int i = 1; i < waypoints.Length; i++)
            {
                var p0 = wp[i - 1].position;
                var p1 = wp[i - 0].position;
                Gizmos.DrawLine(p0, p1);
            }

            if( loop && wp.Length > 1 )
            {
                var p0 = wp[0].position;
                var p1 = wp[wp.Length-1].position;
                Gizmos.DrawLine(p0, p1);
            }
        }
    }
}
