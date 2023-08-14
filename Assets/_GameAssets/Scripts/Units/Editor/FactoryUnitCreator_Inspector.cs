using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(FactoryUnitCreator), true)]
public class FactoryUnitCreator_Inspector : Editor
{
    public override void OnInspectorGUI()
    {

        var targerClass = (FactoryUnitCreator)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Constract") == true)
        {
            targerClass.CreateUnit();
        }



        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        base.OnInspectorGUI();
        serializedObject.Update();
    }

}
