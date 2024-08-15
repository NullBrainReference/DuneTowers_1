using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditButton : MonoBehaviour
{
    [SerializeField] private int levelId;

    public void ToEditor()
    {
        GameStats.Instance.userMapId = levelId;
        GameStats.Instance.gameMode = GameMode.Editor;

        MapStats mapStats = MapStats.LoadUserMap(levelId);

        if (mapStats == null)
        {
            mapStats = new MapStats();
            mapStats.CreateEmpty16x10(levelId);
            mapStats.SaveUserMap();
        }

        SceneManager.LoadScene("UserMapCreatorScene");
    }
}
