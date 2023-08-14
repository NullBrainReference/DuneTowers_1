using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private AchievementBase achievement;

    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;

    [SerializeField] private Text goalText;
    [SerializeField] private Text progressText;

    [SerializeField] private Slider progressSlider;

    [SerializeField] private GameObject rewardGroup;
    [SerializeField] private GameObject rewardImage;
    [SerializeField] private GameObject rewardText;
    [SerializeField] private GameObject progressGroup;


    public AchievementBase Achievement { get { return achievement; } } 

    public void UI_Init()
    {
        if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
        {
            titleText.text = achievement.TitleRu;
            descriptionText.text = achievement.DescriptionRu;
        }
        else
        {
            titleText.text = achievement.Title;
            descriptionText.text = achievement.Description;
        }

        progressGroup.SetActive(achievement.HasProgress);

        RewardGroupUpdate();
        SliderUpdate();
    }

    private void RewardGroupUpdate()
    {
        if (achievement.Achieved && !achievement.RewardTaken)
        {
            rewardGroup.SetActive(true);
            rewardImage.SetActive(false);
            rewardText.SetActive(true);
        }
        else if (achievement.Achieved && achievement.RewardTaken)
        {
            rewardGroup.SetActive(true);
            rewardImage.SetActive(false);
            rewardText.SetActive(false);
        }
        else
        {
            rewardGroup.SetActive(false);
        }
    }

    private void SliderUpdate()
    {
        if (achievement.Achieved)
            progressGroup.SetActive(false);

        progressSlider.maxValue = achievement.Goal;
        progressSlider.value = achievement.Progress;

        goalText.text = achievement.Goal.ToString();
        progressText.text = achievement.Progress.ToString();
    }

    public void Click()
    {
        if (achievement.Achieved == false) 
            return;
        if (achievement.RewardTaken == true) 
            return;

        ShowBannerController.Instance.CallBanner();

        achievement.GiveReward();

        RewardGroupUpdate();
        SliderUpdate();

        MenuManager.Instance.UiStatsUpdate();
    }

    public void SetAchievement(AchievementBase achievement)
    {
        this.achievement = achievement;
    }
}
