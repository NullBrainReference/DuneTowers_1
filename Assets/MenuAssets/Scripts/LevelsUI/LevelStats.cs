using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class LevelStats: PlayerBase
{
    public int id;
    public string name;

    public int topScore;

    [NonSerialized] public int production;
    [NonSerialized] public int extraction;
    [NonSerialized] public int unitsPower;

    public int minOutcomeUpgrades;
    public int minOutcomeCoins;

    public int maxOutcomeUpgrades;
    public int maxOutcomeCoins;

    //public List<UnitStats> unitsStats;

    public void DefaultsTest(int lvl)
    {
        UnitStats tank = new UnitStats(Unit.Tank, 1, 1, 1);
        UnitStats fabric = new UnitStats(Unit.Fabric, 1, 0, 1);
        UnitStats drill = new UnitStats(Unit.Drill, 1, 0, 1);
        UnitStats tower = new UnitStats(Unit.Tower, 1, 1, 1);

        for (int i = 0; i < lvl; i++)
        {
            tank.armorLvl += UnityEngine.Random.Range(0, 2);
            tank.gunLvl += UnityEngine.Random.Range(0, 2);
            tank.speedLvl += UnityEngine.Random.Range(0, 2);

            tower.armorLvl += UnityEngine.Random.Range(0, 2);
            tower.gunLvl += UnityEngine.Random.Range(0, 2);
            tower.speedLvl += UnityEngine.Random.Range(0, 2);

            fabric.armorLvl += UnityEngine.Random.Range(0, 2);
            fabric.speedLvl += UnityEngine.Random.Range(0, 2);

            drill.armorLvl += UnityEngine.Random.Range(0, 2);
            drill.speedLvl += UnityEngine.Random.Range(0, 2);
        }

        unitsStats = new List<UnitStats>() {
            tank,
            new UnitStats(Unit.Base, lvl, 0, lvl),
            fabric,
            drill,
            tower,
            new UnitStats(Unit.Plate, 1, 0, 0),
        };

        CountSimpleStats();
        minOutcomeCoins = 1000 + 200 * lvl;
        maxOutcomeCoins = 1500 + 500 * lvl;

        minOutcomeUpgrades = 4 + lvl;
        maxOutcomeUpgrades = 12 + 2 * lvl;

        topScore = 900;

        //TODO remake sqrtDistance * Credits
        //TODO bot should avoid building tankers in zero credit cells
        //TODO need to teach bot to build it's base in new meta properly
        //TODO need to balance new inbattle meta
        //TODO Mid + Late game economy tests (Seems like reward cards are too important and there is iposible to pass some levels becose of upgrade even this them)  
    }

    public void InitDefaultScore()
    {
        minOutcomeCoins = 900 + 120 * id;
        maxOutcomeCoins = 1120 + 380 * id;

        minOutcomeUpgrades = 10 + id;
        maxOutcomeUpgrades = 20 + 2 * id;

        topScore = 900;
    }

    public void InitUnitsStatsEmpty()
    {
        unitsStats = new List<UnitStats>
        {
            new UnitStats(Unit.Tank, 1, 1, 1),
            new UnitStats(Unit.Base, 1, 0, 0),
            new UnitStats(Unit.Fabric, 1, 0, 1),
            new UnitStats(Unit.Drill, 1, 0, 1),
            new UnitStats(Unit.Tower, 1, 1, 1),
            new UnitStats(Unit.Plate, 1, 0, 0),
        };
    }

    public void CountSimpleStats()
    {
        foreach (var item in unitsStats)
        {
            item.CountStats();
        }

        foreach (var item in unitsStats)
        {
            switch (item.Unit)
            {
                case Unit.Tank:
                    unitsPower += (int)((item.Damage + item.Armor + item.Speed) / 3);
                    break;
                case Unit.Tower:
                    unitsPower += (int)((item.Damage + item.Armor + item.Speed) / 3);
                    break;
                case Unit.Fabric:
                    production += (int)item.Speed;
                    break;
                case Unit.Drill:
                    extraction += (int)item.Speed;
                    break;
            }
        }
    }

    public static LevelStats CreateCustomStats(PlayerBase player, int lvlId)
    {
        LevelStats result = new LevelStats();

        result.minOutcomeCoins = 300;
        result.maxOutcomeCoins = 900;

        result.minOutcomeUpgrades = 2;
        result.maxOutcomeUpgrades = 8;

        int maxArmor = 0;
        int maxGun = 0;

        foreach (var item in player.unitsStats)
        {
            if (item.armorLvl > maxArmor)
                maxArmor = item.armorLvl;
            if (item.gunLvl > maxGun)
                maxGun = item.gunLvl;
        }

        result.id = lvlId;

        result.unitsStats = new List<UnitStats>
        {
            new UnitStats(Unit.Tank, maxArmor, maxGun, player.GetUnit(Unit.Tank).speedLvl),
            new UnitStats(Unit.Helicopter, maxArmor, maxGun, 1),
            new UnitStats(Unit.Base, maxArmor, 0, 0),
            new UnitStats(Unit.Fabric, maxArmor, 0, player.GetUnit(Unit.Fabric).speedLvl),
            new UnitStats(Unit.Drill, maxArmor, 0, player.GetUnit(Unit.Drill).speedLvl),
            new UnitStats(Unit.Tower, maxArmor, maxGun, player.GetUnit(Unit.Tower).speedLvl),
            new UnitStats(Unit.Plate, 1, 0, 0),
        };

        result.CountSimpleStats();

        return result;
    }

    public static LevelStats GetStats(int id)
    {
        string level = PlayerPrefs.GetString("Level" + id);
        LevelStats stats = JsonUtility.FromJson<LevelStats>(level);

        Debug.Log("Level_Loaded");
        return stats;
    }

    public void SaveStats()
    {
        string level = JsonUtility.ToJson(this);

        PlayerPrefs.SetString("Level" + id, level);
        Debug.Log("Level Saved");
    }

#if UNITY_EDITOR
    public void SaveToFile()
    {
        string levelString = JsonUtility.ToJson(this);

        string path = Application.dataPath + @"/Resources/Levels/Stats/levelStats_" + id + ".txt";
        //AssetDatabase.GetAssetPath()
        Debug.Log(path);

        using (StreamWriter sw = new StreamWriter(path, false))
        {
            sw.WriteLine(levelString);
        }
    }
#endif

    public static LevelStats LoadFromFile(int id)
    {
        LevelStats lvl = new LevelStats();

        var textFile = Resources.Load<TextAsset>(@"Levels\Stats\levelStats_" + id.ToString());

        lvl = JsonUtility.FromJson<LevelStats>(textFile.text);

        return lvl;
    }

    public UnitStats GetUnitStats(Unit unit) => unitsStats.Find(x => x.Unit == unit);
    
}
