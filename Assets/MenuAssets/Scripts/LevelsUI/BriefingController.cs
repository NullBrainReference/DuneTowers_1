using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BriefingController : MonoBehaviour
{
    [SerializeField] private Text palyerProductionText;
    [SerializeField] private Text playerExtractionText;
    [SerializeField] private Text playerUnitsText;

    [SerializeField] private Text enemyProductionText;
    [SerializeField] private Text enemyExtractionText;
    [SerializeField] private Text enemyUnitsText;

    [SerializeField] private Text outcomeCoinsText;
    [SerializeField] private Text outcomeUpgradesText;

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject backgroundPanel;

    [SerializeField] private List<GameObject> outcomeGroup;

    [SerializeField] private LevelStats levelStats;

    public void SetPlayerText()
    {
        MenuManager.Instance.PlayerStats.CountSimpleStats();
        PlayerStats stats = MenuManager.Instance.PlayerStats;

        palyerProductionText.text = stats.production.ToString();
        playerExtractionText.text = stats.extraction.ToString();
        playerUnitsText.text = stats.unitsPower.ToString();
    }

    public void SetOutcomeValues(LevelStats stats)
    {
        outcomeCoinsText.text = stats.minOutcomeCoins.ToString() + " - " + stats.maxOutcomeCoins.ToString();
        outcomeUpgradesText.text = stats.minOutcomeUpgrades.ToString() + " - " + stats.maxOutcomeUpgrades.ToString();

        GameStats.Instance.minComponentsOutcome = stats.minOutcomeUpgrades;
        GameStats.Instance.maxComponentsOutcome = stats.maxOutcomeUpgrades;

        GameStats.Instance.minGoldOutcome = stats.minOutcomeCoins;
        GameStats.Instance.maxGoldOutcome = stats.maxOutcomeCoins;
    }

    public void SetEnemyText(LevelStats stats)
    {
        enemyProductionText.text = stats.production.ToString();
        enemyExtractionText.text = stats.extraction.ToString();
        enemyUnitsText.text = stats.unitsPower.ToString();
    }

    private void ShowOutcome(bool show)
    {
        foreach (var item in outcomeGroup)
            item.SetActive(show);
    }

    public void Show(LevelStats levelStats)
    {
        if (levelStats.id == 0)
            ShowOutcome(false);
        else
            ShowOutcome(true);

        SetEnemyText(levelStats);
        SetPlayerText();
        SetOutcomeValues(levelStats);

        mainPanel.SetActive(true);
        backgroundPanel.SetActive(true);

        this.levelStats = levelStats;
    }

    public void Hide()
    {
        SimpleSoundsManager.Instance.PlayClick();

        mainPanel.SetActive(false);
        backgroundPanel.SetActive(false);
    }

    public void ToBattle()
    {
        GameStats.Instance.SetStats(levelStats, MenuManager.Instance.PlayerStats);

        if (levelStats.id == 0)
            GameStats.Instance.isTowerDefence = true;
        else
            GameStats.Instance.isTowerDefence = false;

        SimpleSoundsManager.Instance.SwitchToBattleMusic();

        SceneManager.LoadScene("mainScene", LoadSceneMode.Single);
    }
}
