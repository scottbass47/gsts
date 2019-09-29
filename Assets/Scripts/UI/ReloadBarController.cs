using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadBarController : MonoBehaviour
{
    [SerializeField] private GameObject slider;

    private RectTransform boundaryTransform;
    private RectTransform sliderTransform;
    private AnimationCurve sliderCurve;

    private void Start()
    {
        boundaryTransform = GetComponent<RectTransform>();

        slider.SetActive(false);
        sliderTransform = slider.GetComponent<RectTransform>();

        GameManager.Instance.Events.AddListener<Reload>(this.gameObject, (obj) =>
        {
            StartCoroutine(ReloadAnimation(obj.Duration));
        });

        sliderCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    }

    private IEnumerator ReloadAnimation(float duration)
    {
        slider.SetActive(true);
        sliderTransform.anchoredPosition = Vector3.zero;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            LerpSlider(t / duration);
            yield return null;
        }

        slider.SetActive(false);
    }

    private void LerpSlider(float lerpAmount)
    {
        sliderTransform.anchoredPosition = 
            Vector3.Lerp(Vector3.zero, Vector3.right * boundaryTransform.rect.width, sliderCurve.Evaluate(lerpAmount));
    }
}
