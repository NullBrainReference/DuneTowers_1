using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance { get; private set; }
    public Transform buildPanelParent;
    public GameObject cancelButton;

    public GameObject fabricPanel;
    public ProductionGroupController productionController;
    public FactoryUnitCreator currUnitCreator = null;

    public List<CostController> costControllers;

    public OutcomePanelController outcomePanel;

    public SellButton sellButton;

    public bool isMenuOpened = false;
    //public bool isFabricSelected = false;

    public ProductionType productionType = ProductionType.None;

    private void Awake()
    {
        Instance = this;
        GameStats.Instance.outcomePanelController = outcomePanel;
        GameStats.Instance.PassOutcome();
        GameStats.Instance.battleTimer.InitTimer();

        GameStats.Instance.gameResult = GameResult.None;
    }

    public void SwitchFabricPanel(bool isSelected)
    {
        if (fabricPanel == null)
            return;

        fabricPanel?.SetActive(isSelected);

        productionType = ProductionType.None;
    }

    public void UpdateCost()
    {
        foreach (var costController in costControllers)
        {
            costController.InitCost();
        }
    }

    public void ResetBuildSelect()
    {
        foreach(var costController in costControllers)
        {
            if (costController == null)
                continue;
            costController.SetSelect(false);
        }
    }
}
