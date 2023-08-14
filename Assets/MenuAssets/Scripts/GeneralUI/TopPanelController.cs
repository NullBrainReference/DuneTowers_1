using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TopPanelController : MonoBehaviour, IStatsUi
{
    [SerializeField] private TextMeshProUGUI goldCoinsText;
    [SerializeField] private TextMeshProUGUI powerCoinsText;
    [SerializeField] private TextMeshProUGUI armorCoinsText;

    [SerializeField] private Animator goldAnimator;
    [SerializeField] private Animator powerAnimator;
    [SerializeField] private Animator armorAnimator;

    public string Tag { get; set; } = "TopPanel";

    public void UpdateValues()
    {
        //int gold = Convert.ToInt32(goldCoinsText.text);
        //int power = Convert.ToInt32(powerCoinsText.text);
        //int armor = Convert.ToInt32(armorCoinsText.text);

        goldCoinsText.text = MenuManager.Instance.PlayerStats.goldCoins.ToString();
        powerCoinsText.text = MenuManager.Instance.PlayerStats.powerCoins.ToString();
        armorCoinsText.text = MenuManager.Instance.PlayerStats.armorCoins.ToString();
    }

    public void UpdateValuesStandalone()
    {
        goldCoinsText.text = MenuManager.Instance.PlayerStats.goldCoins.ToString();
        powerCoinsText.text = MenuManager.Instance.PlayerStats.powerCoins.ToString();
        armorCoinsText.text = MenuManager.Instance.PlayerStats.armorCoins.ToString();
    }

    public void Animate()
    {
        int gold = Convert.ToInt32(goldCoinsText.text);
        int power = Convert.ToInt32(powerCoinsText.text);
        int armor = Convert.ToInt32(armorCoinsText.text);

        if (gold < MenuManager.Instance.PlayerStats.goldCoins)
        {
            //goldCoinsText.text = MenuManager.Instance.PlayerStats.goldCoins.ToString();
            goldAnimator.Rebind();
            goldAnimator.Play("CoinIncome");
        }

        if (power < MenuManager.Instance.PlayerStats.powerCoins)
        {
            //powerCoinsText.text = MenuManager.Instance.PlayerStats.powerCoins.ToString();
            powerAnimator.Rebind();
            powerAnimator.Play("CoinIncome");
        }

        if (armor < MenuManager.Instance.PlayerStats.armorCoins)
        {
            //armorCoinsText.text = MenuManager.Instance.PlayerStats.armorCoins.ToString();
            armorAnimator.Rebind();
            armorAnimator.Play("CoinIncome");
        }
    }

    void IStatsUi.UiUpdate()
    {

        //Animate();
        UpdateValues();
    }
}
