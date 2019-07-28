using UnityEngine;
using System.Collections;

using Panda;

namespace Panda.Examples.PlayTag
{
    public class Computer : MonoBehaviour
    {

        public float speed = 1.0f; // per second.

        public Color it;
        public Color notIt;

        public Dialogue tagDialogue;

        Vector3 destination = Vector3.zero;

        Player player;

        #region tasks

        [Task]
        bool IsIt = true; // Whether the agent is "It".

        bool _IsColliding_Player = false;

        [Task]
        bool IsColliding_Player
        {
            get
            {
                return _IsColliding_Player;
            }
        }


        /*
         * Whether the player is near.
         */
        [Task]
        void IsPlayerNear()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, this.transform.position);
            Task.current.Complete(  distanceToPlayer < 4.0f );
        }

        /*
        * Pop a text over the agent.
        */
        [Task]
        bool Say(string text)
        {
            tagDialogue.SetText(text);
            tagDialogue.speaker = this.gameObject;
            tagDialogue.ShowText();
            return true;
        }

        /*
         * Move to the destination at the current speed and succeeds when the destination has been reached.
         */
        [Task]
        void MoveToDestination()
        {
            var task = Task.current;
            var delta = destination - transform.position;

            if (transform.position != destination)
            {
                var velocity = delta.normalized * speed;
                transform.position = transform.position + velocity * Time.deltaTime;

                // Check for overshooting the destination.
                // Succeed when the destination is reached.
                var newDelta = destination - transform.position;
                if (Vector3.Dot(newDelta, delta) < 0.0f)
                {
                    transform.position = destination;
                }
            }

            if (transform.position == destination)
                task.Succeed();
        }

        /*
         * Used the current player position  as destination and succeeds.
         */
        [Task]
        bool SetDestination_Player()
        {
            destination = player.transform.position;
            return true;
        }

        /*
         * Used the a random position as destination and succeeds.
         */
        [Task]
        bool SetDestination_Random()
        {
            destination = Random.insideUnitSphere * player.extend;
            destination.y = 0.0f;

            return true;
        }

        /*
         * Succeeds when the current destination direction is safe. 
         */
        [Task]
        bool IsDirectionSafe
        { 
            get
            {
                Vector3 playerDirection = (player.transform.position - this.transform.position).normalized;
                Vector3 destinatioDirection = (destination - this.transform.position).normalized;
                bool isSafe = Vector3.Angle(destinatioDirection, playerDirection) > 45.0f;
                return isSafe;
            }
        }

        /*
         * Set the current speed and succeeds. 
         */
        [Task]
        bool SetSpeed( float speed )
        {
            this.speed = speed;
            return true;
        }


         /*
         * Tag and apply the color accordingly.
         */
        [Task]
        bool Tag()
        {
            DoTag();
            return true;
        }

        #endregion

        void DoTag()
        {
            IsIt = !IsIt;
            this.GetComponent<Renderer>().material.color = IsIt ? it : notIt;
            player.GetComponent<Renderer>().material.color = IsIt ? notIt : it;
        }

        // Use this for initialization
        void Start()
        {
            player = FindObjectOfType<Player>();
            DoTag();
        }



        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player.gameObject )
                _IsColliding_Player = true;      
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject == player.gameObject)
                _IsColliding_Player = false;       
        }

    }
}
