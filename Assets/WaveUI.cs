using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveNum;
    [SerializeField] private TextMeshProUGUI enemiesLeft;

    private void Start()
    {
        gameObject.SetActive(false);

        var events = GameManager.Instance.Events;

        events.AddListener<WaveStarted>((obj) =>
        {
            waveNum.text = $"Wave {obj.WaveNum}";
            gameObject.SetActive(true);
        });

        events.AddListener<WaveEnemyChange>((obj) =>
        {
            enemiesLeft.text = $"{obj.EnemiesLeft}";
        });

        events.AddListener<WaveEnded>((obj) =>
        {
            gameObject.SetActive(false);
        });
    }
}
