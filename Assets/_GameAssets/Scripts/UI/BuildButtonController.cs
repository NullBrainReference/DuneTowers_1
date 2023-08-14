using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    public struct BuildObject
    {
        public GameObject prefab;
        //public Sprite sprites;
    }

    [SerializeField] private GameObject selectMask;
    [SerializeField] private GameObject nameText;

    [SerializeField] private KeyCode keyCode;

    public Unit unit;
    public BuildObject buildObject;
    public BuildPanelController buildPanelController;
    public CostController costController;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(
            () => 
                {
                    Click();
                    //HeadquartersUnitController.Instance.OnClickMenu(buildObject.prefab, unit);
                    //CanvasManager.Instance.cancelButton.SetActive(true);
                    //CanvasManager.Instance.ResetBuildSelect();
                    //
                    //SimpleSoundsManager.Instance.PlayClick();
                    //SetSelect(true);
                    //
                    //MoneyManager.Instance(0).ReturnReservedMoney();
                    //MoneyManager.Instance(0).ReserveMoney(costController.cost);
                    //
                    //costController.InitCost();
                    ////buildPanelController.DestroyPanel();
                }
            );
    }

    private void Click()
    {
        HeadquartersUnitController.Instance.OnClickMenu(buildObject.prefab, unit);
        CanvasManager.Instance.cancelButton.SetActive(true);
        CanvasManager.Instance.ResetBuildSelect();

        SimpleSoundsManager.Instance.PlayClick();
        SetSelect(true);

        MoneyManager.Instance(0).ReturnReservedMoney();
        MoneyManager.Instance(0).ReserveMoney(costController.cost);

        costController.InitCost();
        //buildPanelController.DestroyPanel();
    }

    private void Update()
    {
        if (button.interactable == false)
            return;

        if (Input.GetKeyDown(keyCode))
        {
            Click();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnMouseEnter");
        nameText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        nameText.SetActive(false);
    }

    public void SetSelect(bool select)
    {
        selectMask.SetActive(select);
    }
}
