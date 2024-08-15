using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode { None, Campaign, Defence, Editor }

public class GameModeSelector : MonoBehaviour
{
    [SerializeField] private GameObject missionsPanel;
    [SerializeField] private GameObject defencePanel;
    [SerializeField] private GameObject editorPanel;

    [SerializeField] private GameObject selectorPanel;

    [SerializeField] private GameObject missionBackButton;
    //[SerializeField] private GameObject defenceBackButton;

    private void Start()
    {
        if (GameStats.Instance == null) return;

        switch (GameStats.Instance.gameMode)
        {
            case GameMode.Campaign:
                SelectMissions(false);
                break;
            case GameMode.Defence:
                SelectDefence(false);
                break;
            case GameMode.Editor:
                SelectEditor(false);
                break;
        }

        GameStats.Instance.gameMode = GameMode.None;
    }

    public void SelectMissions()
    {
        SelectMissions(true);
    }

    private void SelectMissions(bool playClick)
    {
        missionsPanel.SetActive(true);
        defencePanel.SetActive(false);
        editorPanel.SetActive(false);

        selectorPanel.SetActive(false);

        missionBackButton.SetActive(true);

        if (playClick)
            SimpleSoundsManager.Instance.PlayClick();
    }

    public void SelectDefence()
    {
        SelectDefence(true);
    }

    private void SelectDefence(bool playClick)
    {
        missionsPanel.SetActive(false);
        defencePanel.SetActive(true);
        editorPanel.SetActive(false);

        selectorPanel.SetActive(false);

        missionBackButton.SetActive(false);

        if (playClick)
            SimpleSoundsManager.Instance.PlayClick();
    }

    public void SelectEditor()
    {
        SelectEditor(true);
    }

    private void SelectEditor(bool playClick)
    {
        missionsPanel.SetActive(false);
        defencePanel.SetActive(false);
        editorPanel.SetActive(true);

        selectorPanel.SetActive(false);

        missionBackButton.SetActive(false);

        if (playClick)
            SimpleSoundsManager.Instance.PlayClick();
    }

    public void BackToSelector()
    {
        missionsPanel.SetActive(true);
        defencePanel.SetActive(false);
        editorPanel.SetActive(false);

        selectorPanel.SetActive(true);

        missionBackButton.SetActive(false);

        SimpleSoundsManager.Instance.PlayClick();
    }
}
