using System;
using System.Threading.Tasks;
using UnityEngine;

public class RewardCallbacksManager : MonoBehaviour
{
    private Action rewardAction;

    public bool IsFree { get { return rewardAction == null; } }

    public static RewardCallbacksManager Instance { get; private set; }

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
    }

    public void SetRewardAction(Action action)
    {
        rewardAction = action;
    }

    public void ShowReward()
    {
        Debug.Log("Entered Callback ShowReward");
        if (rewardAction == null)
        {
            Debug.Log("callback reward action is null");
            return;
        }

        try
        {
            Debug.Log("Entered Callback try block");
            Task.Run(rewardAction);

            rewardAction = null;
        }
        catch
        {
            rewardAction = null;
            Debug.Log("Show reward task Error");
        }
    }
}
