using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CurrentScene { Menu, Battle }

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    [SerializeField] private GameObject StarPrefab;
    [SerializeField] private GameObject LockedStarPrefab;

    [SerializeField] private GameObject starsContainer;

    [SerializeField] private LevelCell levelCell;

    [SerializeField] private CurrentScene currentScene;

    [SerializeField] private int score;

    private void Start()
    {
        if (currentScene == CurrentScene.Battle) 
            CountScore();
    }

    public void CountScore()
    {
        if (currentScene == CurrentScene.Battle) 
        {
            if (GameStats.Instance.isTowerDefence)
                score = 200 + (int)GameStats.Instance.battleTimer.TotalTime;
            else
                score = 300 + (int)(600f * GameStats.Instance.battleTimer.GetStage());
            //score = (int)GameStats.Instance.battleTimer.TotalTime;
        }

        if (GameStats.Instance.gameResult == GameResult.Lose)
        {
            if (GameStats.Instance.isTowerDefence == false)
                score = 0;
        }

        InitStars();
        if (currentScene == CurrentScene.Battle && GameStats.Instance.gameResult == GameResult.Win)
        {
            LevelResult levelResult = LevelResult.Load(GameStats.Instance.level.id);
            if (levelResult != null)
            {
                if (levelResult.score < score)
                    SaveScore();

                PlayerProfile.WriteProfileProgress(score);
            }
        }

        //scoreText.text = score.ToString();
        //TODO replace Timer test
        scoreText.text = score.ToString();
    }

    public void InitStars()
    {
        int stars = score;

        int achiveStep = 0;

        if (currentScene == CurrentScene.Battle) 
        { 
            achiveStep = (int)(GameStats.Instance.level.topScore / 3); 
        }
        else if (currentScene == CurrentScene.Menu)
        {
            LevelResult levelResult = LevelResult.Load(levelCell.levelStats.id);

            score = levelResult.score;
            stars = levelResult.score;
            achiveStep = (int)(levelCell.levelStats.topScore / 3);
        }

        for (int i = 0; i < 3; i++)
        {
            if(stars >= achiveStep)
            {
                Instantiate(StarPrefab, starsContainer.transform);
            }
            else
            {
                Instantiate(LockedStarPrefab, starsContainer.transform);
            }
            stars -= achiveStep;
        }
    }

    public void SaveScore()
    {
        LevelResult levelResult = new LevelResult();

        levelResult.score = score;
        levelResult.passed = true;
        levelResult.level = GameStats.Instance.level.id;

        levelResult.Save();
    }
}
