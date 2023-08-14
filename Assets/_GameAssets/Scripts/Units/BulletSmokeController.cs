using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSmokeController : MonoBehaviour
{

     public Vector3 direction;
    public float speed = 1;
 
    // Update is called once per frame
    private void Update()
    {
        Vector3 newPos = transform.position + (direction * speed * Time.deltaTime);
        newPos.z = transform.position.z;
        transform.position = newPos;
    }



}
