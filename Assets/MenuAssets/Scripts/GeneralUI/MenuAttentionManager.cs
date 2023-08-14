using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAttentionManager : MonoBehaviour
{
    public GameObject unitsAttentionGO;
    public GameObject shopAttentionGO;

    public GameObject unitsTarget;
    public GameObject[] shopTargets;

    public IAttention unitsAttention;
    public IAttention[] shopAttentions;

    public bool ready = false;

    private void Start()
    {
        unitsAttention = unitsTarget.GetComponent<UnitsScroll>() as IAttention;

        shopAttentions = new IAttention[shopTargets.Length];
        for (int i = 0; i < shopTargets.Length; i++)
        {
            shopAttentions[i] = shopTargets[i].GetComponent<SpanObjectUpdater>() as IAttention;
        }

        ready = true;
        MenuManager.Instance.UiStatsUpdate();
    }

    public void UpdateAttentions()
    {
        if (ready == false) return;

        unitsAttentionGO.SetActive(unitsAttention.Check());

        var profile = PlayerProfile.Load();

        if (profile == null)
            return;
        else if (profile.Level <= 1 && profile.LevelProgress < 200)
            return;

        bool shopAttention = false;
        foreach (IAttention attention in shopAttentions)
        {
            if (attention.Check()) shopAttention = true;
        }
        shopAttentionGO.SetActive(shopAttention);
    }
}
