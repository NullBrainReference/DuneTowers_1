using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AchievementType { 
    None, Destroy, Create, 
    Gold, Power, Armor, 
    UnitLevel, PlayerLevel, 
    DefenceTime, LevelComplete, LevelTime}

[System.Serializable]
public class AchievementBase
{
    [SerializeField] private int key;

    [SerializeField] private string title;
    [SerializeField] private string titleRu;
    [SerializeField] private string description;
    [SerializeField] private string descriptionRu;

    [SerializeField] private float goal;
    [SerializeField] private float progress;

    [SerializeField] private bool hasProgress;

    [SerializeField] private bool achieved;

    [SerializeField] private AchievementType achievementType;

    [SerializeField] private CoinType coinType;
    [SerializeField] private int rewardValue;
    [SerializeField] private bool rewardTaken;

    public int Key { get { return key; } }

    public float Goal { get { return goal; } }
    public float Progress { get { return progress; } }

    public string Title { get { return title; } }
    public string TitleRu { get { return titleRu; } }
    public string Description { get { return description; } }
    public string DescriptionRu { get { return descriptionRu; } }

    public bool Achieved { get { return achieved; } }

    public bool HasProgress { get { return hasProgress; } }

    public AchievementType AchievementType { get { return achievementType; } }
    
    public CoinType CoinType { get { return coinType; } }
    public int RewardValue { get { return rewardValue; } }
    public bool RewardTaken { get { return rewardTaken; } }

    public void AddProgress(float value)
    {
        if (achieved) return;

        progress += value;

        if(progress >= goal)
        {
            achieved = true;
            progress = goal;
        }

        Save();
    }

    public void SetEqualProgress(float value)
    {
        if (achieved) return;

        if (value == goal)
        {
            achieved = true;
            progress = goal;
        }
        else
        {
            return;
        }

        Save();
    }

    public void GiveReward()
    {
        switch (coinType) 
        {
            case CoinType.Gold:
                MenuManager.Instance.PlayerStats.goldCoins += rewardValue;
                break;
            case CoinType.Armor:
                MenuManager.Instance.PlayerStats.armorCoins += rewardValue;
                break;
            case CoinType.Power:
                MenuManager.Instance.PlayerStats.powerCoins += rewardValue;
                break;
        }

        rewardTaken = true;

        Save();

        MenuManager.Instance.PlayerStats.SavePlayer();
        MenuManager.Instance.TopPanelController.Animate();
    }

    public void CompareAndSetProgress(float value)
    {
        if (achieved)
            return;

        if (value < progress)
            return;

        progress = value;

        if (progress >= goal)
        {
            achieved = true;
            progress = goal;
        }

        Save();
    }

    public static AchievementBase GetAchievement(int key)
    {
        string text = PlayerPrefs.GetString("Achievement" + key);
        AchievementBase achievement = JsonUtility.FromJson<AchievementBase>(text);

        //Debug.Log("Level_Loaded");
        return achievement;
    }

    public void Save()
    {
        string achievement = JsonUtility.ToJson(this);

        PlayerPrefs.SetString("Achievement" + key, achievement);
    }
}
