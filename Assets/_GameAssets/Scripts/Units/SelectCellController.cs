using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectCellController : UnitController
{
    public Unit unit;
    public GameObject prefab;

    [SerializeField] private TextMeshPro creditsText;
    [SerializeField] private SliderUnit creditsSlider;

    public void InitCreditsValue(CellController cell)
    {
        if(cell.Credits == 0)
        {
            creditsText.text = "";
            creditsSlider.gameObject.SetActive(false);
            return;
        }

        creditsText.text = cell.Credits.ToString("0");
        creditsSlider.SetValue((float)cell.Credits / cell.StartCredits);
        creditsSlider.gameObject.SetActive(true);
    }

    public void HideCreditSlider()
    {
        creditsText.text = "";
        creditsSlider.gameObject.SetActive(false);
    }

    public override void OnShortTouch(CellController cellController, UnitController xUnitController)
    {
        CanvasManager.Instance.SwitchFabricPanel(false);

        if (!IsPlayer)
            return;

        if (ScanerManager.Instance.IsScanning && unitStats.Unit == Unit.None)
            return;


        //print($"object {prefab.name} created {cellController.position}");

        HeadquartersUnitController.Instance.DestroySelectPlaces(true);

        if (cellController.Child != null && cellController.Child.GetType() == typeof(FoundationSlabUnitController))
            cellController.Child.DestroyUnit(); //Destroy(cellController.Child.gameObject);


        GameObject newObject = Instantiate(prefab, UnitManager.Instance.transform);
        newObject.transform.localPosition = (Vector2)cellController.transform.localPosition;
        newObject.transform.rotation = cellController.transform.rotation;
        UnitController unitController = newObject.GetComponent<UnitController>();

        unitController.unitStats = this.unitStats;

        if (ScanerManager.Instance.IsScanning)
            if (unitStats.Unit != Unit.Plate)
                ScanerManager.Instance.HideScanerSlider(cellController.position);

        MoneyManager.Instance(0).PayReservedMoney();

        CanvasManager.Instance.cancelButton.SetActive(false);
        CanvasManager.Instance.ResetBuildSelect();

        cellController.SetChild2(unitController);

        unitController.OnBuildInit();
    }

    public static GameObject Create(GameObject fieldSelectPlacePrefab, CellController cellController, GameObject unitPrefab, Unit unit)
    {
        GameObject result = Instantiate(fieldSelectPlacePrefab, TempObjectsManager.Instance.transform);
        result.name += $"({cellController.position.x},{cellController.position.y})";

        //Vector3 resultPos = CellsManager.Instance.cellControllersArr[cellController.position.x, cellController.position.y].transform.position;

        //if (cellController.Child != null)
        Vector3 resultPos = new Vector3(
                CellsManager.Instance.cellControllersArr[cellController.position.x, cellController.position.y].transform.position.x,
                CellsManager.Instance.cellControllersArr[cellController.position.x, cellController.position.y].transform.position.y,
                TempObjectsManager.Instance.heightOffset
                );

        result.transform.position = resultPos;

        //result.transform.position = CellsManager.Instance.cellControllersArr[cellController.position.x, cellController.position.y].transform.position;
        SelectCellController selectCellController = result.GetComponent<SelectCellController>();
        selectCellController.prefab = unitPrefab;

        selectCellController.unitStats = GameStats.Instance.player.GetUnit(unit);

        CellsManager.Instance.cellControllersArr[cellController.position.x, cellController.position.y].SetSelect(selectCellController);

        selectCellController.InitCreditsValue(cellController);

        return result;
    }
}
