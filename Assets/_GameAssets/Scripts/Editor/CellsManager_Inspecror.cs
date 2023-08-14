using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CanEditMultipleObjects]
[CustomEditor(typeof(CellsManager), true)]
public class CellsManager_Inspector : Editor
{
    //private GetFiles getFiles;
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        var targerClass = (CellsManager)target;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Show pos") == true)
        {
            foreach(var cell in targerClass.cellControllers)
            {
                cell.textMesh.text = $"{cell.position.x}|{cell.position.y}";
                cell.textMesh.gameObject.SetActive(true);
            }
        }
        if (GUILayout.Button("Show cred") == true)
        {
            foreach (var cell in targerClass.cellControllers)
            {
                cell.textMesh.text = $"{((int)cell.Credits)}";
                cell.textMesh.gameObject.SetActive(true);
            }
        }
        if (GUILayout.Button("Hide all") == true)
        {
            foreach (var cell in targerClass.cellControllers)
            {
                cell.textMesh.gameObject.SetActive(false);
            }
        }


        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        base.OnInspectorGUI();
        serializedObject.Update();
    }


}
