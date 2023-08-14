#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CanEditMultipleObjects]
[CustomEditor(typeof(SetObjects), true)]
public class SetObjects_Inspector : UnityEditor.Editor
{
    //private GetFiles getFiles;
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        SetObjects targerClass = (SetObjects)target;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("UpdatePosition") == true)
        {
            targerClass.TestUpdateObjects();
        }


        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        base.OnInspectorGUI();
        serializedObject.Update();
    }


}

#endif