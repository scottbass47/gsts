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
    private float middleLength;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask playerLayer;
    public LayerMask PlayerLayer => playerLayer;

    [SerializeField] private Transform pivot;

    private Vector3 middleOff;
    private Vector3 endOff;

    private void Awake()
    {
        middleOff = middle.transform.localPosition;
        endOff = end.transform.localPosition;

        var middleSprite = middle.GetComponent<SpriteRenderer>().sprite;
        middleLength = middleSprite.texture.width / (float)GameSettings.Settings.PPU;
    }

    public GameObject Shoot(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
        var ray = Physics2D.Raycast(pivot.position, transform.rotation * Vector3.right, 100, wallLayer | playerLayer);

        var middleScale = (ray.distance - beginningLength - endLength) / middleLength;
        middle.transform.localScale = new Vector3(middleScale, 1, 1);
        middle.transform.localPosition = new Vector3(middleOff.x + (middleScale - 1) * middleLength * 0.5f, 0, 0);
        end.transform.localPosition = new Vector3(endOff.x + (middleScale - 1) * middleLength, 0, 0);

        return ray.collider.gameObject;
    }

    //private void Update()
    //{
    //    Shoot(0);
    //}
}
