using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float rotation;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private GameObject beginning;
    [SerializeField] private GameObject middle;
    [SerializeField] private GameObject end;

    [SerializeField] private float beginningLength;
    [SerializeField] private float endLength;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform pivot;

    private Vector3 middleOff;
    private Vector3 endOff;

    private void Awake()
    {
        middleOff = middle.transform.localPosition;
        endOff = end.transform.localPosition;
    }

    private void Update()
    {
        rotation += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, rotation);

        var ray = Physics2D.Raycast(pivot.position, transform.rotation * Vector3.right, 100, layerMask);

        var middleScale = (ray.distance - beginningLength - endLength) * 2;
        middle.transform.localScale = new Vector3(middleScale, 1, 1);
        middle.transform.localPosition = new Vector3(middleOff.x + (middleScale - 1) * 0.25f, 0, 0);
        end.transform.localPosition = new Vector3(endOff.x + (middleScale - 1) * 0.5f, 0, 0);
    }
}
