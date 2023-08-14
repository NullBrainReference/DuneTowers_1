using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControllerBase : MonoBehaviour {
    public float longClickTime = 0.4f;
    public float reactionTimeDelay = 0.5f;


    public RectTransform touchZoneSize;
    public RectTransform touchZone;
    public Vector2 minTouchZone;
    public Vector2 maxTouchZone;
    public Vector2 minTouchZoneSize;
    public Vector2 maxTouchZoneSize;


    public enum TypeTouch { NoTouch, LongTouch, ShortTouch }
    public TypeTouch typeTouch;

    private float reactionTime = 0f;
    private float? b0time;
    private bool longTouch = false;


    private void Awake()
    {
        Vector3[] worldCorners = new Vector3[4];
        touchZone.GetWorldCorners(worldCorners);
        minTouchZone = worldCorners[0]; 
        maxTouchZone = worldCorners[2];

        Vector3[] worldCornersSize = new Vector3[4];
        touchZoneSize.GetWorldCorners(worldCorners);
        minTouchZoneSize = worldCorners[0];
        maxTouchZoneSize = worldCorners[2];
    }

    // Update is called once per frame
    private void Update()
    {
        CheckTouchObject();
    }


    public bool InTouchZone(Vector2 point)
    {
        return InRange(point.x, minTouchZone.x, maxTouchZone.x) && InRange(point.y, minTouchZone.y, maxTouchZone.y);
    }

    private bool InRange(float value, float min, float max)
    {
        return min <= value && value <= max;
    }

    private void CheckTouchObject()
    {
        bool gameActive = GameActive();
        // проверяем наличие более приоритетных событий, и что игра активна
        if (Zoom2D.Instance.zooming || CameraMovement2D.Instance.moveming || reactionTime > Time.realtimeSinceStartup || !gameActive)
        {
            //сбрасываем все переменные т.к. это не нажание
            ResetValues();
            return;
        }

        // запоминаем когда нажали кнопку
        if (Input.GetMouseButtonDown(0))
        {
            b0time = Time.realtimeSinceStartup;
        }

        // если долго держим, то меняем тип клика на долгий 
        if (b0time != null)
        {
            float dT = Time.realtimeSinceStartup - (float)b0time;
            longTouch = dT > longClickTime;
            typeTouch = longTouch ? TypeTouch.LongTouch : TypeTouch.ShortTouch;
        }

        // Событие если клавишу отпустили или сработал "длинный клик" 
        if (Input.GetMouseButtonUp(0) || longTouch)
        {
            if (typeTouch != TypeTouch.NoTouch)
            {
                //получаем объект
                GameObject hitObject = Raycast2DUtility.GetHitObject(LayerMask.GetMask("MobileUnit"));

                if (!hitObject)
                    hitObject = Raycast2DUtility.GetHitObject(LayerMask.GetMask("Cell"));

                if (hitObject)
                {
                    OnTouch(typeTouch, hitObject);
                }

            }

            //сбрасываем все переменные т.к. нажатие или отработало или это не нажатие
            ResetValues();

        }
    }

    private void ResetValues()
    {
        typeTouch = TypeTouch.NoTouch;
        longTouch = false;
        b0time = null;
    }

    private void CalcReactionTime()
    {
        reactionTime = Time.realtimeSinceStartup + reactionTimeDelay;
    }

    protected virtual bool GameActive()
    {
        throw new NotImplementedException("Добавить проверку на активносить игры и меню в потомке - " + TransformUrlUtil.GetTransformFullName(transform));
    }

    protected virtual void OnTouch(TypeTouch typeTouch, GameObject obj)
    {
        throw new NotImplementedException("Добавить событие в потомке. " + TransformUrlUtil.GetTransformFullName(transform));
        
    }


}
