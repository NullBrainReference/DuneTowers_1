using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PlayerBase
{
    [SerializeField] public List<UnitStats> unitsStats;

    public UnitStats GetUnit(Unit unit)
    {
        return unitsStats.Find(x => x.Unit == unit).Copy();
    }

    public int GetPrice(Unit unit, UnitsCollector unitsCollector)
    {
        return unitsStats.Find(x => x.Unit == unit).GetPrice(unitsCollector);
    }

    public int GetStartGold()
    {
        int result = 1000;

        foreach (UnitStats unitStats in unitsStats)
        {
            if (unitStats.Unit == Unit.Plate) continue;

            result += (unitStats.gunLvl + unitStats.speedLvl + unitStats.armorLvl);
        }

        return result;
    }

    public void StatsLvlIncrement()
    {
        foreach (UnitStats unit in unitsStats)
        {
            if (unit.HasGun())
            {
                unit.gunLvl++;
            }

            if (unit.HasArmor())
            {
                unit.armorLvl++;
            }

            unit.CountStats();
        }
    }
}
