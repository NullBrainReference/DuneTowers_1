using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CanEditMultipleObjects]

[CustomEditor(typeof(AchievementsManager), true)]

public class AchievementsManager_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        var targerClass = (AchievementsManager)target;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save") == true)
        {
            targerClass.SaveAchievement();
            Debug.Log(targerClass.editAchievement.Key + "_Achevement_Saved");
        }

        if (GUILayout.Button("LoadAchievement") == true)
        {
            targerClass.editAchievement = AchievementsManager.LoadAchievement(targerClass.editAchievement.Key);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        base.OnInspectorGUI();
        serializedObject.Update();
    }
}

#endif