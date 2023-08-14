using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ProductionUnit : MonoBehaviour
{
    public ProductionType productionType;
    private Button button;

    [SerializeField] private GameObject selectMask;
    [SerializeField] private ProductionGroupController groupController;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnClick()
    {
        CanvasManager.Instance.productionType = productionType;
        CanvasManager.Instance.currUnitCreator.SwitchProductionType();
        groupController.Select(this);
    }

    public void Switch(bool isSelected)
    {
        button.interactable = !isSelected;
        selectMask.SetActive(isSelected);
    }
}
