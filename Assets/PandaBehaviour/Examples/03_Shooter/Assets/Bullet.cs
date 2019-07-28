using UnityEngine;
using System.Collections;



namespace Panda.Examples.Shooter
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 1.0f;
        public float duration = 5.0f;
        public float damage = 1.0f;
        public GameObject impactPrefab;

        public GameObject shooter;
        float startTime;

        // Use this for initialization
        void Start()
        {
            startTime = Time.time;
        }

        void FixedUpdate()
        {
            Vector3 velocity = this.transform.forward * speed;
            this.transform.position = this.transform.position + velocity * Time.deltaTime;

            if (Time.time - startTime > duration)
                Destroy(this.gameObject);
            else
                BulletScan();
                
        }

        void BulletScan()
        {

            RaycastHit hit;
            var ray = new Ray(this.transform.position, this.transform.forward);
            if (UnityEngine.Physics.Raycast(ray, out hit, 1.0f))
            {
                var other = hit.collider;
                var type = other.gameObject.GetComponent<TriggerType>();
                var go = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
                if (type != null && type.collidesWithBullet && go != shooter)
                {
                    Unit target = go.GetComponent<Unit>();
                    OnHit(target);
                    Explode(hit);

                }
            }
        }




        void OnHit(Unit target)
        {
            if(target != null && target != shooter )
            {
                if (this.shooter != null)
                {
                    var shotBy = this.shooter.GetComponent<Unit>();
                    target.shotBy = shotBy;
                    target.lastShotTime = Time.time;

                    if (shotBy != null)
                    {
                        shotBy.lastHit = target;
                        shotBy.lastHitTime = Time.time;
                    }
                }

                target.health -= damage;

            }


        }


        void Explode(RaycastHit hit)
        {
            var pos = this.transform.position;
            var dir = this.transform.forward;

            var impact = Instantiate(impactPrefab);
            impact.transform.position = pos;

            Destroy(this.gameObject);

            // compute reflection direction by normal
            dir = dir -2.0f*hit.normal*( Vector3.Dot(dir, hit.normal));
            impact.transform.position = hit.point;

            var rot = Quaternion.FromToRotation(impact.transform.forward, dir);
            impact.transform.rotation = rot * impact.transform.rotation;



        }
    }
}

