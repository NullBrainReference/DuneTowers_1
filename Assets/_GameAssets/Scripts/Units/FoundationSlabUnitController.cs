using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoundationSlabUnitController : UnitController
{

    private void Awake()
    {
        unitStats = GameStats.Instance.GetUnit(Unit.Plate, PlayerNo);
        InitUnitStats();
        //UnitsCollector.Instance(PlayerNo).AddUnit(this);
    }

    public override void OnLongTouch(CellController cellController, UnitController xUnitController)
    {

    }

    protected override void UpdateSlider()
    {
        
    }

    internal override DeviceController.SituationResult GetSituationResult(List<UnitController> units, List<UnitController> enemyUnits)
    {
        return null;// base.GetSituationResult(units, enemyUnits);
    }

    internal override void ApplyAction(DeviceController.SituationResult situationResult)
    {
        MapManager.CreateUnit(situationResult.prefab, situationResult.position);
    }

}
