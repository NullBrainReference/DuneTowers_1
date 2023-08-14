using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUrlUtil  
{
    public static string GetTransformFullName(Transform t)
    {
        string result = t.name;
        int i = 0;

        Transform curTransform = t;
        while (curTransform.parent != null && i < 20)
        {
            i++;
            if (result != "") result = "/" + result;
            result = curTransform.parent.name + result;
            curTransform = curTransform.parent;
        }
        return result;
    }
}
