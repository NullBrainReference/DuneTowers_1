using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatItemUI : MonoBehaviour
{
    [SerializeField] private Text valueText;
    [SerializeField] private bool isStandalone;

    public ProfileStat profileStat;

    private void Start()
    {
        if (isStandalone)
        {
            StatItem statItem = GameStats.Instance.Profile.StatItems.Find(x => x.type == profileStat);

            if (statItem == null)
                return;

            SetValue(statItem);
        }
    }

    public void SetValue(StatItem statItem)
    {
        valueText.text = statItem.value.ToString("0");
    }
}
