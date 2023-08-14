//using System;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeviceHeadquartersController : MonoBehaviour
{

    private class Distance
    {
        public float sqrDistance;
        public CellController cellController;
        //public float factor = 1;
        //internal bool block = false;

        public Distance(CellController cellController, Vector2Int headPos, float factor)
        {
            this.cellController = cellController;
            sqrDistance = Vector2.SqrMagnitude(cellController.position - headPos) * factor;
        }


        public float GetSignificance(Unit unit)
        {
            if (unit == Unit.Drill)
            {
                //Debug.Log("sqrDistance * Credits = " + sqrDistance * cellController.Credits);
                //return cellController.Credits;
                return cellController.Credits * cellController.Credits / sqrDistance;
            }

            return sqrDistance;
        }

        public override string ToString()
        {
            return $"{cellController.position} - {sqrDistance}";
        }
    }

    private class ItemUnitType
    {
        public float percent { set; get; }
        public Unit unitType { set; get; }
        public ItemUnitType(float percent, Unit unitType)
        {
            this.percent = percent;
            this.unitType = unitType;
        }
        public override string ToString()
        {
            return $"{percent} - {unitType}";
        }
    }

    [System.Serializable]
    public class RangeUnit
    {
        public int min = 1;
        public int max = 5;
        public int Clamp(int value)
        {
            return Mathf.Clamp(value, min, max);
        }
    }

    //[SerializeField]
    //private float percent = 0.3f;

    [SerializeField]
    private float percentCannons = 0.3f;


    [Space]
    [SerializeField]
    private RangeUnit rangeFactories;

    [SerializeField][Range(0f,1f)]
    private float percentFactories = 0.2f;


    [Space]
    [SerializeField]
    private RangeUnit rangeTankers;

    [SerializeField][Range(0f,1f)]
    private float percentTankers = 0.3f;


    [Space]
    [SerializeField][Range(0f, 1f)]
    private float percentFoundation=0.2f;

    private HeadquartersUnitController headquartersUnitController;
    [SerializeField] private GameObject prefabCannon;
    [SerializeField] private GameObject prefabFactory;
    [SerializeField] private GameObject prefabTanker;
    [SerializeField] private GameObject prefabFoundation;

    //private enum UnitType { Tower, Fabric, Drill, Plate }

    [SerializeField]
    private List<UnitController> enemyUnits;

    private void Awake()
    {
        headquartersUnitController = GetComponent<HeadquartersUnitController>();
    }

    internal DeviceController.SituationResult GetSituationResult(List<UnitController> units, List<UnitController> enemyUnits)
    {
        List<UnitController> tankerUnitControllers = units.FindAll(x => x.GetType().Equals(typeof(TankerUnitController)));
        List<UnitController> tankFactoryUnitControllers = units.FindAll(x => x.GetType().Equals(typeof(TankFactoryUnitController)));

        this.enemyUnits = enemyUnits;

        DeviceController.SituationResult result = default; 

        if (tankerUnitControllers.Count < rangeTankers.min || tankFactoryUnitControllers.Count < rangeFactories.min)
        {

            if (tankerUnitControllers.Count < rangeTankers.min)
                result = GetSituationResult(1, Unit.Drill, true, tankFactoryUnitControllers, tankerUnitControllers);

            if (result == null && tankFactoryUnitControllers.Count < rangeFactories.min)
                result = GetSituationResult(1, Unit.Fabric, true, tankFactoryUnitControllers, tankerUnitControllers);

        }
        else
        {

            List<UnitController> cannonControllers = units.FindAll(x => x.GetType().Equals(typeof(CannonUnitController)));
            List<UnitController> foundationControllers = units.FindAll(x => x.GetType().Equals(typeof(FoundationSlabUnitController)));

            float totalUnits =
                cannonControllers.Count +
                (tankFactoryUnitControllers.Count < rangeTankers.max ? tankFactoryUnitControllers.Count : 0) +
                (tankerUnitControllers.Count < rangeTankers.max ? tankerUnitControllers.Count : 0) +
                foundationControllers.Count;


            var list = new List<ItemUnitType>();


            if (tankerUnitControllers.Count < rangeTankers.max)
                list.Add(new ItemUnitType(percentTankers - (float)tankerUnitControllers.Count / (float)totalUnits, Unit.Drill));

            if (tankFactoryUnitControllers.Count < rangeFactories.max)
                list.Add(new ItemUnitType(percentFactories - (float)tankFactoryUnitControllers.Count / (float)totalUnits, Unit.Fabric));

            list.Add(new ItemUnitType(percentCannons - (float)cannonControllers.Count / (float)totalUnits, Unit.Tower));
            list.Add(new ItemUnitType(percentFoundation - (float)foundationControllers.Count / (float)totalUnits, Unit.Plate));


            list.Sort((x, y) => y.percent.CompareTo(x.percent));

            var unit = list[0];
            result = GetSituationResult(unit.percent, unit.unitType, false, tankFactoryUnitControllers, tankerUnitControllers);
            if (result == null)
                result = GetSituationResult(unit.percent, Unit.Tower, false, tankFactoryUnitControllers, tankerUnitControllers);
        }



        if (!CheckPrice(ref result))
            return default;

        return result;


        //List<UnitController> enemyCannonControllers = units.FindAll(x => x.GetType().Equals(typeof(CannonController)));
        //List<UnitController> enemyTankFactoryUnitControllers = units.FindAll(x => x.GetType().Equals(typeof(TankFactoryUnitController)));
        //List<UnitController> enemyTankerUnitController = units.FindAll(x => x.GetType().Equals(typeof(TankerUnitController)));

        //return new DeviceController.SituationResult();
    }

    private bool CheckPrice(ref DeviceController.SituationResult result)
    {
        UnitStats unitStats = GameStats.Instance.level.GetUnitStats(result.unit);
        result.battlePrice = unitStats.GetPrice(UnitsCollector.Instance(1));

        return MoneyManager.Instance(headquartersUnitController.PlayerNo).Money >= unitStats.GetPrice(UnitsCollector.Instance(1));

        
        //print($"Add CheckPrice for {result.unit}");
        //return true;
    }

    private DeviceController.SituationResult GetSituationResult(float significance, Unit unit, bool force, List<UnitController> factoryUnitControllers, List<UnitController> tankerUnitControllers)
    {
        
        DeviceController.SituationResult situationResult = new DeviceController.SituationResult();
        Vector2Int? pos = GetPosition(unit, factoryUnitControllers, tankerUnitControllers);
        situationResult.unit = unit;

        if (pos == null)
            return default;

        situationResult.position = (Vector2Int)pos;

        situationResult.situationResultType = DeviceController.SituationResultType.Build;
        situationResult.unitController = headquartersUnitController;



        situationResult.significance = significance;

        situationResult.force = force;

        if (unit == Unit.Tower)
            situationResult.prefab = prefabCannon;
        else if (unit == Unit.Fabric)
            situationResult.prefab = prefabFactory;
        else if (unit == Unit.Drill)
            situationResult.prefab = prefabTanker;
        else if (unit == Unit.Plate)
            situationResult.prefab = prefabFoundation;
        
        situationResult.paramDict.Add("otherWays", GetFactoryWays(factoryUnitControllers));

        return situationResult;
    }


    private List<Vector2Int> GetFactoryWays(List<UnitController> factoryUnitControllers)
    {
        
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var tankFactoryUnitController in factoryUnitControllers)
            result.AddRange(((TankFactoryUnitController)tankFactoryUnitController).Way);

        return result.Distinct().ToList();
    }

    /// <summary>
    /// Find place for build
    /// </summary>
    /// <param name="unitType"></param>
    /// <param name="factoryUnitControllers"></param>
    /// <returns></returns>
    private Vector2Int? GetPosition(Unit unitType, List<UnitController> factoryUnitControllers, List<UnitController> tankerUnitController)
    {

        List<CellController> cellsAll = headquartersUnitController.GetCellControllersForSelectPlaces();
        List<CellController> cells = new List<CellController>(cellsAll);

        //elimination factory way
        if (unitType != Unit.Plate)
            foreach(var wayItem in GetFactoryWays(factoryUnitControllers))
            {
                CellController cell = cells.FindLast(x => x.position.Equals(wayItem));
                cells.Remove(cell);
            }


        //--------------------------------------------
        if (unitType == Unit.Tower)
        {
            List<UnitController> unitControllers = new List<UnitController>();
            unitControllers.AddRange(tankerUnitController);
            unitControllers.AddRange(factoryUnitControllers);

            List<Distance> cellsForBuildTotal = new List<Distance>();

            foreach(var unit in unitControllers)
            {
                float factor = unit.GetType() == typeof(FactoryUnitCreator) ? 0.3f : 1;
                var cellsForBuild = GetCellsForBuild(cells, unit.CellControllerBase.position, factor);
                cellsForBuildTotal.AddRange(cellsForBuild);
            }

            cellsForBuildTotal.Sort((x, y) => x.GetSignificance(unitType).CompareTo(y.GetSignificance(unitType))); //min


            int variant = Random.Range(0, (int)((cellsForBuildTotal.Count-1)/3f));

            return cellsForBuildTotal[variant].cellController.position;
        }

        //--------------------------------------------
        else if (unitType == Unit.Fabric)
        {

            var cellsForBuild = GetCellsForBuild(cells, headquartersUnitController.CellControllerBase.position, 1);
            cellsForBuild.Sort((x, y) => x.GetSignificance(unitType).CompareTo(y.GetSignificance(unitType))); //min

            int variant = Random.Range(0, (int)((cellsForBuild.Count - 1) / 3f));

            return cellsForBuild[variant].cellController.position;

        }

        //--------------------------------------------
        else if (unitType == Unit.Drill)
        {

            //var cellsForBuild = GetCellsForBuild(cells, headquartersUnitController.CellControllerBase.position, 1);
            var cellsForBuild = GetDistances(cells, headquartersUnitController.CellControllerBase.position, 1);
            cellsForBuild.Sort((x, y) => y.GetSignificance(unitType).CompareTo(x.GetSignificance(unitType))); //min

            int variant = Random.Range(0, (int)((cellsForBuild.Count - 1) / 3f));

            int variantsLimit = 2;

            if (variant > variantsLimit) 
                variant = variantsLimit;

            return cellsForBuild[variant].cellController.position;

        }


        //--------------------------------------------
        else if ( unitType == Unit.Plate)
        {
            List<Distance> cellsForBuild = new List<Distance>();

            var distances = GetDistances(cells, headquartersUnitController.CellControllerBase.position, 1);
            float maxSignificance = distances.Max(x => x.GetSignificance(unitType)) ;
            

            foreach (var tankFactoryUnitController in factoryUnitControllers)
            {
                foreach (var wayItem in ((TankFactoryUnitController)tankFactoryUnitController).Way)
                {
                    try
                    {
                        CellController cell = cellsAll.FindLast(x => x.position.Equals(wayItem));
                        if (cell != null)
                        {
                            var distance = new Distance(cell, headquartersUnitController.CellControllerBase.position, 1);
                            if (distance.GetSignificance(unitType) < maxSignificance/1.5f)
                                cellsForBuild.Add(distance);
                        }
                    }
                    catch(System.Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }

            //var cellsForBuild = GetCellsForBuild(cells, headquartersUnitController.CellControllerBase.position, 1);
            if (cellsForBuild.Count == 0)
                return null;

            cellsForBuild.Sort((x, y) => x.GetSignificance(unitType).CompareTo(y.GetSignificance(unitType))); //min

            int variant = Random.Range(0, (int)((cellsForBuild.Count - 1) / 3f));
            return cellsForBuild[variant].cellController.position;

        }

        return default;
    }


    private List<Distance> GetDistances(List<CellController> cells, Vector2Int pos, float factor)
    {
        List<Distance> distances = new List<Distance>();
        float maxDistance = 0;
        foreach (CellController cell in cells)
        {
            Distance headDistance = new Distance(cell, pos, factor);
            if (maxDistance < headDistance.sqrDistance)
                maxDistance = headDistance.sqrDistance;
            distances.Add(headDistance);
        }

        return distances;
    }

    private List<Distance> GetCellsForBuild(List<CellController> cells, Vector2Int pos, float factor)
    {
        var distances = GetDistances(cells, pos, factor);
        float part = distances.Max(x => x.sqrDistance) / 3;
        float front = part * 2;
        float rear = part;

        var result = distances.FindAll(x => x.sqrDistance <= rear);

        if (result.Count == 0)
            result = distances.FindAll(x => x.sqrDistance >= rear && x.sqrDistance <= front);

        if (result.Count == 0)
            result = distances.FindAll(x => x.sqrDistance >= front);

        return result;
    }



    //private List<Vector2Int> GetPlayerStationaryPositions()
    //{
    //    List<CellController> playerStationaryCells = CellsManager.Instance.cellControllers.FindAll(x => x.PlayerStationaryUnit(headquartersUnitController.PlayerNo));

    //    List<Vector2Int> playerStationaryPositions = new List<Vector2Int>();
    //    foreach (CellController cellController in playerStationaryCells)
    //        playerStationaryPositions.Add(cellController.position);
    //    return playerStationaryPositions;
    //}

    //private List<Distance> CheckFactoryWay(Unit unitType, List<Distance> cellsForBuild, List<UnitController> tankFactoryUnitControllers, List<Vector2Int> playerStationaryPositions)
    //{
    //    return cellsForBuild;

    //    //List<Distance> result = new List<Distance>(cellsForBuild);

    //    //if (unitType == UnitType.Factory)
    //    //{
    //    //    var targetUnits = enemyUnits.FindAll(x => x.GetType().Equals(typeof(HeadquartersUnitController)));
    //    //    var targetUnitsCount = (targetUnits.Count < 2 ? targetUnits.Count : 2);

    //    //    foreach (var cellForBuild in cellsForBuild)
    //    //    {
    //    //        foreach (var unitController in tankFactoryUnitControllers)
    //    //        {
    //    //            for (int i = 0; i < targetUnitsCount; i++)
    //    //            {
    //    //                Vector2Int target = targetUnits[i].CellControllerBase.position;
    //    //                Vector2Int startPos = unitController.CellControllerBase.position;

    //    //                if (!CheckNextStep(target, startPos, cellForBuild.cellController.position, playerStationaryPositions))
    //    //                    result.Remove(cellForBuild);

    //    //            }
    //    //        }
    //    //    }
    //    //}
    //    //else
    //    //{

    //    //}

    //    //return result;

    //}

    //private bool CheckNextStep(Vector2Int target, Vector2Int startPos, Vector2Int cellForBuildPos, List<Vector2Int> blocksBase)
    //{
    //    //return true;
    //    CourseSearcher courseSearcher = new CourseSearcher(MapManager.Instance.fieldSize);

    //    List<Vector2Int> blocks = new List<Vector2Int>(blocksBase);

    //    blocks.Add(cellForBuildPos);

    //    Vector2Int nextPos = courseSearcher.GetNextStep(startPos, target, blocks);

    //    return startPos != nextPos;
    //}

}
