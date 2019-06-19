using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using System;

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
        StartCoroutine(_Shake(settings.Duration, settings.Amplitude, settings.Frequency));
    }

    public void EditorPreviewShake(float duration, float amplitude, float frequency)
    {
        StopAllCoroutines();
        StartCoroutine(_Shake(duration, amplitude, frequency));
    }

    private IEnumerator _Shake(float duration, float amplitude, float frequency)
    {
        float elapsed = 0;
        virtualCameraNoise.m_AmplitudeGain = amplitude;
        virtualCameraNoise.m_FrequencyGain = frequency;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        virtualCameraNoise.m_AmplitudeGain = 0f;
        virtualCameraNoise.m_FrequencyGain = 0f;
    }
}
