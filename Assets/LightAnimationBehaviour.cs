using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAnimationBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<ControllableLight>().IsAnimating = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<ControllableLight>().IsAnimating = false;
    }
}
