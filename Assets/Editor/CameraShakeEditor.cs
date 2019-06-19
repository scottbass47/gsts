using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleCameraShake))]
public class CameraShakeEditor : Editor
{
    
    private float duration = 1f;
    private float amplitude = 1f;
    private float frequency = 1f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SimpleCameraShake shake = (SimpleCameraShake)target;


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Preview parameters");
        duration = EditorGUILayout.FloatField("Duration", duration);
        amplitude = EditorGUILayout.FloatField("Amplitude", amplitude);
        frequency = EditorGUILayout.FloatField("Frequency", frequency);

        if(GUILayout.Button("Preview"))
        {
            shake.EditorPreviewShake(duration, amplitude, frequency);
        }
    }
}
