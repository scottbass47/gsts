using UnityEngine;
using System.Collections;
using Panda;

namespace Panda.Examples.ChangeColor
{
    public class MyCube : MonoBehaviour
    {
        /*
         * Set the color to the specified rgb value and succeed.
         */
        [Task] // <-- Attribute used to tag a class member as a task implementation.
        void SetColor(float r, float g, float b)
        {
            this.GetComponent<Renderer>().material.color = new Color(r, g, b);
            Task.current.Succeed(); // <-- Task.current gives access to the run-time task bind to this method.
        }
    }
}
