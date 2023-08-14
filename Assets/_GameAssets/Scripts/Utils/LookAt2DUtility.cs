using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt2DUtility
{
    public static bool LookAt2D(Transform transform, Vector3 lookTarget, float speed)
    {
        Quaternion dir = GetDirection(transform, lookTarget, transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, dir, speed);

        return Quaternion.Angle(dir, transform.rotation) < 2;
    }

    private static Quaternion GetDirection(Transform transform, Vector3 lookTarget, Vector3 position)
    {



        // the direction we want the X axis to face (from this object, towards the target)
        Vector3 xDirection = (lookTarget - transform.position).normalized;

        // Y axis is 90 degrees away from the X axis
        Vector3 yDirection = Quaternion.Euler(0, 0, 90) * xDirection;


        // Z should stay facing forward for 2D objects
        Vector3 zDirection = Vector3.forward;

        return Quaternion.LookRotation(zDirection, yDirection);



        // apply the rotation to this object
        //transform.rotation = Quaternion.LookRotation(zDirection, yDirection);
        //return Quaternion.LookRotation(zDirection, yDirection);
    }
}
