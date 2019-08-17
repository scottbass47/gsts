using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    [SerializeField] private float bobAmplitude = 1;
    [SerializeField] private float bobFrequency = 1;

    private float elapsed = 0.0f;
    private Vector3 start;

    private void Start()
    {
        start = transform.localPosition;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        transform.localPosition = start + new Vector3(0, bobAmplitude * Mathf.Sin(bobFrequency * elapsed), 0);
    }
}
