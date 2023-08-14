using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[System.Serializable]
public class PlayerProfile
{
    [SerializeField] private int level;
    [SerializeField] private float levelProgress;
    [SerializeField] private float levelStage;

    [SerializeField] private List<StatItem> statItems;

    public int Level { get { return level; } }
    public float LevelProgress { get { return levelProgress; } }
    public float LevelStage { get { return levelStage; } }

    public List<StatItem> StatItems { get { return statItems; } }

    public void AddProgress(float progress)
    {
        levelProgress += progress;

        if (levelProgress >= levelStage)
        {
            level++;
            levelProgress -= levelStage;
        }
    }

    public void StatIncrement(ProfileStat type)
    {
        StatItem statItem = StatItems.Find(x => x.type == type);

        if (statItem == null) 
            return;

        statItem.ValueIncrement();
    }

    public void StatIncrementMax(ProfileStat type, int value)
    {
        StatItem statItem = StatItems.Find(x => x.type == type);

        if (statItem == null)
            return;

        if (value > statItem.value)
        {
            statItem.value = value;
        }
    }

    public void InitCleanProfile(int levelStage)
    {
        level = 1;
        levelProgress = 0;
        this.levelStage = levelStage;

        statItems = new List<StatItem>();

        statItems.Add(new StatItem { id = 0, type = ProfileStat.Kill, value = 0 });
        statItems.Add(new StatItem { id = 1, type = ProfileStat.Death, value = 0 });
        statItems.Add(new StatItem { id = 2, type = ProfileStat.Win, value = 0 });
        statItems.Add(new StatItem { id = 3, type = ProfileStat.Lose, value = 0 });
        statItems.Add(new StatItem { id = 4, type = ProfileStat.Wave, value = 0 });
    }

    public void Save()
    {
        string profile = JsonUtility.ToJson(this);

        PlayerPrefs.SetString("Profile", profile);
    }

    public static PlayerProfile Load()
    {
        string profile = PlayerPrefs.GetString("Profile");
        PlayerProfile playerProfile = JsonUtility.FromJson<PlayerProfile>(profile);

        return playerProfile;
    }

    public static PlayerProfile LoadOrInitProfile()
    {
        PlayerProfile result = Load();

        if (result == null)
        {
            result = new PlayerProfile();
            result.InitCleanProfile(1200);
            result.Save();
        }

        return result;
    }

    public static void WriteProfileProgress(float score)
    {
        PlayerProfile playerProfile = GameStats.Instance.Profile;

        playerProfile.AddProgress(score);
        playerProfile.Save();
    }
}
