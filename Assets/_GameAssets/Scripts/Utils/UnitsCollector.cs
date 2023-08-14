using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UnitsCollector : MonoBehaviour
{
    public List<UnitController> plates = new List<UnitController>();
    public List<UnitController> factories = new List<UnitController>();
    public List<UnitController> towers = new List<UnitController>();
    public List<UnitController> tankers = new List<UnitController>();
    public List<UnitController> bases = new List<UnitController>();
    public List<UnitController> other = new List<UnitController>();
    public List<UnitController> tanks = new List<UnitController>();
    public List<UnitController> helicopters = new List<UnitController>();

    public UnitsCollector mainEnemyUnitsCollector;

    [SerializeField] private int playerNo;

    private static Dictionary<int, UnitsCollector> instances = new Dictionary<int, UnitsCollector>();


    public static List<int> players = new List<int>();
    public static UnitsCollector Instance(int playerNo) => instances.ContainsKey(playerNo)? instances[playerNo] : null;

    public List<UnitController> GetAllUnits()
    {
        var result = new List<UnitController>();
        result.AddRange(plates);
        result.AddRange(factories);
        result.AddRange(towers);
        result.AddRange(tankers);
        result.AddRange(bases);
        result.AddRange(tanks);
        result.AddRange(other);
        return result;
    }

    public List<UnitController> GetMobileUnits()
    {
        return tanks;
    }

    public List<UnitController> GetStaticUnits()
    {
        var result = new List<UnitController>();

        result.AddRange(plates);
        result.AddRange(factories);
        result.AddRange(towers);
        result.AddRange(tankers);
        result.AddRange(bases);
        result.AddRange(other);

        return result;
    }

    public List<Vector2Int> GetAllUnitObstructionPositions()
    {
        var unitControllers = new List<UnitController>();
        unitControllers.AddRange(factories);
        unitControllers.AddRange(towers);
        unitControllers.AddRange(tankers);
        unitControllers.AddRange(bases);
        unitControllers.AddRange(tanks);
        unitControllers.AddRange(other);

        var result = new List<Vector2Int>();
        foreach (var item in unitControllers)
            if (item.CellControllerBase != null)
            result.Add(item.CellControllerBase.position);
        return result;
    }

    public List<Vector2Int> GetAirObstructionPositions()
    {
        var unitControllers = new List<UnitController>();
        unitControllers.AddRange(helicopters);

        var result = new List<Vector2Int>();
        foreach (var item in unitControllers)
            if (item.CellControllerBase != null)
                result.Add(item.CellControllerBase.position);
        return result;
    }

    private static Dictionary<int, List<UnitController>> unitControllers = new Dictionary<int, List<UnitController>>();



    public void Awake()
    {
        //plates = new List<UnitController>();
        //factories = new List<UnitController>();
        //towers = new List<UnitController>();
        //tankers = new List<UnitController>();

        if (instances.ContainsKey(playerNo))
            instances.Remove(playerNo);

        instances.Add(playerNo, this);
        players.Add(playerNo);

        if (unitControllers.ContainsKey(playerNo))
        {
            foreach(var unit in unitControllers[playerNo])
            {
                AddUnit(unit);
            }
        }
        unitControllers.Remove(playerNo);
    }

    public static void AcyncAddUnitController(int playerNo, UnitController unitController)
    {
        if (GameStats.Instance.isTowerDefence)
        {
            if (unitController.unitStats.Unit == Unit.Fabric && unitController.PlayerNo == 1)
            {
                MapManager.Instance.spawnersManager.AddSpawner(unitController as Spawner);

                return;
            }
        }

        if (Instance(playerNo) != null)
        {
            Instance(playerNo).AddUnit(unitController);
        }
        else
        {
            if (!unitControllers.ContainsKey(playerNo))
                unitControllers.Add(playerNo, new List<UnitController>());
            unitControllers[playerNo].Add(unitController);
        }
    }

    public void AddUnit(UnitController controller)
    {
        Unit unit = controller.unitStats.Unit;

        switch (unit)
        {
            case Unit.Plate:
                plates.Add(controller);
                break;
            case Unit.Fabric:
                factories.Add(controller);
                break;
            case Unit.Tower:
                towers.Add(controller);
                break;
            case Unit.Drill:
                tankers.Add(controller);
                break;
            case Unit.Base:
                bases.Add(controller);
                break;
            case Unit.Tank:
                tanks.Add(controller);
                break;
            case Unit.Helicopter:
                helicopters.Add(controller);
                break;
            case Unit.None:
                other.Add(controller);
                break;


        }
    }



    public void RemoveUnit(UnitController controller)
    {
        Unit unit = controller.unitStats.Unit;

        switch (unit)
        {
            case Unit.Plate:
                plates.Remove(controller);
                break;
            case Unit.Fabric:
                factories.Remove(controller);
                break;
            case Unit.Tower:
                towers.Remove(controller);
                break;
            case Unit.Drill:
                tankers.Remove(controller);
                break;
            case Unit.Base:
                bases.Remove(controller);
                break;
            case Unit.Tank:
                tanks.Remove(controller);
                break;
            case Unit.None:
                other.Remove(controller);
                break;
        }
    }

    public int GetCount(Unit unit)
    {
        switch (unit)
        {
            case Unit.Plate:
                return 0;
            case Unit.Fabric:
                return factories.Count;
            case Unit.Tower:
                return 0;
            case Unit.Drill:
                return 0;//tankers.Count;
        }

        return 0;
    }

    private void OnDestroy()
    {
        instances.Clear();
        players.Clear();
    }
}
