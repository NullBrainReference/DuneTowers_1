using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DeviceController;

public class HeadquartersUnitController : UnitController
{

    public static HeadquartersUnitController Instance { private set; get; }

    [Space]
    public SliderUnit sliderUnit;

    public GameObject fieldSelectPlacePrefab;
    public GameObject buildPanelPrefab;

    [SerializeField]
    private GameObject unitPrefab;

    [SerializeField]
    private List<GameObject> fieldSelectPlaces = new List<GameObject>();


    private DeviceHeadquartersController deviceHeadquartersController;

    private void Awake()
    {
        if (!Instance && IsPlayer)
        {
            Instance = this;
            //Button createUnit = GameObject.FindGameObjectWithTag("CreateUnit").GetComponent<Button>();
            //createUnit.onClick.AddListener(OpenMenu);
        }

        deviceHeadquartersController = GetComponent<DeviceHeadquartersController>();

        unitStats = GameStats.Instance.GetUnit(Unit.Base, PlayerNo);
        InitUnitStats();
    }

    protected override void UpdateSlider()
    {
        //print("curHealth / health = " + (curHealth / health));
        sliderUnit.SetValue(CurHealthPercent);
    }



    public override void OnLongTouch(CellController cellController, UnitController xUnitController)
    {
        if (!IsPlayer)
            return;
        //SelectManager.Instance.SelectObject(transform);
        print("OnLongTouch " + TransformUrlUtil.GetTransformFullName(transform));
    }

    public override void OnShortTouch(CellController cellController, UnitController xUnitController)
    {
        if (!IsPlayer)
            return;

        base.OnShortTouch(cellController, xUnitController);

        //OpenMenu();

        //CreateSelectPlaces();
        //fieldSelectPlacePrefab 
    }


    public void DestroySelectPlaces(bool resetCreatingPrefab)
    {
        CanvasManager.Instance.ResetBuildSelect();

        if (fieldSelectPlaces.Count != 0)
        {
            foreach (GameObject fieldSelectPlace in fieldSelectPlaces)
            //foreach (CellController cellController in fieldSelectPlaces.FindAll(b => b != exceptСellController))
            {
                Destroy(fieldSelectPlace);
            }
            fieldSelectPlaces.Clear();
        }
        if (resetCreatingPrefab)
            unitPrefab = null;
    }


    private new void OnDestroy()
    {
        if (GameStats.Instance == null) 
            return;

        //if (GameStats.Instance.goldOutcome == 0)
        //{
        //ShowBannerController.Instance.CallBanner();

        if (GameStats.Instance.gameResult != GameResult.None)
            return;

        GameStats.Instance.CountOutcomes();
        GameStats.Instance.CallOutcomePanel(PlayerNo);
        //}

        GameObject obj = GameObject.FindGameObjectWithTag("CreateUnit");
        if (obj is not null)
        {
            Button createUnit = obj.GetComponent<Button>();
            createUnit.onClick.RemoveListener(OpenMenu);
        }
        base.OnDestroy();
        DestroySelectPlaces(true);
    }

    //public override void Unselect()
    //{
    //    base.Unselect();
    //    print("Unselect1");
    //    foreach (GameObject obj in fieldSelectPlaces)
    //    {
    //        Destroy(obj);
    //    }
    //}




    private void CreateSelectPlaces(GameObject unitPrefab, Unit unit)
    {
        this.unitPrefab = unitPrefab;
        DestroySelectPlaces(false);

        List<CellController> cellControllers = GetCellControllersForSelectPlaces();
        //print("cellControllers.Count=" + cellControllers.Count);
        foreach (CellController cellController in cellControllers)
        {
            fieldSelectPlaces.Add(SelectCellController.Create(fieldSelectPlacePrefab, cellController, unitPrefab, unit));
        }
    }

    private void UpdateSelectFields()
    {
        CreateSelectPlaces(unitPrefab, Unit.Tower);
        //List<CellController> cellControllersSelected = CellsManager.Instance.cellControllers.FindAll(b => b.SelectCellController != null);
        //List<CellController> cellControllers = GetCellControllersForSelectPlaces();

        //foreach(CellController cellController in cellControllersSelected)

    }

    public List<CellController> GetCellControllersForSelectPlaces()
    {
        List<CellController> result = new List<CellController>();
        if (IsPlayer)
        {
            List<CellController> cellControllersSelected = CellsManager.Instance.cellControllers.FindAll(b => b.SelectCellController != null);
            foreach (CellController cell in cellControllersSelected.ToArray())
                cell.SetSelect(null);
        }

        List<CellController> cellControllers = CellsManager.Instance.cellControllers.FindAll(b => b.ChildForBuild(PlayerNo));
        foreach (CellController cellController in cellControllers)
        {
            for (int x = cellController.position.x - 1; x <= cellController.position.x + 1; x++)
                for (int y = cellController.position.y - 1; y <= cellController.position.y + 1; y++)
                    if (CellsManager.Instance.InRangeArray(x, y))
                    {
                        CellController nearCellController = CellsManager.Instance.cellControllersArr[x, y];

                        if (nearCellController.SelectCellController != null) continue;

                        if (nearCellController.IsFreeToBuild())
                        {
                            if (!result.Contains(nearCellController))
                                result.Add(nearCellController);
                        }

                        //if (nearCellController.Child == null)
                        //{
                        //    if (!result.Contains(nearCellController))
                        //        if (nearCellController.IsFreeToBuild())
                        //            result.Add(nearCellController);
                        //}
                        //else if (nearCellController.Child.unitStats.Unit == Unit.Plate)
                        //{
                        //    if (!result.Contains(nearCellController))
                        //        if (nearCellController.IsFreeToBuild())
                        //            result.Add(nearCellController);
                        //}
                    }
        }
        return result;
    }

    public void OpenMenu()
    {
        if (CanvasManager.Instance.isMenuOpened) return;

        Instantiate(buildPanelPrefab, CanvasManager.Instance.buildPanelParent);
        CanvasManager.Instance.isMenuOpened = true;
    }

    internal void ReareateSelectCells()
    {
        //print("ReareateSelectCells " + creatingPrefab?.name);
        if (unitPrefab != null)
            UpdateSelectFields();
        else
            DestroySelectPlaces(true);

    }

    /// <summary>
    /// создание объекта  
    /// </summary>
    /// <param name="prefab"></param>
    public void OnClickMenu(GameObject prefab, Unit unit)
    {
        CreateSelectPlaces(prefab, unit);
    }


    internal override void ApplyAction(SituationResult situationResult)
    {

        if (situationResult == default)
            return;

        if (situationResult.situationResultType == SituationResultType.Build)
        {
            var newObj = Instantiate(situationResult.prefab, UnitManager.Instance.transform);
            UnitController unitController = newObj.GetComponent<UnitController>();
            
            unitController.PlayerNo = PlayerNo;

            GameObject cell = MapManager.Instance.cells[situationResult.position.x, situationResult.position.y];
            CellController cellController = cell.GetComponent<CellController>();
            cellController.SetChild2(unitController);
            cellController.Child.transform.localPosition = cellController.transform.position;
 
            unitController.Init(situationResult.paramDict);
            MoneyManager.Instance(PlayerNo).WithdrawMoney(unitController.unitStats.GetPrice(UnitsCollector.Instance(1)));

            unitController.OnBuildInit();

            ScanerManager.Instance.HideScanerSlider(cellController.position);
            //ScanerManager.Instance.RemoveScaner(cellController.position);

            if (unitController.unitStats.Unit == Unit.Drill)
            {
                TankerUnitController tanker = (TankerUnitController)unitController;
                tanker.InitCellValues();
            }
            //UnitsCollector.Instance(1).AddUnit(unitController);
        }
    }


    internal override SituationResult GetSituationResult(List<UnitController> units, List<UnitController> enemyUnits)
    {

        return deviceHeadquartersController.GetSituationResult(units, enemyUnits);
    }

}
