using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] private LevelCell[] levelCells;
    [SerializeField] private GameObject scrollGO;

    [SerializeField] private ScrollRect mapScroll;
    [SerializeField] private RectTransform contentPanel;

    [SerializeField] private float focusOffsetX;
    [SerializeField] private float focusOffsetY;

    private int lastLvl = 0;

    private void Start()
    {
        if (AreStatsInited() == false) 
            InitLevels();
        InitCells();
    }

    private void InitCells()
    {
        levelCells[0].SetLevel(1, 0, false);

        for (int i = 1; i < levelCells.Length; i++ )
        {
            levelCells[i].SetLevel(i + 1, 101 * i + 301 - i);
        }
        //InitLevels();
        LoadLevels();
        CentralizeLastLevel();
    }

    public void CentralizeLastLevel()
    {
        Canvas.ForceUpdateCanvases();

        lastLvl = MenuManager.Instance.PlayerStats.LastLevel;

        RectTransform target = levelCells[lastLvl].GetComponent<RectTransform>();

        contentPanel.anchoredPosition =
                //(Vector2)mapScroll.transform.InverseTransformPoint(contentPanel.position)
                -(Vector2)mapScroll.transform.InverseTransformPoint(target.position) - new Vector2(focusOffsetX, focusOffsetY);
    }

    public void InitLevels()
    {
        for (int i = 0; i < levelCells.Length; i++)
        {
            //LevelStats levelStats = new LevelStats();
            //levelStats.DefaultsTest(i + 1);
            //levelStats.id = i + 1;
            ////levelCells[i].SetStats(levelStats);
            //levelStats.SaveStats();

            InitResult(i);

            PlayerPrefs.SetInt("LastLevel", 0);
        }
    }

    private bool AreStatsInited()
    {
        if (LevelResult.Load(1) == null)
            return false;
        else
            return true;
    } 

    public void InitResult(int i)
    {
        LevelResult levelResult = new LevelResult();
        levelResult.score = 0;
        levelResult.level = i + 1;
        levelResult.passed = false;
        levelResult.Save();
    }

    public void LoadLevels()
    {
        for (int i = 0; i < levelCells.Length; i++)
        {
            LevelStats levelStats = LevelStats.LoadFromFile(i + 1);
            levelStats.CountSimpleStats();

            levelCells[i].SetStats(levelStats);
        }
    }
}
