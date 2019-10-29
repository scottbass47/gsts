using Guns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Gun))]
public class GunAnimatorController : MonoBehaviour
{
    [SerializeField] private string fireTrigger = "fire";
    private Animator animator;

    private void Start()
    {
        var gun = GetComponent<Gun>();
        gun.GunModel.OnBulletShot += OnBulletShot;
        animator = GetComponent<Animator>();
    }

    private void OnBulletShot()
    {
        animator.SetTrigger(fireTrigger);
    }
}

