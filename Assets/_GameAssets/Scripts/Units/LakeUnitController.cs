using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeUnitController : UnitController
{
    [SerializeField]
    private GameObject sprite;

    protected override void UpdateSlider()
    {
        
    }

    protected override void StartUpdate()
    {
        sprite.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
    }

    public override void OnShortTouch(CellController cellController, UnitController xUnitController)
    {
        CanvasManager.Instance.SwitchFabricPanel(false);
        return;
    }

    public override void OnLongTouch(CellController cellController, UnitController xUnitController)
    {
        return;
    }

    internal override DeviceController.SituationResult GetSituationResult(List<UnitController> units, List<UnitController> enemyUnits)
    {
        return null; // base.GetSituationResult(units, enemyUnits);
    }
    internal override void ApplyAction(DeviceController.SituationResult situationResult)
    {
        //base.ApplyAction(situationResult);
    }
}
