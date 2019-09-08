//using Assets.FastRotSprite.Scripts;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
//public class AnimatorRenderRotation : RotationBase
//{
//    [SerializeField] private ClipData[] clips;
//    private Animator animator;
//    private SpriteRenderer spriteRenderer;
//    private Dictionary<string, Rotator[]> clipDict;
//    private Sprite rotatedSprite;

//    private void Awake()
//    {
//        animator = GetComponent<Animator>();
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        clipDict = new Dictionary<string, Rotator[]>();

//        foreach (var data in clips)
//        {
//            var rotators = new Rotator[data.Sprites.Length];
//            for(int i = 0; i < data.Sprites.Length; i++)
//            {
//                var tex = data.Sprites[i].texture;
//                rotators[i] = new Rotator(tex.GetPixels32(), tex.width, tex.height);
//            }
//            clipDict.Add(data.Clip.name, rotators);
//        }

//        Source = spriteRenderer.sprite.texture;
//        rotatedSprite = spriteRenderer.sprite;
//    }

//    public override void SetSprite(Sprite sprite)
//    {
//        rotatedSprite = sprite;
//    }

//    private void Update()
//    {
//        var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
//        var currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

//        float percentDone = currentStateInfo.normalizedTime % 1;
//        float clipLength = currentClipInfo.clip.length;
//        float frameRate = currentClipInfo.clip.frameRate;
//        int currFrame = (int)(percentDone * clipLength * frameRate);

//        var name = currentClipInfo.clip.name;
//        //Debug.Log($"Frame: {currFrame}, Clip Length: {clipLength}, Frame Rate: {frameRate}");
//        Rotator = clipDict[name][currFrame];
//    }

//    private void LateUpdate()
//    {
//        spriteRenderer.sprite = rotatedSprite;
//    }
//}

//[System.Serializable]
//public class ClipData
//{
//    public AnimationClip Clip;
//    public Sprite[] Sprites;
//}
