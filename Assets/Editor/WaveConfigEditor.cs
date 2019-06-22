//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using UnityEditorInternal;

//[CustomEditor(typeof(WaveConfig))]
//public class WaveConfigEditor : Editor 
//{
//    //private ReorderableList list;

//    private WaveConfig waveConfig => (WaveConfig)target;

//    //private float lineHeight;
//    //private float lineHeightSpace;

//    //private void OnEnable()
//    //{
//    //    lineHeight = EditorGUIUtility.singleLineHeight;
//    //    lineHeightSpace = lineHeight + 10;

//    //    list = new ReorderableList(serializedObject, serializedObject.FindProperty("waves"), true, true, true, true);


//    //    list.drawElementCallback += (rect, index, isActive, isFocused) =>
//    //    {
//    //        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

//    //        rect = new Rect(rect.x, rect.y, rect.width, rect.height);
//    //        EditorGUI.PropertyField(rect, element, GUIContent.none);
//    //    };

//    //    list.elementHeightCallback = (index) =>
//    //    {
//    //        var elem = list.serializedProperty.GetArrayElementAtIndex(index);
//    //        var inner = elem.FindPropertyRelative("subWaves");
//    //        return (inner.arraySize + 4) * EditorGUIUtility.singleLineHeight;
//    //    };
//    //}

//    SerializedProperty waves;
//    private void OnEnable()
//    {
//    }

//    public override void OnInspectorGUI()
//    {
//        //serializedObject.Update();
//        //list.DoLayoutList();
//        //serializedObject.ApplyModifiedProperties();

//        waves = serializedObject.FindProperty("waves");

//        int waveNum = 1;
//        for (int i = 0; i < waveConfig.Waves.Count; i++)
//        {
//            var wave = waveConfig.Waves[i];
//            GUILayout.BeginHorizontal();

//            GUILayout.Label($"Wave {waveNum}", EditorStyles.boldLabel);
//            waveNum++;

//            GUILayout.FlexibleSpace();

//            if (GUILayout.Button("^"))
//            {
//                if (waveNum != 0)
//                {
//                    var w1 = waveConfig.Waves[waveNum - 1];
//                    waveConfig.Waves[waveNum - 1] = waveConfig.Waves[waveNum - 2];
//                    waveConfig.Waves[waveNum - 2] = w1;
//                }
//            }

//            if (GUILayout.Button("v"))
//            {

//            }

//            if (GUILayout.Button("-"))
//            {

//            }

//            GUILayout.EndHorizontal();

//            EditorGUILayout.PropertyField(waves.GetArrayElementAtIndex(i));
//        }

//        if (GUILayout.Button("Add Wave"))
//        {
//            waveConfig.Waves.Add(new Wave());
//        }

//    }
//}

//using UnityEditor;
//using UnityEngine;

//[CustomPropertyDrawer(typeof(EnemyData))]
//public class EnemyDataDrawer : PropertyDrawer
//{

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        EditorGUI.BeginProperty(position, label, property);
//        EditorGUI.indentLevel = 0;

//        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

//        var enemyType = property.FindPropertyRelative("type");
//        var amount = property.FindPropertyRelative("amount");

//        var typeRect = new Rect(position.x, position.y, 60, position.height);
//        var amountRect = new Rect(position.x + 65, position.y, 20, position.height);

//        EditorGUI.PropertyField(typeRect, enemyType);
//        //EditorGUI.PropertyField(amountRect, amount);

//        EditorGUI.EndProperty();
//    }

//}

//[CustomPropertyDrawer(typeof(Wave))]
//public class WaveDrawer : PropertyDrawer
//{

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        var subWaves = property.FindPropertyRelative("subWaves");
//        for (int i = 0; i < subWaves.arraySize; i++)
//        {
//            var subWave = subWaves.GetArrayElementAtIndex(i);
//            EditorGUI.LabelField(position, "fuck");
//        }
//    }

//    //private Dictionary<string, ReorderableList> lists = new Dictionary<string, ReorderableList>();

//    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    //{
//    //    EditorGUI.BeginProperty(position, label, property);

//    //    ReorderableList list = null;
//    //    if(!lists.TryGetValue(property.propertyPath, out list))
//    //    {
//    //        list = CreateWaveList(property.FindPropertyRelative("subWaves"));
//    //        lists.Add(property.propertyPath, list);
//    //    }
//    //    list = lists[property.propertyPath];

//    //    var listRect = new Rect(position.x, position.y + 30, position.width, EditorGUIUtility.singleLineHeight * 10);
//    //    list.DoList(position);

//    //    EditorGUI.EndProperty();
//    //}

//    //private ReorderableList CreateWaveList(SerializedProperty property)
//    //{
//    //    ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, true);

//    //    list.drawElementCallback = (rect, index, isActive, isFocused) =>
//    //    {
//    //        EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index), true);
//    //    };

//    //    list.elementHeightCallback = (index) =>
//    //    {
//    //        var elem = list.serializedProperty.GetArrayElementAtIndex(index);
//    //        var inner = elem.FindPropertyRelative("subWaves");
//    //        return 4 * EditorGUIUtility.singleLineHeight;
//    //    };

//    //    return list;
//    //}
//}
