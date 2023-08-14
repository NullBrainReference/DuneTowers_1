using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }
    public static List<Action> InitActions = new List<Action>();

    //public Color[] playerColors;
    private void Awake()
    {
        Instance = this;
        InitObjectsOnAwake();
    }

    public void InitObjectsOnAwake()
    {
        foreach(Action action in InitActions)
        {
            action.Invoke();
        }
        InitActions.Clear();
    }

}
