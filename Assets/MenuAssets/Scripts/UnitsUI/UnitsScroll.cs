using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsScroll : MonoBehaviour, IAttention
{
    public List<UnitUI> units;
    public UnitUI currentUnit;

    public int centerSlotId;
    public int leftSlotId;
    public int rightSlotId;

    [SerializeField] private string panelLeftAnimName;
    [SerializeField] private string panelRightAnimName;

    public bool triggerPulled = false;

    public List<Transform> slots;

    //[SerializeField] private List<GameObject> prefabs;
    [SerializeField] private GameObject basePrefab;
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject helicopterPrefab;
    [SerializeField] private GameObject fabricPrefab;
    [SerializeField] private GameObject drillPrefab;
    [SerializeField] private GameObject towerPrefab;

    private void Start()
    {

    }

    public void InitScroll()
    {
        List<UnitStats> unitsStats = MenuManager.Instance.PlayerStats.unitsStats;
        units = new List<UnitUI>();

        for (int i = 0; i < unitsStats.Count; i++)
        {
            if (unitsStats[i].Unit == Unit.Plate) continue;

            UnitStats stats = unitsStats[i];

            GameObject newUnit = Instantiate(SelectPrefab(unitsStats[i]), slots[i]);
            UnitUI unit;
            if (i == 2)
            {
                //newUnit = Instantiate(prefabs[1], slots[i]);
                unit = newUnit.GetComponent<UnitUI>();
                unit.panelController.PanelStayOpen();
                unit.PlayCenter();
            }
            else
            {
                //newUnit = Instantiate(prefabs[0], slots[i]);
                unit = newUnit.GetComponent<UnitUI>();
                unit.panelController.PanelStayClose();
            }
            unit.SetStats(stats);
            unit.UnitsScroll = this;
            unit.Slot = slots[i];
            unit.SlotId = i;
            //unit.text.text = i.ToString();
            unit.text.text = "";

            stats.CountStats();

            unit.panelController.SpeedStat.TextUpdate((int)stats.Speed, stats.speedLvl);
            unit.panelController.ArmorStat.TextUpdate((int)stats.Armor, stats.armorLvl);
            unit.panelController.GunStat.TextUpdate((int)stats.Damage, stats.gunLvl);

            units.Add(unit);

            MenuManager.Instance.AddUIStat(unit.panelController.ArmorStat);
            MenuManager.Instance.AddUIStat(unit.panelController.GunStat);
            MenuManager.Instance.AddUIStat(unit.panelController.SpeedStat);
        }

        MenuManager.Instance.UiStatsUpdate();
    }

    private void OnEnable()
    {
        ReOpen();
    }

    private GameObject SelectPrefab(UnitStats unitStats)
    {
        var result = tankPrefab;

        switch (unitStats.Unit)
        {
            case Unit.Tank:
                return tankPrefab;
            case Unit.Helicopter:
                return helicopterPrefab;
            case Unit.Base:
                return basePrefab;
            case Unit.Fabric:
                return fabricPrefab;
            case Unit.Drill:
                return drillPrefab;
            case Unit.Tower:
                return towerPrefab;
        }

        return result;
    }

    public void SetNext()
    {
        if (units[0].IsMoving) return;

        triggerPulled = false;

        var tmp = new List<UnitUI> {null, null, null, null, null, null };
        for (int i = 0; i < units.Count - 1; i++)
        {
            tmp[i + 1] = units[i];
        }
        tmp[0] = units[units.Count - 1];
        units = tmp;

        for (int i = 0; i < units.Count; i++)
        {
            units[i].InitMoving(slots[i], i);

            NextAnimChooser(i);
        }

        SimpleSoundsManager.Instance.PlayClick();
    }

    public void SetPrev()
    {
        if (units[0].IsMoving) return;

        triggerPulled = false;

        var tmp = new List<UnitUI> { null, null, null, null, null, null };
        for (int i = 1; i < units.Count; i++)
        {
            tmp[i - 1] = units[i];
        }
        tmp[units.Count - 1] = units[0];
        units = tmp;

        for (int i = 0; i < units.Count; i++)
        {
            units[i].InitMoving(slots[i], i);

            PrevAnimChooser(i);
        }

        SimpleSoundsManager.Instance.PlayClick();
    }

    private void ReOpen()
    {
        foreach(UnitUI unit in units)
        {
            if(unit.SlotId == centerSlotId)
            {
                unit.panelController.PanelStayOpen();
                unit.PlayCenter();
            }
        }
    }

    private void NextAnimChooser(int pos)
    {
        if (pos == leftSlotId + 1)
        {
            units[pos].PlayLeftToCenter();
            units[pos].panelController.PanelPlayOpen();
        }
        else if (pos == centerSlotId + 1)
        {
            units[pos].PlayCenterToRight();
            units[pos].panelController.PanelPlayClose();
        }
        else
        {
            units[pos].PlayRight();
            units[pos].panelController.PanelStayClose();
        }
    }

    private void PrevAnimChooser(int pos)
    {
        if (pos == rightSlotId - 1)
        {
            units[pos].PlayRightToCenter();
        }
        else if (pos == centerSlotId - 1)
        {
            units[pos].PlayCenterToLeft();
        }
        else
        {
            units[pos].PlayLeft();
        }
    }

    public UnitUI GetCurrentUnit()
    {
        return units[centerSlotId];
    }

    public void UnitsUpdate()
    {
        foreach(UnitUI unit in units)
        {
            unit.PutInSlot();
        }
    }

    bool IAttention.Check()
    {
        foreach(UnitStats unit in MenuManager.Instance.PlayerStats.unitsStats)
        {
            if (unit.UpgradeAvaible(
                MenuManager.Instance.PlayerStats.goldCoins,
                MenuManager.Instance.PlayerStats.powerCoins,
                MenuManager.Instance.PlayerStats.armorCoins
                ))
            {
                if (unit.Unit == Unit.Plate) continue;
                return true;
            }
        }

        return false;
    }
}
