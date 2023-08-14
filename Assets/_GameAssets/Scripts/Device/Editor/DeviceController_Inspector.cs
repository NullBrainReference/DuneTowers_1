using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(DeviceController), true)]
public class DeviceController_Inspector : Editor
{
    
    float time = 0.1f;

    public override void OnInspectorGUI()
    {

        var targerClass = (DeviceController)target;

        EditorGUILayout.BeginHorizontal();
        //GUILayout.Toggle(isProcess, "isProcess");

        if (GUILayout.Button("2") == true)
        {
            time = 2f;
            targerClass.StopAllCoroutines();
            targerClass.StartCoroutine(targerClass.RunProcess(time));
        }
        if (GUILayout.Button("1") == true)
        {
                time = 1f;
                targerClass.StopAllCoroutines();
                targerClass.StartCoroutine(targerClass.RunProcess(time));
        }
        if (GUILayout.Button("0.5") == true)
        {
                time = 0.5f;
                targerClass.StopAllCoroutines();
                targerClass.StartCoroutine(targerClass.RunProcess(time));
        }
        if (GUILayout.Button("0.2") == true)
        {
                time = 0.2f;
                targerClass.StopAllCoroutines();
                targerClass.StartCoroutine(targerClass.RunProcess(time));
        }
        if (GUILayout.Button("stop") == true)
        {
                time = 0f;
                targerClass.isProcess = false;
                targerClass.StopAllCoroutines();
        }

        GUILayout.TextField(time.ToString(), 25);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Run MainProcess") == true)
        {
            if (!targerClass.isProcess)
            {
                targerClass.isProcess = true;
                DateTime dateTime = DateTime.Now;
                targerClass.MainProcess();
                Debug.Log(((DateTime.Now - dateTime).TotalMilliseconds / 1000));
                targerClass.isProcess = false;
            }
        }



        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        base.OnInspectorGUI();
        serializedObject.Update();
    }

}


