using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTasks : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    private IMovement movement;

    private void Start()
    {
        if(animator == null)
        {
            Debug.LogError("AnimationTasks - No animator assigned!");
        }
        movement = GetComponent<IMovement>();
        movement?.SetAnimator(animator);
    }

    [Task]
    public void PlayAnimation(string animation)
    {
        if(animator == null)
        {
            Task.current.Fail();
            return;
        }
        animator.SetTrigger(animation);
        Task.current.Succeed();
    }

}
