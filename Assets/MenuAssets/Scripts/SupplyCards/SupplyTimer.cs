using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyTimer
{
    private float time;
    public float startTime;

    public bool IsEnded { 
        get 
        { 
            if (time <= 0) return true;
            else return false;
        } 
    }

    public void TimerUpdate()
    {
        time -= Time.deltaTime;
    }

    public void SetUp()
    {
        time = startTime;
    }
}
