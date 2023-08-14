using UnityEngine;
using System;
using System.Collections;

public class CameraMovement2D : MonoBehaviour
{
    //[SerializeField]
    public float sensitivity = 0.01f;
    public float criticalOffsetDeltaPos = 0.05f;
    [SerializeField]
    private TouchController touchController;

    private Vector3 prevMousePos;
    //public Transform mainCamParent;

    //private Vector2 screenSize = new Vector3(Screen.width, Screen.height);

    public static CameraMovement2D Instance { get; private set; }
    public bool moveming;

    private void Awake()
    {
        Instance = this;

    }

    private IEnumerator Start()
    {
        while (!(MapManager.Instance?.Initialazed ?? false))
            yield return null;

        float factorX = CellsManager.Instance.size.x / CellsManager.Instance.size.y * 1.4f;
        float factorY = CellsManager.Instance.size.y / CellsManager.Instance.size.x * 1.4f;

        minWorld = new Vector2(CellsManager.Instance.size.x * factorX, CellsManager.Instance.size.y * factorY) / -2f;
        maxWorld = -minWorld;

        UpdatePos(1);
    }

    public Vector2 minWorld;
    public Vector2 maxWorld;

    public Vector2 curMinWorld;
    public Vector2 curMaxWorld;

    private void Update()
    {

        if (Input.touchCount != 0)
        {
            MoveTouchMobile();
        }
        else
        {
            MoveMouse();
        }

        float multiply = 150f / Camera.main.orthographicSize;
        UpdatePos(multiply);

    }


    private void UpdatePos(float multiply)
    {
        curMaxWorld = (Vector2)Camera.main.ScreenToWorldPoint(touchController.maxTouchZone) - maxWorld;
        curMinWorld = minWorld - (Vector2)Camera.main.ScreenToWorldPoint(touchController.minTouchZone);

        if (curMaxWorld.x < 0 && curMinWorld.x > 0)
            transform.position = new Vector3((transform.position.x - curMaxWorld.x / multiply), transform.position.y, transform.position.z);

        if (curMinWorld.x < 0 && curMaxWorld.x > 0)
            transform.position = new Vector3((transform.position.x + curMinWorld.x / multiply), transform.position.y, transform.position.z);



        if (curMaxWorld.y < 0 && curMinWorld.y > 0)
            transform.position = new Vector3(transform.position.x, (transform.position.y - curMaxWorld.y / multiply), transform.position.z);

        if (curMinWorld.y < 0 && curMaxWorld.y > 0)
            transform.position = new Vector3(transform.position.x, (transform.position.y + curMinWorld.y / multiply), transform.position.z);
    }

    private float GetSensitivity()
    {
        //print(Screen.dpi);
        return sensitivity * Screen.dpi ;
        //return sensitivity * 720f / Screen.width;
    }

    public void ResetPosition()
    {
        transform.position = Vector3.zero;
    }

    private void MoveMouse()
    {      

        if (Input.GetMouseButtonDown(0))
            prevMousePos = Input.mousePosition;
        else if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 deltaPos = -(mousePos - prevMousePos) * GetSensitivity();
            //print("sqrMagnitude=" + deltaPos.sqrMagnitude.ToString("F5"));

            //moveming |= deltaPos.sqrMagnitude != 0;
            //if (deltaPos.sqrMagnitude > criticalOffsetDeltaPos) print("sqrMagnitude=" + deltaPos.sqrMagnitude.ToString("F5"));
            moveming |= deltaPos.sqrMagnitude > criticalOffsetDeltaPos;
            if (moveming)
            {
                Vector3 pos = transform.position;

                pos.x += deltaPos.x;
                pos.y += deltaPos.y;
                //pos.z = 0f;

                transform.position = pos;

                //mainCamParent.localEulerAngles = rot;
            }
            prevMousePos = mousePos;

        }

        if (Input.GetMouseButtonUp(0))
        {
            moveming = false;
        }

    }


    private int? fingerId = null;
    private void MoveTouchMobile()
    {
        if (Input.touchCount > 1)
        {
            fingerId = null;
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle finger movements based on touch phase.
            switch (touch.phase)
            {
                // Record initial touch position.
                case TouchPhase.Began:
                    prevMousePos = Input.mousePosition;
                    if (fingerId == null) fingerId = touch.fingerId;
                    break;

                // Determine direction by comparing the current touch position with the initial one.
                case TouchPhase.Moved:
                    if (fingerId == touch.fingerId)
                    {

                        Vector3 mousePos = touch.position;
                        Vector2 deltaPos = -(mousePos - prevMousePos) * GetSensitivity();
                        //moveming |= deltaPos.sqrMagnitude != 0;
                        moveming |= deltaPos.sqrMagnitude > criticalOffsetDeltaPos;
                        if (moveming)
                        {
                            Vector3 pos = transform.position;

                            pos.x += deltaPos.x;
                            pos.y += deltaPos.y;
                            //pos.z = 0f;

                            transform.position = pos;
                        }
                        prevMousePos = mousePos;
                    }
                    break;

                // Report that a direction has been chosen when the finger is lifted.
                case TouchPhase.Ended:
                    if (fingerId == touch.fingerId)
                    {
                        fingerId = null;
                        moveming = false;
                    }
                    break;
            }
        }

    }
}
