using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadBarUI : MonoBehaviour
{
    [SerializeField] private RectTransform sliderBegin;
    [SerializeField] private RectTransform sliderEnd;
    [SerializeField] private GameObject sliderPrefab;

    private GameObject slider;
    private RectTransform sliderTransform;

    private void Start()
    {
        slider = Instantiate(sliderPrefab, transform);
        sliderTransform = slider.GetComponent<RectTransform>();
        slider.SetActive(false);

        GameManager.Instance.Events.AddListener<Reload>(this.gameObject, (obj) =>
        {
            gameObject.SetActive(true);
            StartCoroutine(ReloadAnimation(obj.Duration));
        });

        gameObject.SetActive(false);
    }

    private IEnumerator ReloadAnimation(float duration)
    {
        slider.SetActive(true);
        float t = 0;
        sliderTransform.anchoredPosition = sliderBegin.position;

        while(t < duration)
        {
            t += Time.deltaTime;
            sliderTransform.anchoredPosition = Vector3.Lerp(sliderBegin.anchoredPosition, sliderEnd.anchoredPosition, t / duration);

            yield return null;
        }
        slider.SetActive(false);
        gameObject.SetActive(false);
    }
}
