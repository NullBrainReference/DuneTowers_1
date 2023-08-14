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
}
