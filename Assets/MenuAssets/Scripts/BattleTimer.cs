using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTimer : MonoBehaviour
{
    [SerializeField] private float startTime;
    [SerializeField] private float topTime;

    [SerializeField] private float totalTime;

    [SerializeField] private float currentTime;
    private float CurrentTime {
        get { return currentTime; }
        set 
        { 
            currentTime = value;
            if (currentTime < 0) currentTime = 0;
        }
    }

    public float TotalTime { get { return totalTime; } }

    private void FixedUpdate()
    {
        CurrentTime -= Time.deltaTime;
        totalTime += Time.deltaTime;
    }

    public void InitTimer()
    {
        currentTime = startTime;

        totalTime = 0;
    }

    public float GetStage()
    {
        return CurrentTime / topTime;
    }
}
