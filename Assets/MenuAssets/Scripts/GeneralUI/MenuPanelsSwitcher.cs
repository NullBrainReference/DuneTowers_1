using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelsSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject unitsPanel;
    [SerializeField] private GameObject levelsPanel;
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private GameObject unitsMask;
    [SerializeField] private GameObject levelsMask;
    [SerializeField] private GameObject shopMask;

    [SerializeField] private ProfileUI profileUI;

    [SerializeField] private GameModeSelector gameModeSelector;

    public void ShowUnitsPanel()
    {
        ShowBannerController.Instance.CallBanner();

        SimpleSoundsManager.Instance.PlayClick();

        shopPanel.SetActive(false);
        levelsPanel.SetActive(false);
        unitsPanel.SetActive(true);

        shopMask.SetActive(false);
        levelsMask.SetActive(false);
        unitsMask.SetActive(true);

        profileUI.ClosePanel();
    }

    public void ShowLevelsPanel()
    {
        ShowBannerController.Instance.CallBanner();

        SimpleSoundsManager.Instance.PlayClick();

        shopPanel.SetActive(false);
        levelsPanel.SetActive(true);
        unitsPanel.SetActive(false);

        shopMask.SetActive(false);
        levelsMask.SetActive(true);
        unitsMask.SetActive(false);

        profileUI.ClosePanel();

        gameModeSelector.BackToSelector();
    }

    public void ShowShopPanel()
    {
        //ShowBannerController.Instance.CallBanner();

        SimpleSoundsManager.Instance.PlayClick();

        shopPanel.SetActive(true);
        levelsPanel.SetActive(false);
        unitsPanel.SetActive(false);

        shopMask.SetActive(true);
        levelsMask.SetActive(false);
        unitsMask.SetActive(false);

        profileUI.ClosePanel();
    }
}
