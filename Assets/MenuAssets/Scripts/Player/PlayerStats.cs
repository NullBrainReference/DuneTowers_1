using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public enum CoinType { Gold, Power, Armor}

[System.Serializable]
public class PlayerStats: PlayerBase
{
    [NonSerialized] public int production;
    [NonSerialized] public int extraction;
    [NonSerialized] public int unitsPower;

    public int powerCoins;
    public int armorCoins;
    public int goldCoins;

    //[SerializeField] public List<UnitStats> unitsStats;
    [SerializeField] public List<LevelResult> levelResults;

    public int LastLevel { get { return PlayerPrefs.GetInt("LastLevel"); } }

    public void DropSimpleStats()
    {
        production = 0;
        extraction = 0;
        unitsPower = 0;
    }

    public void CountSimpleStats()
    {
        DropSimpleStats();

        foreach (var item in unitsStats)
        {
            switch (item.Unit)
            {
                case Unit.Tank:
                    unitsPower += (int)((item.Damage + item.Armor + item.Speed) / 3);
                    break;
                case Unit.Helicopter:
                    unitsPower += (int)((item.Damage + item.Armor + item.Speed) / 3);
                    break;
                case Unit.Tower:
                    unitsPower += (int)((item.Damage + item.Armor + item.Speed) / 3);
                    break;
                case Unit.Fabric:
                    production += (int)item.Speed;
                    break;
                case Unit.Drill:
                    extraction += (int)item.Speed;
                    break;
            }
        }
    }

    public void Pay(int price, CoinType coin)
    {
        switch (coin)
        {
            case CoinType.Gold:
                goldCoins -= price;
                break;
            case CoinType.Power:
                powerCoins -= price;
                break;
            case CoinType.Armor:
                armorCoins -= price;
                break;
        }
    }

    public void CountUnitStats()
    {
        foreach(var item in unitsStats)
        {
            item.CountStats();
        }
        MenuManager.Instance.UiStatsUpdate();
    }

    public void SavePlayer()
    {
        string playerString = JsonUtility.ToJson(this);

        PlayerPrefs.SetString("Player", playerString);
        //Debug.Log("Payer Saved");
    }

    public static PlayerStats GetPlayer()
    {
        string playerString = PlayerPrefs.GetString("Player");
        PlayerStats player = JsonUtility.FromJson<PlayerStats>(playerString);

        //Debug.Log("Player_Loaded");
        return player;
    }
}
