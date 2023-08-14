using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupplyCard : MonoBehaviour
{
    public GameObject playGroup;
    public GameObject timeGroup;

    public Text armorText;
    public Text powerText;
    public Text goldText;

    public OutcomePanelController outcomePanel;

    public SupplyTimer timer;

    //public CoinType coinType;

    public int minArmor;
    public int maxArmor;

    public int minPower;
    public int maxPower;

    public int minGold;
    public int maxGold;

    private void Start()
    {
        timer = new SupplyTimer();

        InitValues();
    }

    private void FixedUpdate()
    {
        timer.TimerUpdate();
    }

    public void TurnTimerOn()
    {
        timeGroup.SetActive(true);
        playGroup.SetActive(false);
    }

    public void TurnTimerOff()
    {
        timeGroup.SetActive(false);
        playGroup.SetActive(true);
    }

    public void ShowAdd()
    {
        Yandex.ShowReward();
        
        PauseManager.Pause();
    }

    public void OnClick()
    {
        //TODO replace with yandex
        SimpleSoundsManager.Instance.PlayOutcome();
        
        outcomePanel.PlayShow();
        PassResults();
        //========================

        if (timer.IsEnded == false) return; 

        //TODO replace with yandex
        timer.SetUp();
        TurnTimerOn();
        //========================

        ShowAdd();
    }

    public void GiveReward()
    {
        SimpleSoundsManager.Instance.PlayOutcome();

        outcomePanel.PlayShow();
        PassResults();

        //if (timer.IsEnded == false) return;

        timer.SetUp();
        TurnTimerOn();
    }

    public void InitValues()
    {
        if (armorText != null)
            armorText.text = minArmor.ToString() + "-" + maxArmor.ToString(); 
        
        if (powerText != null)
            powerText.text = minPower.ToString() + "-" + maxPower.ToString();
        
        if (goldText != null)
            goldText.text = ((int)(minGold/1000)).ToString() + "K-" + ((int)(maxGold / 1000)).ToString() + "K";
    }

    public List<int> GetResultValues()
    {
        List<int> resultValues = new List<int>();

        if (armorText != null)
            resultValues.Add(Random.Range(minArmor, maxArmor + 1));

        if (powerText != null)
            resultValues.Add(Random.Range(minPower, maxPower + 1));

        if (goldText != null)
            resultValues.Add(Random.Range(minGold, maxGold + 1));

        return resultValues;
    }

    public List<CoinType> GetResultCoins()
    {
        List<CoinType> resultCoins = new List<CoinType>();

        if (armorText != null)
            resultCoins.Add(CoinType.Armor);
        
        if (powerText != null)
            resultCoins.Add(CoinType.Power);
        
        if (goldText != null)
            resultCoins.Add(CoinType.Gold);

        return resultCoins;
    }

    public void PassResults()
    {
        outcomePanel.ShowResult(GetResultCoins(), GetResultValues());
    }

}
