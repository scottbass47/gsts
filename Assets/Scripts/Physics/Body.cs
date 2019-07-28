using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] private Transform centerFeet;
    [SerializeField] private Transform centerBody;

    public Transform CenterFeet => centerFeet;
    public Transform CenterBody => centerBody;
}
