using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Panda;

namespace Panda.Examples.PlayTag
{
    public class Dialogue : MonoBehaviour
    {

        public GameObject speaker;

        Text text;
        PandaBehaviour bt;
        RectTransform rectTransform;

        [Task]
        void EnableText(bool show )
        {
            text.enabled = show;
            Task.current.Succeed();
        }


        public void SetText( string text  )
        {
            this.text.text = text;
        }

        public void ShowText()
        {
            bt.enabled = true;
            bt.Reset();
        }

        // Use this for initialization
        void Start()
        {
            text = this.GetComponent<Text>();
            bt =  this.GetComponent<PandaBehaviour>();
            rectTransform = this.GetComponent<RectTransform>();
            bt.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, speaker.transform.position + Vector3.forward*1.0f);
            rectTransform.position = screenPoint;
        }
    }
}
