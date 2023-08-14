using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementScroll : MonoBehaviour
{
    [SerializeField] private GameObject content;

    [SerializeField] private GameObject prefab;

    public List<AchievementUI> achievements;

    private void Awake()
    {
        AchievementsManager.Instance.SetScroll(this);
        InitAchievements();
    }

    private void OnEnable()
    {
        if (achievements == null) 
            return;

        UpdateAchievements();
    }

    public void InitAchievements()
    {
        foreach (AchievementBase achievement in AchievementsManager.Instance.achievements)
        {
            AddAchievement(achievement);
        }
    }

    public void UpdateAchievements()
    {
        foreach(AchievementUI achievement in achievements)
        {
            achievement.UI_Init();
        }
    }

    public void AddAchievement(AchievementBase achievement)
    {
        GameObject newObj = Instantiate(prefab, content.transform);//Caused null ref

        AchievementUI achievementUI = newObj.GetComponent<AchievementUI>();
        achievementUI.SetAchievement(achievement);
        achievementUI.UI_Init();
        achievements.Add(achievementUI);
    }
}
