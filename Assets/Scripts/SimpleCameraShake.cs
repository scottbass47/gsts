using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using System;
using Random = UnityEngine.Random;

public class SimpleCameraShake : MonoBehaviour
{
    // Cinemachine Shake
    [SerializeField] private CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    // Screen shake settings
    [SerializeField] private ScreenShakeSettings playerDamage;
    [SerializeField] private ScreenShakeSettings weaponFired;

    // Use this for initialization
    private void Start()
    {
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        var eventManager = GameManager.Instance.Events;
        eventManager.AddListener<PlayerDamage>((obj) =>
        {
            Shake(playerDamage);
        });
        eventManager.AddListener<WeaponFired>((obj) =>
        {
            Shake(weaponFired);
        });
    }

    private void Shake(ScreenShakeSettings settings)
    {
        StopAllCoroutines();
        StartCoroutine(_Shake(settings.Duration, settings.Noise));
    }

    public void EditorPreviewShake(float duration, NoiseSettings noise)
    {
        StopAllCoroutines();
        StartCoroutine(_Shake(duration, noise));
    }

    private IEnumerator _Shake(float duration, NoiseSettings noise)
    {
        float elapsed = 0;
        virtualCameraNoise.m_NoiseProfile = noise;
        virtualCameraNoise.m_AmplitudeGain = 1f;
        virtualCameraNoise.m_FrequencyGain = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        virtualCameraNoise.m_AmplitudeGain = 0f;
        virtualCameraNoise.m_FrequencyGain = 0f;
    }
}
