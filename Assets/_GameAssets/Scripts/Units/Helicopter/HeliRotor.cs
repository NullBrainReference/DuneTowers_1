using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliRotor : MonoBehaviour
{
    [SerializeField] private float speed;

    private void FixedUpdate()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.rotation = new Quaternion(
            transform.rotation.x,
            transform.rotation.y,
            transform.rotation.z + speed * Time.deltaTime,
            transform.rotation.w
            );
    }
}
