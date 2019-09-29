using Cinemachine;
using UnityEngine;
using UnityEngine.U2D;
using System.Reflection;

/// <summary>
/// Add this component to a camera that has PixelPerfectCamera and CinemachineBrain
/// components to prevent the active CinemachineVirtualCamera from overwriting the
/// correct orthographic size as calculated by the PixelPerfectCamera.
/// </summary>
[RequireComponent(typeof(PixelPerfectCamera), typeof(CinemachineBrain))]
class OrthographicOverride : MonoBehaviour
{
    CinemachineBrain CB;
    object Internal; // PixelPerfectCameraInternal
    FieldInfo OrthoInfo;

    void Start()
    {
        CB = GetComponent<CinemachineBrain>();
        Internal = typeof(PixelPerfectCamera).GetField("m_Internal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GetComponent<PixelPerfectCamera>());
        OrthoInfo = Internal.GetType().GetField("orthoSize", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    void LateUpdate()
    {
        var cam = CB.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (cam)
            cam.m_Lens.OrthographicSize = (float)OrthoInfo.GetValue(Internal);
    }
}
