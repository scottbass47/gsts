using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{

    [SerializeField] private GameObject healthUnit;
    [SerializeField] private Sprite healthFull;
    [SerializeField] private Sprite healthEmpty;

    private GameObject[] healthUnits;
        
    // Start is called before the first frame update
    public void Start()
    {
        var playerStats = GetComponentInParent<HUDUtils>().PlayerStats;
        healthUnits = new GameObject[playerStats.MaxHealth];

        for(int i = 0; i < healthUnits.Length; i++)
        {
            var obj = Instantiate(healthUnit, Vector3.zero, Quaternion.identity, transform);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero + Vector2.right * i * 20;
            rect.anchoredPosition3D = rect.anchoredPosition;
            healthUnits[i] = obj;
        }

        var events = GameManager.Instance.Events;
        events.AddListener<PlayerHealthEvent>(this.gameObject, (health) => 
        {
            int newHealth = health.Health;
            if (newHealth < 0 || newHealth > healthUnits.Length) return;

            for(int i = 0; i < healthUnits.Length; i++)
            {
                var unit = healthUnits[i];
                var image = unit.GetComponent<Image>();
                image.sprite = i < newHealth ? healthFull : healthEmpty; 
            }
        });
    }
}
