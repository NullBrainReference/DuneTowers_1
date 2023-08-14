using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Yandex : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    public static extern void ShowAdv();

    //[DllImport ("__Internal")]
    //public static extern void ShowReward(string nameValue);

    [DllImport("__Internal")]
    public static extern void ShowReward();
}
