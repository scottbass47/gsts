using UnityEngine;
using System.Collections;
using Panda;


namespace Panda.Examples.Shooter
{
    namespace Panda
    {
        public class BulletImpact : MonoBehaviour
        {
            public Light flash;
            public float duration = 1.0f;

            float startTime;
            float startIntensity;
            // Use this for initialization
            void Start()
            {
                startTime = Time.time;
                startIntensity = flash.intensity;
            }

            // Update is called once per frame
            void Update()
            {

                float elapsedTime = Time.time - startTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                flash.intensity = startIntensity * (1.0f - t);

                if (elapsedTime > duration)
                    Destroy(this.gameObject);
            }
        }
    }
}
