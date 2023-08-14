using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Unit { None, Tank, Plate, Base, Fabric, Drill, Tower, Helicopter, Spawner }

[System.Serializable]
public class UnitStats
{
    [SerializeField] private string unitName;
    [SerializeField] private Unit unit;

    [NonSerialized] private int health;
    [NonSerialized] private int damage;
    [NonSerialized] private int speed;

    [SerializeField] public int armorLvl;
    [SerializeField] public int gunLvl;
    [SerializeField] public int speedLvl;

    [SerializeField] public int battlePrice = 30;

    public Unit Unit { get { return unit; } }
    public string UnitName { get { return unitName; } }
    public int Armor { get { return health; } }
    public int Damage { get { return damage; } }
    public int Speed { get { return speed; } }

    public UnitStats(Unit unit, int armorLvl, int powerLvl, int speedLvl)
    {
        this.unit = unit;
        this.unitName = unit.ToString();
        this.armorLvl = armorLvl;
        this.gunLvl = powerLvl;
        this.speedLvl = speedLvl;

        CountStats();
    }

    public void SaveStats()
    {

    }

    public void GetStats()
    {

    }

    public bool HasArmor()
    {
        return true;
    }

    public bool HasGun()
    {
        switch (unit)
        {
            case Unit.Tank:
                return true;
            case Unit.Tower:
                return true;
            case Unit.Helicopter:
                return true;
        }
        return false;
    }

    public bool HasSpeed()
    {
        switch (unit)
        {
            case Unit.Tank:
                return true;
            case Unit.Helicopter:
                return true;
            case Unit.Tower:
                return true;
            case Unit.Fabric:
                return true;
            case Unit.Drill:
                return true;
        }
        return false;
    }

    public void UpgradeGun()
    {
        if (WalletManager.UpgradeTrasaction(gunLvl, CoinType.Power) == false) return; 
        gunLvl++;
        CountStats();
        MenuManager.Instance.PlayerStats.SavePlayer();
        MenuManager.Instance.UiStatsUpdate();

        AchievementsManager.Instance.UpgradeUpdate(gunLvl);
    }

    public void UpgradeArmor()
    {
        if (WalletManager.UpgradeTrasaction(armorLvl, CoinType.Armor) == false) return;
        armorLvl++;
        CountStats();
        MenuManager.Instance.PlayerStats.SavePlayer();
        MenuManager.Instance.UiStatsUpdate();
    }

    public void UpgradeSpeed()
    {
        if (WalletManager.UpgradeTrasaction(speedLvl, CoinType.Power) == false) return;
        speedLvl++;
        CountStats();
        MenuManager.Instance.PlayerStats.SavePlayer();
        MenuManager.Instance.UiStatsUpdate();
    }

    public void CountStats()
    {
        health = GetHealthBase() + armorLvl * 3;
        damage = 1 + gunLvl;
        speed = 20 + speedLvl;

        LoadPrice();
    }

    public float GetSpeed()
    {
        float result = 1;

        result += speedLvl * 0.05f;

        return result;
    }

    public float GetEfficiency()
    {
        return speedLvl * 0.05f;
    }

    public int GetHealthBase()
    {
        if (this.Unit == Unit.None) return 0;

        return PlayerPrefs.GetInt(this.Unit + "Health");
    }

    public void LoadPrice()
    {
        if (this.Unit == Unit.None) return;

        this.battlePrice = PlayerPrefs.GetInt(this.Unit + "Price");
    }

    public int CountLvlValue(int lvl, StatType statType)
    {
        if (statType == StatType.Gun)
        {
            return 1 + lvl;
        }
        if (statType == StatType.Armor)
        {
            return lvl * 3 + GetHealthBase();
        }
        //if (statType == StatType.Speed)
        //{
        //    if (Unit == Unit.Drill)
        //    {
        //        return 20f + lvl * GetEfficiency();
        //    }
        //}

        return lvl + 20;
    }

    public static int GetUpGoldPrice(int lvl)
    {
        int result = 0;

        result = lvl * 300;

        return result;
    }

    public int GetPrice(UnitsCollector unitsCollector)
    {
        int result = battlePrice;

        result += (armorLvl + speedLvl + gunLvl) * 2;

        int multiplier = (int)Math.Pow(2, unitsCollector.GetCount(Unit));

        result = result * multiplier;

        return result;
    }

    public static int GetUpComponentsPrice(int lvl)
    {
        int result = 0;

        result = lvl * 2;

        return result;
    }

    public bool UpgradeAvaible(int gold, int power, int armor)
    {
        if (GetUpComponentsPrice(armorLvl) <= armor && GetUpGoldPrice(armorLvl) <= gold && HasArmor())
            return true;
        if (GetUpComponentsPrice(gunLvl) <= power && GetUpGoldPrice(gunLvl) <= gold && HasGun())
            return true;
        if (GetUpComponentsPrice(speedLvl) <= power && GetUpGoldPrice(speedLvl) <= gold && HasSpeed())
            return true;
        return false;
    }

    public UnitStats Copy()
    {
        UnitStats result = new UnitStats(this.Unit, this.armorLvl, this.gunLvl, this.speedLvl);

        result.CountStats();

        return result;
    }
}
