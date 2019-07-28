using UnityEngine;
using System.Collections;
using Panda;


namespace Panda.Examples.Shooter
{
    public class Unit: MonoBehaviour
    {


        public int team = 0; // Team the unit is in. A unit in a different team is an enemy.
        public float health = 10.0f; // HP
        public GameObject bulletPrefab;
        public GameObject jammedEffectPrefab;
        public float rotationSpeed = 1.0f;
        public float reloadRate = 0.5f; // How many bullet per second get restored.
        public int ammo = 5;

        public GameObject explosionPrefab;
        [HideInInspector]
        public Unit shotBy; // Last unit that shot this unit.

        [HideInInspector]
        public float lastShotTime; // Time when the unit has been shot for the last time.

        [HideInInspector]
        public Unit lastHit; // Last unit this unit shot to.

        [HideInInspector]
        public float lastHitTime; // Last time this unit hit another unit.

        [HideInInspector]
        public UnityEngine.AI.NavMeshAgent navMeshAgent;

        [HideInInspector]
        public Vector3 destination; // The movement destination.
        public Vector3 target;      // The position to aim to.

        [HideInInspector]
        public float startHealth; 



        [HideInInspector]
        public int startAmmo;

        [HideInInspector]
        public float lastReloadTime; // Last time a bullet has been restored.

        // Use this for initialization
        void Start()
        {
            lastShotTime = lastHitTime = float.NegativeInfinity;

            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            startHealth = health;
            destination = this.transform.position;

            startAmmo = ammo;
            lastReloadTime = float.NegativeInfinity;
        }

        void Update()
        {
            if( (Time.time - lastReloadTime) * reloadRate >= 1.0f )
            {
                ammo++;
                if (ammo > startAmmo) ammo = startAmmo;
                lastReloadTime = Time.time;
            }

        }


        Vector3 GetMousePosition()
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.worldToCameraMatrix.MultiplyPoint(this.transform.position).z;
            var pos = Camera.main.ScreenToWorldPoint(mousePosition);
            return pos;
        }

        #region navigation tasks

        [Task]
        public bool IsHealthLessThan( float health )
        {
            return this.health < health;
        }

        [Task]
        public bool IsHealth_PercentLessThan(float percent)
        {
            return (this.health / startHealth)*100.0 < percent;
        }

        [Task]
        public bool HasAmmo()
        {
            return ammo > 0;
        }

        [Task]
        public bool Ammo_LessThan( int i )
        {
            if( Task.isInspected)
                Task.current.debugInfo = string.Format("a={0}", ammo);
            return ammo < i;
        }


        [Task]
        public bool SetDestination(Vector3 p)
        {
            destination =  p;
            navMeshAgent.destination = destination;

            if( Task.isInspected )
                Task.current.debugInfo = string.Format("({0}, {1})", destination.x, destination.y);
            return true;
        }

        [Task]
        public void WaitArrival()
        {
            var task = Task.current;
            float d = navMeshAgent.remainingDistance;
            if (!task.isStarting && navMeshAgent.remainingDistance <= 1e-2)
            {
                task.Succeed();
                d = 0.0f;
            }
           
            if( Task.isInspected )
                task.debugInfo = string.Format("d-{0:0.00}", d );
        }

        [Task]
        public void MoveTo(Vector3 dst)
        {
            SetDestination(dst);
            if (Task.current.isStarting)
                navMeshAgent.isStopped = false;
            WaitArrival();
        }

        [Task]
        public void MoveTo_Destination()
        {
            MoveTo(destination);
            WaitArrival();
        }


        [Task]
        public bool SetDestination_Mouse()
        {
            var pos = GetMousePosition();
            SetDestination(pos);
            return true;
        }

        [Task]
        public bool SetDestination_WASD()
        {
            bool isSet = false;
            Vector3 dir = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                dir += Vector3.forward;
            if (Input.GetKey(KeyCode.A))
                dir += Vector3.left;
            if (Input.GetKey(KeyCode.S))
                dir += Vector3.back;
            if (Input.GetKey(KeyCode.D))
                dir += Vector3.right;

            if (dir.magnitude > 0.0f)
            {
                var pos =this.transform.position + dir.normalized;
                SetDestination(pos);
                isSet = true;
            }


            return isSet;
        }


        [Task]
        public bool Stop()
        {
            navMeshAgent.isStopped = true;
            return true;
        }

        [Task]
        public bool LastShotTime_LessThan(float duration)
        {
            var t = (Time.time - lastShotTime);
            if( Task.isInspected )
                Task.current.debugInfo = string.Format("t={0:0.00}", t);
            return t < duration;
        }
        #endregion

        #region combat tasks

        [Task]
        public bool Fire()
        {

            var bulletOb = ammo > 0? GameObject.Instantiate(bulletPrefab): GameObject.Instantiate(jammedEffectPrefab);

            bulletOb.transform.position = this.transform.position;
            bulletOb.transform.rotation = this.transform.rotation;
            if (ammo > 0)
            {
                

                var bullet = bulletOb.GetComponent<Bullet>();
                bullet.shooter = this.gameObject;

                ammo--;
                lastReloadTime = Time.time;
            }
            else
            {
                lastReloadTime = Time.time + (1.0f/reloadRate);
                bulletOb.transform.parent = this.transform;
            }


            return true;
        }

        [Task]
        public bool SetTarget( Vector3 target )
        {
            this.target = target;
            this.target.y = this.transform.position.y;
            return true;
        }

        [Task]
        public bool SetTarget_Mouse()
        {
            target = GetMousePosition();
            return true;
        }

        [Task]
        bool SetTarget_Destination()
        {
            return SetTarget(destination);
        }

        [Task]
        public void AimAt_Target()
        {
            var targetDelta = (target - this.transform.position);
            var targetDir = targetDelta.normalized;

            if (targetDelta.magnitude > 0.2f)
            {


                Vector3 axis = Vector3.up * Mathf.Sign(Vector3.Cross(this.transform.forward, targetDir).y);

                var rot = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, axis);
                this.transform.rotation = rot * this.transform.rotation;

                Vector3 newAxis = Vector3.up * Mathf.Sign(Vector3.Cross(this.transform.forward, targetDir).y);

                float dot = Vector3.Dot(axis, newAxis);

                if (dot < 0.01f) 
                {// We overshooted the target
                    var snapRot = Quaternion.FromToRotation(this.transform.forward, targetDir);
                    this.transform.rotation = snapRot * this.transform.rotation;
                    Task.current.Succeed();
                }

                var straighUp = Quaternion.FromToRotation(this.transform.up, Vector3.up);
                this.transform.rotation = straighUp * this.transform.rotation;
            }
            else
            {
                Task.current.Succeed();
            }

            if( Task.isInspected )
                Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(targetDir, this.transform.forward));


        }
        #endregion

        [Task]
        bool Explode()
        {
            ShooterGameController.instance.OnUnitDestroy(this);

            if (explosionPrefab != null)
            {
                var explosion = GameObject.Instantiate(explosionPrefab);
                explosion.transform.position = this.transform.position;
            }

            Destroy(this.gameObject);
            return true;
        }


    }
}
