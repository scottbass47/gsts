//using Assets.FastRotSprite.Scripts;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class RotationTest : MonoBehaviour
//{
//    public Transform rotateAbout;
//    public SpriteRendererRotation[] rotators;
//    public SpriteRenderer shaderRenderer;
//    public Transform normalRotate;

//    private void Update()
//    {
//        var inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        var diff = inputPos - rotateAbout.position;
//        var angleRad = Mathf.Atan2(diff.y, diff.x);
//        foreach(var rot in rotators)
//        {
//            rot.RotationRadians = angleRad;
//        }
//        shaderRenderer.material.SetFloat("_Rotation", angleRad * Mathf.Rad2Deg);
//        normalRotate.eulerAngles = new Vector3(0, 0, angleRad * Mathf.Rad2Deg);
//    }
//}
