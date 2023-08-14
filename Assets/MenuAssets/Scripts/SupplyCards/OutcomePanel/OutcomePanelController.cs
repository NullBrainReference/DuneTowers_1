using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutcomePanelController : MonoBehaviour
{
    public GameObject outcomePanel;
    public Button okButton;

    public OutcomeBackgroundController backgroundController;
    public TopPanelController topPanelController;

    public GameObject armorGroup;
    public GameObject powerGroup;
    public GameObject goldGroup;

    [SerializeField] private GameObject defeatText;
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject defenceText;

    [SerializeField] private List<TextItemUI> textItems;

    public Text armorText;
    public Text powerText;
    public Text goldText;

    public Animator animator;

    private int gold;
    private int power;
    private int armor;

    public void PlayShow()
    {
        if (okButton == null) return;

        okButton.interactable = true;

        backgroundController.Open();
        outcomePanel.SetActive(true);

        animator.Rebind();

        animator.Play("RewardShow");

        if (textItems == null) return;

        foreach (TextItemUI item in textItems)
        {
            item.PlayOpen();
        }
    }

    public void PlayClose()
    {
        okButton.interactable = false;
        backgroundController.PlayClose();
        animator.Play("RewardCloseAlt");

        if (textItems == null) return;

        foreach (TextItemUI item in textItems)
        {
            item.PlayClose();
        }
    }

    public void OnClose()
    {
        outcomePanel.SetActive(false);
        //backgroundController.PlayClose();
    }

    public void ShowResult(List<CoinType> coins, List<int> values)
    {
        gold = 0;
        armor = 0;
        power = 0;

        armorGroup.SetActive(false);
        powerGroup.SetActive(false);
        goldGroup.SetActive(false);

        for (int i = 0; i < coins.Count; i++)
        {
            switch (coins[i])
            {
                case CoinType.Armor:
                    armorGroup.SetActive(true);
                    armor = values[i];
                    break;
                case CoinType.Power:
                    powerGroup.SetActive(true);
                    power = values[i];
                    break;
                case CoinType.Gold:
                    goldGroup.SetActive(true);
                    gold = values[i];
                    break;
            }
        }

        SetValues(armor, power, gold);
    }

    public void SetValues(int armor, int power, int gold)
    {
        this.armor = armor;
        this.power = power;
        this.gold = gold;

        if (armorText != null)
        {
            armorText.text = armor.ToString();
        }
        if (powerText != null)
        {
            powerText.text = power.ToString();
        }
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
    }

    public void PressOk()
    {
        SimpleSoundsManager.Instance.PlayClick();

        if (topPanelController != null)
            MenuManager.Instance.AddEveryCoin(gold, power, armor);
        else
            SaveBattleOutcome();

        AchievementsManager.Instance.ArmorUpdate(armor);
        AchievementsManager.Instance.PowerUpdate(power);
        AchievementsManager.Instance.GoldUpdate(gold);

        if (topPanelController != null)
        {
            topPanelController.Animate();
        }
        else 
        {
            GameStats.Instance.Profile.Save();
            //GameStats.Instance.profile = null;

            if (GameStats.Instance.gameResult == GameResult.Win)
            {
                AchievementsManager.Instance.LevelTimeUpdate(GameStats.Instance.battleTimer.TotalTime);
                AchievementsManager.Instance.LevelUpdate(GameStats.Instance.level.id);
            }

            Time.timeScale = 1;

            SimpleSoundsManager.Instance.SwitchToMenuMusic();

            SceneManager.LoadScene("HorisontalMenu");

            ShowBannerController.Instance.CallBanner();

            return;
        }

        if (MenuManager.Instance != null)
            MenuManager.Instance.UiUpdateByTag("StatUnit");
    }

    public void SaveBattleOutcome()
    {
        PlayerStats player = PlayerStats.GetPlayer();

        player.goldCoins += gold;
        player.armorCoins += armor;
        player.powerCoins += power;

        player.SavePlayer();
    }

    public void PlayShowEndEvent()
    {
        if (topPanelController == null)
        {
            Time.timeScale = 0;
            Debug.Log("Game Paused");
        }
    }

    public void InitDefeatValue(int playerNo)
    {
        if (defeatText == null) 
            return;

        if (playerNo == 0)
        {
            if (GameStats.Instance.isTowerDefence)
            {
                defeatText.SetActive(false);
                victoryText.SetActive(false);
                defenceText.SetActive(true);

                armorGroup.SetActive(true);
                powerGroup.SetActive(true);
                goldGroup.SetActive(true);
            }
            else
            {
                defeatText.SetActive(true);
                victoryText.SetActive(false);
                defenceText.SetActive(false);

                armorGroup.SetActive(false);
                powerGroup.SetActive(false);
                goldGroup.SetActive(false);

                gold = 0;
                armor = 0;
                power = 0;
            }

            //gold = 0;
            //armor = 0;
            //power = 0;

            GameStats.Instance.gameResult = GameResult.Lose;
            if (GameStats.Instance.isTowerDefence == false)
                GameStats.Instance.Profile.StatIncrement(ProfileStat.Lose);

            SimpleSoundsManager.Instance.PlayLoseSound();
        }
        else
        {
            defeatText.SetActive(false);
            victoryText.SetActive(true);

            armorGroup.SetActive(true);
            powerGroup.SetActive(true);
            goldGroup.SetActive(true);

            GameStats.Instance.gameResult = GameResult.Win;

            GameStats.Instance.Profile.StatIncrement(ProfileStat.Win);

            SimpleSoundsManager.Instance.PlayWinSound();
        }
    }
}
