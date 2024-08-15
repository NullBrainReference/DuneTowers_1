using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MapStats
{
    public int lvl;

    public int xSize;
    public int ySize;

    public List<CellStats> stats;

    //public MapStats()
    //{
    //    stats = new List<CellStats>();
    //}

    public void SetStats(List<MapCell> cells)
    {
        stats = new List<CellStats>();

        foreach (var item in cells)
            stats.Add(item.cellStats);
    }

    public void CreateEmpty16x10(int lvl)
    {
        stats = new List<CellStats>();

        this.lvl = lvl;

        xSize = 16;
        ySize = 10;

        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                CellStats cell = new CellStats();
       
                cell.x = i;
                cell.y = j;

                stats.Add(cell);
            }
        }

        stats[0].hq = HQ_place_status.HQ_0;
        stats[1].credits = 10000;
        stats[stats.Count - 1].hq = HQ_place_status.HQ_1;
        stats[stats.Count - 2].credits = 10000;
    }

    public void SaveUserMap()
    {
        lvl = GameStats.Instance.userMapId;

        string levelString = JsonUtility.ToJson(this);

        PlayerPrefs.SetString("UserMap_" + lvl, levelString);
    }

    public void SaveMap()
    {
        string levelString = JsonUtility.ToJson(this);

        //string path = @"C:\UnityProjects\DuneTowers (1)\Assets\Resources\Levels\" + "level_" + lvl + ".txt";
        string path = Application.dataPath + @"/Resources/Levels/level_" + lvl + ".txt";
        //AssetDatabase.GetAssetPath()
        Debug.Log(path);

        using (StreamWriter sw = new StreamWriter(path, false))
        {
            sw.WriteLine(levelString);
        }
    }

    public static MapStats Load(int lvl)
    {
        MapStats map = new MapStats();

        var textFile = Resources.Load<TextAsset>(@"Levels\level_" + lvl.ToString());

        map = JsonUtility.FromJson<MapStats>(textFile.text);

        return map;
    }

    public static MapStats LoadUserMap(int id)
    {
        string map = PlayerPrefs.GetString("UserMap_" + id);

        return JsonUtility.FromJson<MapStats>(map);

    }
}
