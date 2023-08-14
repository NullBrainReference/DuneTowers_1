using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom2D : MonoBehaviour
{
    public float scaleMouse = 0.25f;
    public float scaleMobile = 0.01f;


    private float maxZoom;// = -4f;
    private float minZoom;// = -18f;

    private Vector3 centerPos;

    public static Zoom2D Instance { get; private set; }

    // заблокированная по краям зона
    //public int lockZone = 100;
    //public int minTouchZone;
    //public int maxTouchZone;
    //public RectTransform touchZone;
    [SerializeField]
    private TouchControllerBase touchControllerBase;


    public bool zooming;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        maxZoom = Screen.height / touchControllerBase.maxTouchZoneSize.y * 5.5f;
        minZoom = maxZoom / 2f;

        centerPos = Camera.main.transform.position;

        Centralize();
    }

    private void Update()
    {

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
        ZoomMouse();

#else
        //UNITY_ANDROID || UNITY_IOS
        ZoomTouchMobile();
#endif

        if (Input.GetKeyDown(KeyCode.Space))
            Centralize();

    }

    public void Centralize()
    {
        SetPos(10000, 1);

        Camera.main.transform.position = centerPos;
    }

    private void ZoomMouse()
    {

        //Touch touch0 = new Touch();
        //Touch touch1 = new Touch();
        //touch0.position.Set(1,1);
        //touch1.position = Input.mousePosition;



        //float touchDelta = (touch0.position - touch1.position).sqrMagnitude;
        //xTouchDelta = par1;
        //par1 = touchDelta;
        //par2 = (touchDelta - xTouchDelta) * 0.000005f;
        //SetPos((touchDelta - xTouchDelta), scaleMobile);

        if (!touchControllerBase.InTouchZone(Input.mousePosition))
            return;

        SetPos(Input.mouseScrollDelta.y, -scaleMouse);

        zooming = Input.mouseScrollDelta.y != 0;

    }


    //public float par1;
    //public float par2;
    private float xTouchDelta;
    private void ZoomTouchMobile()
    {

        // If there are two touches on the device...

        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (!zooming) {
                //if (!InRange(minTouchZone, maxTouchZone, touch0.position.y) || !InRange(minTouchZone, maxTouchZone, touch1.position.y)) { }
            }    
            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touchOnePrevPos = touch1.position - touch1.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;
            zooming |= touchDeltaMag != 0;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // ... change the orthographic size based on the change in distance between the touches.


            SetPos(deltaMagnitudeDiff, -scaleMobile);


            //camera1.orthographicSize += deltaMagnitudeDiff * scaleMobile;

            //// Make sure the orthographic size never drops below zero.
            //camera1.orthographicSize = Mathf.Max(camera1.orthographicSize, 0.1f);

            //camera1.orthographicSize = Mathf.Clamp(camera1.orthographicSize, minZoom, maxZoom);
            //SetPos((touch0.deltaPosition - touch1.deltaPosition).magnitude, 0.001f);


            //float touchDelta = (touch0.position - touch1.position).sqrMagnitude;
            //xTouchDelta = par1;
            //par1 = touchDelta;
            //par2 = (touchDelta - xTouchDelta)*0.0001f;

            //par1 = deltaMagnitudeDiff;
            //par2 = deltaMagnitudeDiff * scaleMobile;

        }
        else
        {
            zooming = false;
            //par1 = 0;
            //par2 = 0;
        }


    }

    internal void SetCameraMaxDistance(float maxCameraDistance)
    {
        this.minZoom = -maxCameraDistance;
        SetPos(0, 0);
    }

    //private bool InRange(int minLockZone, int maxLockZone, float value)
    //{
    //    return minLockZone <= value && value <= maxLockZone; 
    //}

    void SetPos(float _scrollDelta, float _scale)
    {
        float pos = Camera.main.orthographicSize;
        

        pos += _scrollDelta * _scale;
        
        
        pos = Mathf.Clamp(pos, minZoom, maxZoom);



        Camera.main.orthographicSize = pos;
    }


}
