using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public enum TypeShake { Position, Rotation, Both }

    public bool shakeTestStart;

    [Space]
    public TypeShake typeShake;

    public float shakeTime = 1.5f;
    public float shakeDistPosition = 0.1f;
    public float shakeDistRotate = 1.5f;

    private float startTime;
    private Vector3 neutralPos = Vector3.zero;
    private Vector3 neutralRotate = Vector3.zero;

    private bool shakeNow = false;

    

    public static CameraShaker Instance { get; private set; }

    private void Awake()
    {
        Instance = this;    
    }

    private void Update()
    {
        if (shakeTestStart)
        {
            shakeTestStart = false;
            StartShake();
        }
    }

    public void StartShake()
    {
        if (!shakeNow) 
        {
            shakeNow = true;
            startTime = Time.time;
            //if (typeShake == TypeShake.Position) StartCoroutine(ShakePosition(shakeTime, shakeDistPos));
            StartCoroutine(Shake());
        }
        else
        {
            startTime = Time.time;
        }
        
    }


    //private IEnumerator ShakePosition(float shakeTime, float shakeDistance)
    //{
    //    shakeNow = true;
    //    //float startTime = Time.time;
    //    neutralPos = transform.position;

    //    while (Time.time < startTime + shakeTime)
    //    {
    //        float factor = 1 - (Time.time - startTime) / shakeTime;
    //        if (factor < 0) factor = 0;
    //        transform.position = neutralPos + Random.insideUnitSphere * shakeDistance * factor;

    //        yield return new WaitForEndOfFrame();
    //    }

    //    transform.position = neutralPos;
    //    shakeNow = false;
    //}



    private IEnumerator Shake()
    {
        shakeNow = true;

        neutralPos = transform.localPosition;

        Quaternion neutralRotationQuaternion = transform.localRotation;
        neutralRotate = neutralRotationQuaternion.eulerAngles;


        while (Time.time < startTime + shakeTime)
        {
            float factorTime = 1 - (Time.time - startTime) / shakeTime;
            if (factorTime < 0) factorTime = 0;

            UpdatePosition(factorTime);

            UpdateRotation(factorTime);
            
            yield return new WaitForEndOfFrame();
        }

        //neutralRotationQuaternion.eulerAngles = neutralRotate;
        transform.localRotation = neutralRotationQuaternion;

        transform.localPosition = neutralPos;

        shakeNow = false;
    }

    private void UpdateRotation(float factorTime)
    {
        if (typeShake == TypeShake.Rotation || typeShake == TypeShake.Both)
        {
            Quaternion rotation = new Quaternion
            {
                eulerAngles = neutralRotate + UnityEngine.Random.insideUnitSphere * shakeDistRotate * factorTime
            };
            transform.localRotation = rotation;
        }
    }

    private void UpdatePosition(float factor)
    {
        if (typeShake == TypeShake.Position || typeShake == TypeShake.Both)
        {
            transform.localPosition = neutralPos + UnityEngine.Random.insideUnitSphere * shakeDistPosition * factor;
        }
    }

}
