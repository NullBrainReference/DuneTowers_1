using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempObjectsManager : MonoBehaviour
{
    public static TempObjectsManager Instance { get; private set; }

    public float heightOffset;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;    
    }


}
