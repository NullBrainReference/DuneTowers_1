using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnonShadow : MonoBehaviour
{
    [System.Serializable]
    public struct Area
    {
        public float min;
        public float max;
        public bool ValueInArea(float value)
        {
            return min < value && value < max;
        }
    }

    public UnityEngine.Transform parent;

    public Area[] restrictedArea;
    public SpriteRenderer spriteRenderer;

    //public bool valueInArea;
    //public float z;

    // Update is called once per frame
    void Update()
    {

        //z = parent.rotation.eulerAngles.z;
        bool valueInArea = false;
        foreach (Area area in restrictedArea) 
        {
            valueInArea |= area.ValueInArea(parent.rotation.eulerAngles.z);
        }
        
        if (!valueInArea) transform.rotation = parent.rotation;
        spriteRenderer.enabled = !valueInArea;
    }
}
