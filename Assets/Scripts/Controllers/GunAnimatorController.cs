using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(GunController))]
public class GunAnimatorController : MonoBehaviour
{
    [SerializeField] private string fireTrigger = "fire";
    private Animator animator;

    private void Start()
    {
        var gunController = GetComponent<GunController>();
        gunController.OnFire += OnFire;
        animator = GetComponent<Animator>();
    }

    private void OnFire()
    {
        animator.SetTrigger(fireTrigger);
    }
}
