using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelResult
{
    public int level;
    public int score;
    public bool passed;

    public void Save()
    {
        string level = JsonUtility.ToJson(this);

        PlayerPrefs.SetString("LevelResult" + this.level, level);
#if UNITY_EDITOR
        //Debug.Log("Result Saved");
#endif
    }

    public static LevelResult Load(int lvl)
    {
        string level = PlayerPrefs.GetString("LevelResult" + lvl);
        LevelResult result = JsonUtility.FromJson<LevelResult>(level);
#if UNITY_EDITOR
        //Debug.Log("Result_Loaded");
#endif
        return result;
    }
}
