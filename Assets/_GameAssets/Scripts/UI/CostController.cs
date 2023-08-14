using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CostController : MonoBehaviour
{
    public float cost;

    public Text costText;
    private Button button;
    [SerializeField] private BuildButtonController buildController;

    private void Start()
    {
        button = GetComponent<Button>();
        InitCost();
    }

    private void Update()
    {
        button.interactable = cost <= MoneyManager.Instance(0).Money + MoneyManager.Instance(0).reservedMoney;
    }

    public void InitCost()
    {
        if (costText == null) 
            return;

        cost = GameStats.Instance.player.GetPrice(buildController.unit, UnitsCollector.Instance(0));
        costText.text = string.Format("{0}c", cost);
    }

    public void SetSelect(bool select)
    {
        if (gameObject == null) return;

        gameObject.GetComponent<BuildButtonController>().SetSelect(select);
    }
}
