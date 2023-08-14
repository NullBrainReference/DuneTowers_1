using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
#if UNITY_EDITOR
[CanEditMultipleObjects]

[CustomEditor(typeof(MenuManager), true)]

public class MenuManager_inspector : Editor

{
    public override void OnInspectorGUI()
    {
        var targerClass = (MenuManager)target;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save") == true)
        {
            targerClass.SavePlayer();
        }

        if (GUILayout.Button("InitPL") == true)
        {
            targerClass.InitPlayerStats();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("AddGold") == true)
        {
            targerClass.AddGoldCoins();
        }

        if (GUILayout.Button("AddArmor") == true)
        {
            targerClass.AddArmorCoins();
        }

        if (GUILayout.Button("AddPower") == true)
        {
            targerClass.AddPowerCoins();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (GUILayout.Button("InitPrices") == true)
        {
            targerClass.InitPrices();
        }

        base.OnInspectorGUI();
        serializedObject.Update();
    }
}

#endif
