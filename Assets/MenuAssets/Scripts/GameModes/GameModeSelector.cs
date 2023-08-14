using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeSelector : MonoBehaviour
{
    [SerializeField] private GameObject missionsPanel;
    [SerializeField] private GameObject defencePanel;

    [SerializeField] private GameObject selectorPanel;

    [SerializeField] private GameObject missionBackButton;
    //[SerializeField] private GameObject defenceBackButton;

    public void SelectMissions()
    {
        missionsPanel.SetActive(true);
        defencePanel.SetActive(false);

        selectorPanel.SetActive(false);

        missionBackButton.SetActive(true);

        SimpleSoundsManager.Instance.PlayClick();
    }

    public void SelectDefence()
    {
        missionsPanel.SetActive(false);
        defencePanel.SetActive(true);

        selectorPanel.SetActive(false);

        missionBackButton.SetActive(false);

        SimpleSoundsManager.Instance.PlayClick();
    }

    public void BackToSelector()
    {
        missionsPanel.SetActive(true);
        defencePanel.SetActive(false);

        selectorPanel.SetActive(true);

        missionBackButton.SetActive(false);

        SimpleSoundsManager.Instance.PlayClick();
    }
}
