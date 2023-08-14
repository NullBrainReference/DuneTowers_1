using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Raycast2DUtility
{
    public static GameObject GetHitObject(int layerMask)
    {
        Camera c = Camera.main;


        bool pointerRaycastUIExists = PointerRaycastUILayerExists(Input.mousePosition);

        if (pointerRaycastUIExists) return null;

        

        Ray ray = c.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 15f, layerMask);




        //if (hit != null)
       // {
        return hit.transform?.gameObject;
        //}
        //int layerMask = LayerMask.GetMask("Mine");

        //if (Physics.Raycast(ray, out RaycastHit hit, 50, layerMask))
        //{

        //    return hit.transform.gameObject;
        //}



        //return null;
    }

    public static bool PointerRaycastUILayerExists(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        List<RaycastResult> resultsData = new List<RaycastResult>();
        pointerData.position = position;
        EventSystem.current.RaycastAll(pointerData, resultsData);

        int layer = LayerMask.NameToLayer("UI");
        return resultsData.Exists(x => x.gameObject.layer == layer);
        //if (resultsData.Count > 0)
        //{
        //    return resultsData[0].gameObject;
        //}

        //return null;
    }

    public static GameObject GetObjectPointerRaycastUI(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        List<RaycastResult> resultsData = new List<RaycastResult>();
        pointerData.position = position;
        EventSystem.current.RaycastAll(pointerData, resultsData);

        if (resultsData.Count > 0)
        {
            return resultsData[0].gameObject;
        }

        return null;
    }

}
