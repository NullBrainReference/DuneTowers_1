using UnityEngine;
//using UnityEngine.UI;
using TMPro;
using System;

public class StatsGenerator : MonoBehaviour
{
    [SerializeField] private TMP_InputField armorInput;
    [SerializeField] private TMP_InputField damageInput;
    [SerializeField] private TMP_InputField speedInput;

    [SerializeField] private TMP_Dropdown dropdown;

    public LevelStats levelStats;
    public Unit currUnit;

    private void Awake()
    {
        levelStats.InitUnitsStatsEmpty();

        currUnit = Unit.Tank;

        armorInput.text = "0";
        damageInput.text = "0";
        speedInput.text = "0";
    }

#if UNITY_EDITOR
    public void AutoStats()
    {
        levelStats = new LevelStats();

        int lvl = MapGenerator.Instance.lvl;

        levelStats.DefaultsTest(lvl);
        levelStats.id = lvl;

        levelStats.SaveToFile();
    }
#endif

    public void FillActualStats()
    {
        UnitStats stats = levelStats.GetUnit(currUnit);

        if (armorInput.gameObject.activeInHierarchy)
            armorInput.text = stats.armorLvl.ToString();
        if (damageInput.gameObject.activeInHierarchy)
            damageInput.text = stats.gunLvl.ToString();
        if (speedInput.gameObject.activeInHierarchy)
            speedInput.text = stats.speedLvl.ToString();
    }

    public void ChangeUnit()
    {
        var currValue = dropdown.value;

        switch (currValue)
        {
            case 0:
                SetUpUnit(currUnit);
                UpdateInputs(true, true, true);
                currUnit = Unit.Tank;
                break;
            case 1:
                SetUpUnit(currUnit);
                UpdateInputs(true, false, false);
                currUnit = Unit.Base;
                break;
            case 2:
                SetUpUnit(currUnit);
                UpdateInputs(true, false, true);
                currUnit = Unit.Fabric;
                break;
            case 3:
                SetUpUnit(currUnit);
                UpdateInputs(true, false, true);
                currUnit = Unit.Drill;
                break;
            case 4:
                SetUpUnit(currUnit);
                UpdateInputs(true, true, true);
                currUnit = Unit.Tower;
                break;
        }

        FillActualStats();
    }

    public void WriteStats()
    {
        SetUpUnit(currUnit);
    }

#if UNITY_EDITOR
    public void SaveStats()
    {
        levelStats.id = MapGenerator.Instance.lvl;
        levelStats.InitDefaultScore();
        levelStats.SaveToFile();
    }
#endif

    private void SetUpUnit(Unit unit)
    {
        try
        {
            UnitStats stats = levelStats.GetUnit(unit);

            if (armorInput.gameObject.activeInHierarchy)
                stats.armorLvl = Convert.ToInt32(armorInput.text);
            if (damageInput.gameObject.activeInHierarchy)
                stats.gunLvl = Convert.ToInt32(damageInput.text);
            if (speedInput.gameObject.activeInHierarchy)
                stats.speedLvl = Convert.ToInt32(speedInput.text);
        }
        catch
        {
            Debug.Log(unit.ToString() + " setup failed");
        }
    } 

    private void UpdateInputs(bool armor, bool dmg, bool speed)
    {
        armorInput.gameObject.SetActive(armor);
        damageInput.gameObject.SetActive(dmg);
        speedInput.gameObject.SetActive(speed);
    }
}
