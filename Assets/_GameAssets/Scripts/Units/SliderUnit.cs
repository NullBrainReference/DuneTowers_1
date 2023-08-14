using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SliderUnit : MonoBehaviour
{
#if UNITY_EDITOR   
    [Range(0f,1f)]
    public float value;
#endif
    [SerializeField]
    private Transform fill;

    [SerializeField]
    [Range(0f, 0.1f)]
    private float minValue=0;

#if UNITY_EDITOR   
    void Update()
    {
        SetValue(value);
    }
#endif

    public void SetValue(float value)
    {
#if UNITY_EDITOR   
        this.value = value;
#endif
        Vector3 scale = fill.localScale;
        scale.x = Mathf.Clamp(value, minValue, 1f);
        fill.localScale = scale;
    }

    

}
