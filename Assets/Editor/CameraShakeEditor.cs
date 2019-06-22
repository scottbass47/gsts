using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleCameraShake))]
public class CameraShakeEditor : Editor
{
    
    private float duration = 1f;
    private NoiseSettings noise;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SimpleCameraShake shake = (SimpleCameraShake)target;


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Preview parameters");
        duration = EditorGUILayout.FloatField("Duration", duration);

        noise = (NoiseSettings) EditorGUILayout.ObjectField("Noise", noise, typeof(NoiseSettings), true);

        if(GUILayout.Button("Preview"))
        {
            shake.EditorPreviewShake(duration, noise);
        }
    }
}
