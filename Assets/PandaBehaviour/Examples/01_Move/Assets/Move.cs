using UnityEngine;
using System.Collections;

using Panda;
namespace Panda.Examples.Move
{
    public class Move : MonoBehaviour
    {

        float speed = 1.0f; // current speed


        /*
         * Move to the (x,y) position at the current speed.
         */
        [Task]
        void MoveTo(float x, float y)
        {
            Vector3 destination = new Vector3(x, y, 0.0f);
            Vector3 delta = (destination - transform.position);
            Vector3 velocity = speed*delta.normalized;

            transform.position = transform.position + velocity * Time.deltaTime;

            Vector3 newDelta = (destination - transform.position);
            float d = newDelta.magnitude;

            if (Task.isInspected)
                Task.current.debugInfo = string.Format("d={0:0.000}", d);

            if ( Vector3.Dot(delta, newDelta) <= 0.0f || d < 1e-3)
            {
                transform.position = destination;
                Task.current.Succeed();
                d = 0.0f;
                Task.current.debugInfo = "d=0.000";
            }

        }

        // This structure is used to store data for rotation tweening.
        struct RotateTween
        {
            public Quaternion startRotation;
            public Quaternion endRotation;
            public float startTime;
        }

        /*
         * Rotate about the given angle for the given duration. 
         */
        [Task]
        void Rotate(float angle, float duration)
        {
            var task = Task.current; // shortcut to the current task.
            RotateTween rt;

            // The Task.isStarting property is true on the first tick of a task.
            // We used it to perform initialization.
            if ( task.isStarting ) 
            {
                // Compute tweeing data
                rt.startTime = Time.time;
                rt.startRotation = this.transform.localRotation;
                rt.endRotation   = Quaternion.AngleAxis( angle, Vector3.forward)*transform.localRotation;

                // Task.item is a placeholder attached to a Task.
                // It is useful for storing any data used for the progression of a task.
                // We use it here to hold the tweening data.
                task.item = rt;
            }

            rt = (RotateTween)task.item; // Retrieve the tweening data.

            // Interpolate for the current rotation
            float elapsedTime = Time.time - rt.startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            this.transform.localRotation = Quaternion.Lerp(rt.startRotation, rt.endRotation, t);


            // Display the tweening progression withing the code viewer in the Inspector.
            if( Task.isInspected )
                task.debugInfo = string.Format("t={0:0.00}", t);

            // Succeed the task when the tweening is complete.
            if (t == 1.0f)
            {
                task.debugInfo = "t=1.00";
                task.Succeed();
            }


        }


    }
}
