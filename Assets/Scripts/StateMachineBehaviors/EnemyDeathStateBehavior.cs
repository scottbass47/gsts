using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathStateBehavior : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.enabled = false;
        animator.gameObject.GetComponent<EnemyDeath>().AnimationFinished();
    }
}
