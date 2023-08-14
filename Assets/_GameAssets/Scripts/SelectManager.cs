//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SelectManager : MonoBehaviour
//{
//    public GameObject selectObject;


//    [SerializeField]
//    private UnitController curUnit;

//    public static SelectManager Instance { get; private set; }

//    private void Awake()
//    {
//        Instance = this;    
//    }

//    internal void SelectObject(Transform objTransform)
//    {
//        //this.curUnit = curUnit;
//        //transform.position = objTransform.position;
//        //selectObject.SetActive(true);
//    }


//    internal void UnselectObject()
//    {
//        curUnit = null;
//        //selectObject.SetActive(false);        
//    }


//    internal void UnselectObject(float time)
//    {
//        StopAllCoroutines();
//        StartCoroutine(UnselectObjectTime(time));
//    }

//    private IEnumerator UnselectObjectTime(float time)
//    {
//        yield return new WaitForSecondsRealtime(time);
//        UnselectObject();
//    }

//    public void Follow(UnitController curUnit, float time)
//    {
//        this.curUnit = curUnit;
//        //StopAllCoroutines();
//        StartCoroutine(FollowCorutine(curUnit, time));
//    }

//    private IEnumerator FollowCorutine(UnitController curUnit, float time)
//    {
//        float startTime = Time.realtimeSinceStartup;
//        //this.curUnit = curUnit;

//        while (true)

//        //    while (Time.realtimeSinceStartup - startTime > time)

//        {
//            yield return new WaitForEndOfFrame();
//            if (curUnit != null)
//            {
//                selectObject.transform.position = curUnit.transform.position;
//            }
//            else
//            {
//                UnselectObject();
//                StopAllCoroutines();
//            }
//        }

//        UnselectObject();

//    }


    

//    //public void UnselectObject()
//    //{

//    //}
//}
