using UnityEngine;
using System.Collections;
using Panda;

namespace Panda.Examples.Shooter
{
    public class UnitExplosion : MonoBehaviour
    {

        public float duration = 1.5f;

        float startTime;
        // Use this for initialization
        void Start()
        {
            startTime = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            if( Time.time - startTime > duration )
            {
                Destroy(this.gameObject);
            }
        }
    }
}
