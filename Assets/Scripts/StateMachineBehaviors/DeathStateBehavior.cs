using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStateBehavior : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.enabled = false;
        animator.gameObject.GetComponentInParent<Death>().OnAnimationFinish();
    }
}
