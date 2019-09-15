using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimatedLight))]
public class AnimatedLightEditor : Editor
{
    private readonly string[] options = new string[] { "Intensity", "Color", "Inner Radius", "Outer Radius" };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var maskProperty = serializedObject.FindProperty("AnimationMask");
        maskProperty.intValue = EditorGUILayout.MaskField("Animation Mask", maskProperty.intValue, options);
        serializedObject.ApplyModifiedProperties();
    }
}
