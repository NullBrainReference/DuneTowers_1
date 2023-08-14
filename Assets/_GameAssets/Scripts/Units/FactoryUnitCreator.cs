using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TankDeviceTargetController;

public enum ProductionType { None, Tank, Helicopter }

public class FactoryUnitCreator : MonoBehaviour
{
    //public TankFactoryUnitController tankFactoryUnitController;
    public bool isSpawner;

    public GameObject prefabObject;
    public GameObject prefabHelicopter;

    public ProductionType productionType;

    private CellController defaultCellTank;

    //public float repeatRatePercent;

    //[Header("????? ????? ???????")]
    public float repeatRate = 0.5f;

    //[Header("???-?? ?????? ?? ???????")]
    public float increasePercentage = 40;

    //[Header("????????? ???????")]
    public float cost = 100f;

    //[Header("??????? ???????? ?????")]
    [Range(0f,1f)]
    public float percent;

    private float costPart;
    private float percentPart;

    [SerializeField]
    private UnitController curUnitController;
    public UnitController CurUnitController { get { return curUnitController; } }

    [SerializeField] private FabricStageIndicator stageIndicator;

    //[SerializeField]
    //public Vector2Int defaultPosition;


    private UnitController unitControllerFactory;

    public UnitController UnitControllerFactory
    {
        get
        {
            if (unitControllerFactory == null)
                unitControllerFactory = GetComponent<UnitController>();
            
            return unitControllerFactory;
        }
    }

    //private UnitController xUnit = null;
    private bool facroryFree = true; 

    public bool FacroryFree
    { 
        get 
        {
            return unitControllerFactory.CellControllerBase.Child == unitControllerFactory;  
        } 
    }

    private void Start()
    {
        if (isSpawner)
            return;

        cost = PlayerPrefs.GetInt("TankPrice");
        percentPart = 1f / increasePercentage;
        costPart = cost / increasePercentage;
        StartCoroutine(AddIncreasePercentage());

        stageIndicator.SetPercent(0);

        //if (unitControllerFactory.PlayerNo == 1)
        //    InvokeRepeating("AutoHelicopterSwitch", 3f, 15f);
    }

    private bool IsFreeAround()
    {
        bool isFree = true;

        Vector2Int pos = unitControllerFactory.CellControllerBase.position;

        bool left = true;
        bool right = true;
        bool top = true;
        bool bottom = true;
        
        if (pos.y + 1 >= CellsManager.Instance.size.y)
            top = false;
        else if (CellsManager.Instance.cellControllersArr[pos.x, pos.y + 1].IsFreeToBuild() == false)
            top = false;

        if (pos.y - 1 < 0)
            bottom = false;
        else if (CellsManager.Instance.cellControllersArr[pos.x, pos.y - 1].IsFreeToBuild() == false)
            bottom = false;

        if (pos.x + 1 >= CellsManager.Instance.size.x)
            right = false;
        else if (CellsManager.Instance.cellControllersArr[pos.x + 1, pos.y].IsFreeToBuild() == false)
            right = false;

        if (pos.x - 1 < 0)
            left = false;
        else if (CellsManager.Instance.cellControllersArr[pos.x - 1, pos.y].IsFreeToBuild() == false)
            left = false;

        if (!left && !right && !top && !bottom)
        {
            isFree = false;
        }
        else
        {
            isFree = true;
        }

        return isFree;
    }

    private List<Vector2Int> GetBlocks(List<UnitController> units)
    {
        var result = new List<Vector2Int>();
        foreach (var unit in units)
            if (unit.CellControllerBase != null)
                result.Add(unit.CellControllerBase.position);
        return result;
    }

    private CourseSearcher GetCourseSearcher(List<UnitController> units, List<UnitController> obstructions)
    {
        var blocks = GetBlocks(units);
        blocks.AddRange(GetBlocks(obstructions));
        var position = unitControllerFactory.CellControllerBase.position;
        CourseSearcher courseSearcher = new CourseSearcher(MapManager.Instance.fieldSize, position, blocks);
        //courseSearcher.FillNearFields(position.x, position.y, 0);

        return courseSearcher;
    }

    private bool CheckWay(EnemyItem enemyItem, List<Vector2Int> blocks)
    {
        CourseSearcher courseSearcher = new CourseSearcher(MapManager.Instance.fieldSize);
        var pos = unitControllerFactory.CellControllerBase.position;
        return courseSearcher.GetNextStep(pos, enemyItem.Pos, blocks) != pos;
    }

    private EnemyItem GetEnemyItem(List<EnemyItem> enemyItems, List<Vector2Int> blocks)
    {
        foreach (var item in enemyItems)
        {
            if (CheckWay(item, blocks))
                return item;
        }
        return default;
    }

    public void FindNewDefaultCell()
    {
        var units = UnitsCollector.Instance(unitControllerFactory.PlayerNo).GetAllUnits();
        var enemyUnits = UnitsCollector.Instance(Math.Abs(unitControllerFactory.PlayerNo - 1)).GetAllUnits();

        var unitsWithoutPlate = units.FindAll(x => x.unitStats.Unit != Unit.Plate);
        var obstructions = ObstructionsManager.Instance.all;
        CourseSearcher courseSearcher = GetCourseSearcher(unitsWithoutPlate, obstructions);

        var enemyUnitsWithoutObstructions = enemyUnits.FindAll(x => x.unitType != UnitController.UnitType.Obstruction);

        List<EnemyItem> enemyItems = new List<EnemyItem>();

        foreach (var item in enemyUnitsWithoutObstructions)
        {
            if (item.CellControllerBase != null)
            {
                var pos = item.CellControllerBase.position;
                enemyItems.Add(new EnemyItem(item, courseSearcher.Mas[pos.x, pos.y], pos));
            }
        }

        enemyItems.Sort((x, y) => y.significance.CompareTo(x.significance));

        var enemyUnitsWithoutPlate = enemyUnits.FindAll(x => x.unitStats.Unit != Unit.Plate);

        EnemyItem enemyItem = enemyItems.Find(x => x.unitController.unitStats.Unit == Unit.Base);

        if (enemyItem == null)
            return;

        var freePos = GetClosestFreePos(enemyItem.Pos);

        //Debug.Log("_enemyItem_Pos_" + freePos.x + "_" + freePos.y);

        IDefaultPosition unitIDefaultPosition = ((IDefaultPosition)UnitControllerFactory);
        //defaultCellTank = unitIDefaultPosition.DefaultPosition;
        unitIDefaultPosition.DefaultPosition = CellsManager.Instance.cellControllersArr[freePos.x, freePos.y];
    }

    private Vector2Int GetClosestFreePos(Vector2Int pos)
    {
        var cells = CellsManager.Instance.cellControllersArr;

        List<Vector2Int> poses = new List<Vector2Int>();

        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (cells[i, j].Child == null)
                    poses.Add(cells[i, j].position);
            }
        }

        poses.Sort((x,y) => Math.Abs(x.x - pos.x) + Math.Abs(x.y - pos.y).CompareTo(Math.Abs(y.x - pos.x) + Math.Abs(y.y - pos.y)));

        return poses[0];
    }

    public void AutoTankSwitch()
    {
        if (isSpawner)
            return;

        productionType = ProductionType.Tank;

        RecountProgress();
    }

    public void AutoHelicopterSwitch()
    {
        if (isSpawner)
            return;

        productionType = ProductionType.Helicopter;

        RecountProgress();
    }

    public void SwitchProductionType()
    {
        if (isSpawner)
            return;

        if (CanvasManager.Instance.productionType != ProductionType.None)
            productionType = CanvasManager.Instance.productionType;

        RecountProgress();
    }

    public void RecountProgress()
    {
        int newPrice = PlayerPrefs.GetInt(productionType.ToString() + "Price");
        float progress = cost * percent / newPrice;

        cost = newPrice;
        costPart = cost / increasePercentage;

        percent = progress;
        stageIndicator.SetPercent(percent);
    }

    private IEnumerator AddIncreasePercentage()
    {
        while (true)
        {
            yield return new WaitForSeconds(repeatRate / UnitControllerFactory.Efficiency);

            //if (xUnit != null && !xUnit.cellController.Equals(UnitController.cellController))
            //    xUnit = null;

            if (UnitControllerFactory.IsOn() // ???? ??????? 
                &&
                FacroryFree
                //(xUnit == null //???? ??? ?????? ?? ?????????????
                 //   || 
                //    !UnitController.CellControllerBase.Equals(xUnit.CellControllerBase))
                )
            {
                IBuildable iBuild = UnitControllerFactory as IBuildable;
                while (iBuild.IsBuilding)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                if (MoneyManager.Instance(UnitControllerFactory.PlayerNo).WithdrawMoney(costPart))
                {
                    if (percent >= 1)
                    {
                        if (unitControllerFactory.PlayerNo == 1)
                        {
                            if (IsFreeAround() && productionType == ProductionType.Helicopter)
                            {
                                IDefaultPosition unitIDefaultPosition = ((IDefaultPosition)UnitControllerFactory);
                                unitIDefaultPosition.DefaultPosition = defaultCellTank;

                                AutoTankSwitch();

                                yield return new WaitForSeconds(repeatRate / UnitControllerFactory.Efficiency);
                            }
                            else if (!IsFreeAround() && productionType == ProductionType.Tank)
                            {
                                IDefaultPosition unitIDefaultPosition = ((IDefaultPosition)UnitControllerFactory);
                                defaultCellTank = unitIDefaultPosition.DefaultPosition;

                                FindNewDefaultCell();

                                AutoHelicopterSwitch();

                                while(percent < 1)
                                {
                                    yield return new WaitForSeconds(repeatRate / UnitControllerFactory.Efficiency);

                                    percent += percentPart;
                                    stageIndicator.SetPercent(percent);
                                }
                            }

                            if (productionType == ProductionType.Helicopter)
                                FindNewDefaultCell();
                        }

                        CreateUnit();
                        percent = 0;

                        costPart = cost / increasePercentage;
                    }
                    else
                    {
                        percent += percentPart;
                    }

                    stageIndicator.SetPercent(percent);
                }
            }
        }
    }

    public void CreateUnit()
    {
        GameObject unit;

        if (productionType == ProductionType.Tank)
            unit = Instantiate(prefabObject, transform.position, transform.rotation , UnitManager.Instance.transform);
        else
            unit = Instantiate(prefabHelicopter, transform.position, transform.rotation, UnitManager.Instance.transform);

        curUnitController = unit.GetComponent<UnitController>();

        curUnitController.SetUnitControllerFactory(unitControllerFactory);
        unitControllerFactory.CellControllerBase.SetFabricChild(curUnitController);

        facroryFree = false;

        StartCoroutine(MoveToDefaultCell());
    }

    private IEnumerator MoveToDefaultCell()
    {
        IMoveToDefaultCell newUnitIMoveToDefaultCell = ((IMoveToDefaultCell)curUnitController);
        IDefaultPosition unitIDefaultPosition = ((IDefaultPosition)UnitControllerFactory);

        CellController defaultPosition = null;
        while (defaultPosition == null)
        {
            defaultPosition = unitIDefaultPosition.DefaultPosition;
            if (defaultPosition != null)
            {
                newUnitIMoveToDefaultCell.MoveToDefaultCell(
                    UnitControllerFactory.CellControllerBase, 
                    unitIDefaultPosition.DefaultPosition,
                    () => { 
                        facroryFree = true;
                        newUnitIMoveToDefaultCell.SetNoAtFactory();
                        curUnitController.gameObject.layer = LayerMask.NameToLayer("MobileUnit");
                    }
                    );
            }
            yield return new WaitForSeconds(0.5f);

        }
    }


    //public List<Vector2Int> GetTargetVariants()
    //{
    //    var pos = UnitControllerFactory.CellControllerBase.position;
    //    foreach(var cell in CellsManager.Instance.cellControllersArr[pos.x, pos.y].nearCells)
    //    {
    //        CellController
    //    }
    //}
    //public void UpdateDefaultPosition()
    //{
    //    if (curUnitController)
    //    {

    //        ((IMoveToDefaultCell)curUnitController).UpdateTargetCell(UnitControllerFactory.CellControllerBase, ((IDefaultPosition)UnitControllerFactory).DefaultPosition);
    //    }
        
    //}


}
