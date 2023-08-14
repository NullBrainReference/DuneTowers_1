using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public enum StatType { Armor, Gun, Speed }

public class StatUnit : MonoBehaviour, IStatsUi
{
    [SerializeField] private Button upButton;
    [SerializeField] private Image upImage;
    [SerializeField] private Image goldImage;
    [SerializeField] private Image comonentImage;

    [SerializeField] private Text nameText;
    [SerializeField] private Text valueText;
    [SerializeField] private Text lvlText;
    [SerializeField] private Text lvlNumText;
    [SerializeField] private Text coinsPrice;
    [SerializeField] private Text componentPrice;

    [SerializeField] private string valueName;
    [SerializeField] private string lvlName;

    [SerializeField] private UnitStats unit;
    [SerializeField] private StatType type;
    [SerializeField] private CoinType coinType;

    [SerializeField] private Animator animator;

    public string Tag { get; set; } = "StatUnit";

    private void Start()
    {
        //MenuManager.Instance.AddUIStat(this);
        //lvlName = lvlText.text;
    }

    public void SetUpUnit(UnitStats unit, StatType type)
    {
        this.type = type;
        this.unit = unit;
    }

    public void PriceUpdate()
    {
        switch (type)
        {
            case StatType.Armor:
                coinsPrice.text = UnitStats.GetUpGoldPrice(unit.armorLvl).ToString();
                componentPrice.text = UnitStats.GetUpComponentsPrice(unit.armorLvl).ToString();
                break;
            case StatType.Gun:
                coinsPrice.text = UnitStats.GetUpGoldPrice(unit.gunLvl).ToString();
                componentPrice.text = UnitStats.GetUpComponentsPrice(unit.gunLvl).ToString();
                break;
            case StatType.Speed:
                coinsPrice.text = UnitStats.GetUpGoldPrice(unit.speedLvl).ToString();
                componentPrice.text = UnitStats.GetUpComponentsPrice(unit.speedLvl).ToString();
                break;
        }
    }

    public void TextUpdate(int value, int lvl)
    {
        //nameText.text = valueName;
        valueText.text = value.ToString() + " => " + unit.CountLvlValue(lvl+1, type);
        
        lvlNumText.text = lvl.ToString();
        PriceUpdate();
    }

    public void BalanceCheck(int componentCoins, int coinsAmount, int lvl)
    {
        bool gold = UnitStats.GetUpGoldPrice(lvl) <= MenuManager.Instance.PlayerStats.goldCoins;

        if (componentCoins <= coinsAmount && gold)
        {
            upButton.interactable = true;
            upImage.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);

            goldImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            comonentImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
            upButton.interactable = false;
            upImage.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            goldImage.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            comonentImage.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }
    }

    public void PlayUpgrage()
    {
        animator.Play("UpgradeAnim");
    }

    public void OnClick()
    {
        SimpleSoundsManager.Instance.PlayClick();

        PlayUpgrage();
    }

    public void Upgrade()
    {
        //SimpleSoundsManager.Instance.PlayClick();
        PlayerProfile.WriteProfileProgress(GameStats.Instance.upgradeScore);

        switch (type)
        {
            case StatType.Armor:
                unit.UpgradeArmor();
                break;
            case StatType.Gun:
                unit.UpgradeGun();
                break;
            case StatType.Speed:
                unit.UpgradeSpeed();
                break;
        }

        //PlayUpgrage();
    }

    void IStatsUi.UiUpdate()
    {
        switch (type)
        {
            case StatType.Armor:
                TextUpdate(unit.Armor, unit.armorLvl);
                BalanceCheck(
                    UnitStats.GetUpComponentsPrice(unit.armorLvl),
                    MenuManager.Instance.PlayerStats.armorCoins,
                    unit.armorLvl);
                break;
            case StatType.Gun:
                TextUpdate(unit.Damage, unit.gunLvl);
                BalanceCheck(
                    UnitStats.GetUpComponentsPrice(unit.gunLvl),
                    MenuManager.Instance.PlayerStats.powerCoins,
                    unit.gunLvl);
                break;
            case StatType.Speed:
                TextUpdate(unit.Speed, unit.speedLvl);
                BalanceCheck(
                    UnitStats.GetUpComponentsPrice(unit.speedLvl),
                    MenuManager.Instance.PlayerStats.powerCoins,
                    unit.speedLvl);
                break;
        }
    }
}
