using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

#if UNITY_EDITOR

[CanEditMultipleObjects]

[CustomEditor(typeof(TextGenerator), true)]
public class TextGenerator_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        var targerClass = (TextGenerator)target;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save") == true)
        {
            targerClass.WriteEdItem();
        }

        if (GUILayout.Button("Load") == true)
        {
            targerClass.LoadEdItem();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        base.OnInspectorGUI();
        serializedObject.Update();
    }
}

#endif