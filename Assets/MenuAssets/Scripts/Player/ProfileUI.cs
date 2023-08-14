using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour, IStatsUi
{
    private PlayerProfile profile;
    public PlayerProfile Profile { get { return profile; } }

    [SerializeField] private int stageScore;

    [SerializeField] private Text levelText;
    [SerializeField] private Text stageScoreText;
    [SerializeField] private Text progressScoreText;

    [SerializeField] private Slider progressSlider;

    [SerializeField] private Image profileImage;

    [SerializeField] private GameObject profilePanel;

    [SerializeField] private List<StatItemUI> statItems;
    //[SerializeField] private AchievementScroll achievementScroll;

    public string Tag { get; set; } = "Profile";

    private void Awake()
    {
        //AchievementsManager.Instance.SetScroll(achievementScroll);
        profile = PlayerProfile.LoadOrInitProfile();

        UI_update();
    }

    public void UI_update()
    {
        if (GameStats.Instance != null)
            this.profile = GameStats.Instance.Profile;

        UpdateProgressValue();

        levelText.text = profile.Level.ToString();
        stageScoreText.text = profile.LevelStage.ToString();
        progressScoreText.text = profile.LevelProgress.ToString();

        InitStatsUI();
    }

    public void InitStatsUI()
    {
        foreach (StatItemUI statItem in statItems)
        {
            statItem.SetValue(profile.StatItems.Find(x => x.type == statItem.profileStat));
        }
    }

    private void UpdateProgressValue()
    {
        progressSlider.maxValue = Profile.LevelStage;
        progressSlider.value = Profile.LevelProgress;
    }

    private void GetPictureYB()
    {

    }

    public void SwitchPanel()
    {
        profilePanel.SetActive(!profilePanel.activeInHierarchy);
    }

    public void ClosePanel()
    {
        profilePanel.SetActive(false);
    }

    void IStatsUi.UiUpdate()
    {
        UI_update();
    }
}
