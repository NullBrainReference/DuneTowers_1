using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] private GameObject armorPanel;
    [SerializeField] private GameObject damagePanel;
    [SerializeField] private GameObject speedPanel;

    private StatUnit armorStat;
    private StatUnit damageStat;
    private StatUnit speedStat;

    private void Start()
    {
        armorStat = armorPanel.GetComponent<StatUnit>();
        damageStat = damagePanel.GetComponent<StatUnit>();
        speedStat = speedPanel.GetComponent<StatUnit>();
    }

    public void ShowStats(UnitStats stats)
    {
        armorPanel.SetActive(stats.HasArmor());
        damagePanel.SetActive(stats.HasGun());
        speedPanel.SetActive(stats.HasSpeed());

        armorStat.TextUpdate((int)stats.Armor, 1);
        damageStat.TextUpdate((int)stats.Damage, 1);
        speedStat.TextUpdate((int)stats.Speed, 1);
    }
}
