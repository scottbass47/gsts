using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private Animator animator;
    private Action openCallback;
    private Action closeCallback;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Open(Action callback)
    {
        openCallback = callback;
        animator.SetTrigger("open");
    }

    public void Close(Action callback)
    {
        closeCallback = callback;
        animator.SetTrigger("close");
    }

    public void OpenAnimationFinished()
    {
        openCallback?.Invoke();
    }

    public void CloseAnimationFinished()
    {
        closeCallback?.Invoke();
    }
}
