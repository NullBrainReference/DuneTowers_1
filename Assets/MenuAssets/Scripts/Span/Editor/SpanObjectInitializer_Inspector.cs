using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpanObjectInitializer), true)]
public class SpanObjectInitializer_Inspector : Editor
{

    public override void OnInspectorGUI()
    {

        var targetClass = (SpanObjectInitializer)target;

        EditorGUILayout.BeginHorizontal();
        //GUILayout.Toggle(isProcess, "isProcess");

        if (GUILayout.Button("Clear") == true)
        {
            targetClass.periodYandexButtons.Clear();
        }
        if (GUILayout.Button("Fill") == true)
        {
            var spanObjectUpdaters = GameObject.FindObjectsOfType(typeof(SpanObjectUpdater),true);

            foreach(var obj in spanObjectUpdaters)
            {
                Debug.Log(obj);
                var spanObjectUpdater = ((SpanObjectUpdater)obj);//.GetComponent<SpanObjectUpdater>();
                targetClass.periodYandexButtons.Add(spanObjectUpdater.PeriodButton);
            }
        }

        EditorGUILayout.EndHorizontal();



        EditorGUILayout.Space();
        base.OnInspectorGUI();
        serializedObject.Update();
    }

}


