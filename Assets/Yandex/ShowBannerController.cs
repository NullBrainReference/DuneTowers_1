using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBannerController : MonoBehaviour
{
    [SerializeField] private float currTime;

    [SerializeField] private float coolDown;

    public static ShowBannerController Instance { get; private set; } 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //currTime = coolDown;
    }

    private void FixedUpdate()
    {
        currTime -= Time.deltaTime;
    }

    public void CallBanner()
    {
        if (currTime > 0)
            return;

        try
        {
            Yandex.ShowAdv();
            PauseManager.Pause();
        }
        catch
        {

        }

        currTime = coolDown;
    }
}
