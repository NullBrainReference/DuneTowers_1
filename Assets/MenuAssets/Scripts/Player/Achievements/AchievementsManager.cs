using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    [SerializeField] private AchievementScroll achievementScroll;

    public AchievementBase editAchievement;

    public List<AchievementBase> achievements;

    public static AchievementsManager Instance { get; private set; }

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

    public void SetScroll(AchievementScroll scroll)
    {
        achievementScroll = scroll;
    }

    public void InitAchievements()
    {
        achievements = new List<AchievementBase>();

        if (PlayerPrefs.GetInt("achievementsInited") != 1)
        {
            achievements.Add(LoadAchievement(0));
            achievements.Add(LoadAchievement(1));
            achievements.Add(LoadAchievement(2));
            achievements.Add(LoadAchievement(3));
            achievements.Add(LoadAchievement(4));
            achievements.Add(LoadAchievement(5));
            achievements.Add(LoadAchievement(6));
            achievements.Add(LoadAchievement(7));

            foreach (AchievementBase achievement in achievements)
                achievement.Save();
        }
        else
        {
            achievements.Add(AchievementBase.GetAchievement(0));
            achievements.Add(AchievementBase.GetAchievement(1));
            achievements.Add(AchievementBase.GetAchievement(2));
            achievements.Add(AchievementBase.GetAchievement(3));
            achievements.Add(AchievementBase.GetAchievement(4));
            achievements.Add(AchievementBase.GetAchievement(5));
            achievements.Add(AchievementBase.GetAchievement(6));
            achievements.Add(AchievementBase.GetAchievement(7));
        }

        PlayerPrefs.SetInt("achievementsInited", 1);

        if (achievementScroll == null) 
            return;
    }

    public void AchievementsUpdate(AchievementType type, float value = 0)
    {
        switch (type)
        {
            //Economy actions
            case AchievementType.Gold:
                break;
            case AchievementType.Power:
                break;
            case AchievementType.Armor:
                break;

            //Player and units actions
            case AchievementType.UnitLevel:
                break;
            case AchievementType.PlayerLevel:
                break;
            
            //Mission actions
            case AchievementType.LevelComplete:
                break;
            case AchievementType.DefenceTime:
                break;
            case AchievementType.Destroy:
                break;
        }
    }

    public void GoldUpdate(float value)
    {
        AchievementBase achievement = achievements.Find(x => x.AchievementType == AchievementType.Gold);

        if (achievement != null)
            achievement.AddProgress(value);
    }

    public void ArmorUpdate(float value)
    {
        AchievementBase achievement = achievements.Find(x => x.AchievementType == AchievementType.Armor);

        if (achievement != null)
            achievement.AddProgress(value);
    }

    public void PowerUpdate(float value)
    {
        AchievementBase achievement = achievements.Find(x => x.AchievementType == AchievementType.Power);

        if (achievement != null)
            achievement.AddProgress(value);
    }

    public void DestroyUpdate(float value)
    {
        AchievementBase achievement = achievements.Find(x => x.AchievementType == AchievementType.Destroy);

        if (achievement != null)
            achievement.AddProgress(value);
    }

    public void UpgradeUpdate(float value)
    {
        AchievementBase achievement = achievements.Find(x => x.AchievementType == AchievementType.UnitLevel);

        if (achievement != null)
            achievement.CompareAndSetProgress(value);
    }

    public void LevelTimeUpdate(float value)
    {
        AchievementBase achievement = achievements.Find(x => x.AchievementType == AchievementType.LevelTime);

        if (achievement != null)
        {
            if (achievement.Goal >= value)
                achievement.AddProgress(achievement.Goal);
        }
    }

    public void DefenceTimeUpdate(float value)
    {
        AchievementBase achievement = achievements.Find(x => x.AchievementType == AchievementType.DefenceTime);

        if (achievement != null)
        {
            if (achievement.Goal <= value)
                achievement.AddProgress(achievement.Goal);
        }
    }

    public void LevelUpdate(float value)
    {
        AchievementBase achievement = achievements.Find(x => x.AchievementType == AchievementType.LevelComplete);

        if (achievement != null)
        {
            achievement.SetEqualProgress(value);
        }
    }

    public static AchievementBase LoadAchievement(int key)
    {
        AchievementBase achievement = new AchievementBase();

        var textFile = Resources.Load<TextAsset>(@"Achievements\achievement_" + key.ToString());

        if (textFile == null)
        {
            Debug.Log("_Achievement_" + key.ToString() + "_Doesn't exist");
            return new AchievementBase();
        }

        achievement = JsonUtility.FromJson<AchievementBase>(textFile.text);

        return achievement;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Use only for creating/editing Achievements no progress should be saved
    /// </summary>
    public void SaveAchievement()
    {
        string acieveString = JsonUtility.ToJson(editAchievement);

        //string path = @"C:\UnityProjects\DuneTowers (1)\Assets\Resources\Levels\" + "level_" + lvl + ".txt";
        string path = Application.dataPath + @"/Resources/Achievements/achievement_" + editAchievement.Key + ".txt";
        //AssetDatabase.GetAssetPath()
        Debug.Log(path);

        using (StreamWriter sw = new StreamWriter(path, false))
        {
            sw.WriteLine(acieveString);
        }
    }
#endif
}

