using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public static bool UpgradeTrasaction(int lvl, CoinType coinType)
    {
        bool result = false;

        if (coinType == CoinType.Armor)
        {
            if (UnitStats.GetUpComponentsPrice(lvl) > MenuManager.Instance.PlayerStats.armorCoins)
            {
                return false;
            }
        }
        else if (coinType == CoinType.Power)
        {
            if (UnitStats.GetUpComponentsPrice(lvl) > MenuManager.Instance.PlayerStats.powerCoins)
            {
                return false;
            }
        }


        if (MenuManager.Instance.PlayerStats.goldCoins < UnitStats.GetUpGoldPrice(lvl))
        {
            return false;
        }
        else
        {
            MenuManager.Instance.PlayerStats.Pay(UnitStats.GetUpGoldPrice(lvl), CoinType.Gold);
            MenuManager.Instance.PlayerStats.Pay(UnitStats.GetUpComponentsPrice(lvl), coinType);
            result = true;
        }

        return result;
    }

    public static void AddOutcome(int goldCoins, int armorCoins, int powerCoins)
    {
        if (goldCoins > 0)
        {
            MenuManager.Instance.AddGoldCoins(goldCoins);
        }

        if (armorCoins > 0)
        {
            MenuManager.Instance.AddArmorCoins(armorCoins);
        }

        if (powerCoins > 0)
        {
            MenuManager.Instance.AddPowerCoins(powerCoins);
        }
    }
}
