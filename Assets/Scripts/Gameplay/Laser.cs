using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Laser : MonoBehaviour
{
    [SerializeField] private GameObject beginning;
    [SerializeField] private GameObject middle;
    [SerializeField] private GameObject end;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask playerLayer;
    public LayerMask PlayerLayer => playerLayer;

    [SerializeField] private Transform pivot;

    private float beginningPieceSize;
    private float endPieceSize;
    private float middlePieceSize;

    private Vector3 middleOff;
    private Vector3 endOff;
    private float middleScale;
    private float laserLength;

    private float middleSectionSize => laserLength - beginningPieceSize - endPieceSize;
    private float middleSizeDelta => middleSectionSize - middlePieceSize;
    private float minimumLaserSize => beginningPieceSize + endPieceSize;
    private bool noMiddle => middleSectionSize <= 0;

    private GameObject hitObject;
    public GameObject HitObject => hitObject;

    private void Start()
    {
        middleOff = middle.transform.localPosition;
        endOff = end.transform.localPosition;

        beginningPieceSize = GetWidthFromTexture(beginning);
        middlePieceSize = GetWidthFromTexture(middle);
        endPieceSize = GetWidthFromTexture(end);
    }

    private float GetWidthFromTexture(GameObject gameObject)
    {
        var sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        return sprite.texture.width / (float)GameSettings.Settings.PPU;
    }

    //private void Update()
    //{
    //    Shoot(90);
    //}

    public void Shoot(float angle)
    {
        SetAngle(angle);

        var ray = ObtainRay();
        laserLength = ray.distance;

        ScalePieces();
        PositionPieces();

        hitObject = ray.collider.gameObject;
    }

    private void PositionPieces()
    {
        PositionMiddlePiece();
        PositionEndPiece();
    }

    private void PositionEndPiece()
    {
        if(noMiddle) 
            end.transform.localPosition = new Vector3(endOff.x - middlePieceSize, 0, 0);
        else 
            end.transform.localPosition = new Vector3(endOff.x + middleSizeDelta, 0, 0);
    }

    private void PositionMiddlePiece()
    {
        var xOff = middleSizeDelta * 0.5f;
        middle.transform.localPosition = new Vector3(middleOff.x + xOff, 0, 0);
    }

    private void ScalePieces()
    {
        if(middleSectionSize < 0)
        {
            ScaleNoMiddle();
        }
        else
        {
            ScaleMiddle();
        }
    }

    private void ScaleMiddle()
    {
        middle.SetActive(true);
        middleScale = middleSectionSize / middlePieceSize;
        middle.transform.localScale = new Vector3(middleScale, 1, 1);
    }

    private void ScaleNoMiddle()
    {
        middle.SetActive(false);
    }

    private RaycastHit2D ObtainRay()
    {
        return Physics2D.Raycast(pivot.position, transform.rotation * Vector3.right, 100, wallLayer | playerLayer);
    }

    private void SetAngle(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
