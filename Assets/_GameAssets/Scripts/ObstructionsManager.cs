using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructionsManager : MonoBehaviour
{
    public static ObstructionsManager Instance { private set; get; }

    private static List<UnitController> unitControllers = new List<UnitController>();

    public List<UnitController> rocks = new List<UnitController>();
    public List<UnitController> lakes = new List<UnitController>();

    public List<UnitController> all = new List<UnitController>();

    public List<Vector2Int> rockPositions = new List<Vector2Int>();
    public List<Vector2Int> lakePositions = new List<Vector2Int>();

    public List<Vector2Int> allPositions = new List<Vector2Int>();

    public void Awake()
    {
        Instance = this;

        foreach (var unit in unitControllers)
        {
            AddUnit(unit);
        }
        
        unitControllers.Clear();
    }

    public static void AcyncAddUnitController(UnitController unitController)
    {
        if (Instance != null)
            Instance.AddUnit(unitController);
        else
        {
            unitControllers.Add(unitController);
        }
    }


    private void AddUnit(UnitController controller)
    {

        if (controller.unitType != UnitController.UnitType.Obstruction)
            return;

        if (controller.GetType() == typeof(LakeUnitController))
        {
            lakes.Add(controller);
            lakePositions.Add(controller.CellControllerBase.position);
        }
        else if (controller.GetType() == typeof(RockUnitController))
        {
            rocks.Add(controller);
            rockPositions.Add(controller.CellControllerBase.position);
        }

        all.Add(controller);
        allPositions.Add(controller.CellControllerBase.position);

    }




}
