//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TankDeviceTargetController), true)]
public class TankDeviceTargetController_Inspector : Editor
{
    public override void OnInspectorGUI()
    {

        var targerClass = (TankDeviceTargetController)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowField") == true)
        {
            
            CourseSearcher courseSearcher = targerClass.GetCourseSearcher(DeviceController.Instance(1).Units, DeviceController.Instance(1).Obstructions);
            foreach(var item in courseSearcher.MasToList())
            {
                var cell = CellsManager.Instance.cellControllersArr[item.x, item.y];
                cell.textMesh.text = $"{item.z}";
                cell.textMesh.gameObject.SetActive(true);
            }
        }



        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        base.OnInspectorGUI();
        serializedObject.Update();
    }
}
