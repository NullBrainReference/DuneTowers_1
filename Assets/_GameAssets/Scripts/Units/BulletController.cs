using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    public float powerBullet;
    public Vector3 direction;
    public float speed = 1;
    public UnitController parentUnitController;
    public GameObject explosion;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    private void Update()
    {
       
        Vector3 newPos = transform.position + (direction * speed * Time.deltaTime);
        newPos.z = transform.position.z;
        transform.position = newPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        UnitController other = collision.GetComponent<UnitController>();
        if (other != null)
        {
            //print("OnTriggerEnter2D 123");
            if (other == parentUnitController)
                return;

            if(other.PlayerNo == parentUnitController.PlayerNo)
                return;


            other.HitUnit(powerBullet);
            Instantiate(explosion, transform.position, transform.rotation, transform.parent);
            
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //print("OnTriggerExit2D=" + collision.name);
        var cannonController = collision.GetComponent<CannonController>();
        if (cannonController != null)
            if (parentUnitController == cannonController.unitController)
                OnDestroy();

    }


    internal void OnDestroy()
    {
        Destroy(gameObject,0.5f);
    }
}
