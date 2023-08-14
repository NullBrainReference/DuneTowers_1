using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDestroer : MonoBehaviour
{

    public float timeDelay=1.5f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeDelay);        
    }

}
