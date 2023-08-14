using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationInstance : MonoBehaviour
{
    public static LocalizationInstance Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
