using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUnitController : UnitController, IMoveToDefaultCell
{

    [Space]
    //public SliderUnit sliderUnit;
    public CannonController cannonController;

    private TankDeviceTargetController tankDeviceTargetController;
    public UnitController unitControllerFactory;

    private void Awake()
    {
        unitStats = GameStats.Instance.GetUnit(Unit.Tank, PlayerNo);
        tankDeviceTargetController = GetComponent<TankDeviceTargetController>();
        InitUnitStats();
    }


    protected override void UpdateSlider()
    {
        //print("curHealth / health = " + (curHealth / health));
        //if (sliderUnit != null)
        //    sliderUnit.SetValue(CurHealthPercent);
    }

    public override void OnLongTouch(CellController cellController, UnitController xUnitController)
    {
        if (!IsPlayer)
            return;

        //SelectManager.Instance.SelectObject(transform);
        print("OnLongTouch " + TransformUrlUtil.GetTransformFullName(transform));
    }

    public override void OnNextLongTouch(CellController cellController)
    {
        if (!IsPlayer)
            return;

        print("OnNextLongTouch " + TransformUrlUtil.GetTransformFullName(transform));
    }

    public override void OnNextShortTouch(CellController cellController)
    {
        CanvasManager.Instance.sellButton.Turn(false, false, 0, null);

        if (!IsPlayer)
            return;

        cellController.Select();
        Unselect();

        UnitMover unitMover = GetComponent<UnitMover>();

        if (cannonController.CheckTarget(cellController.Child)) // если он в целях, то нацеливаем на него
        {
            cannonController.ChangeTarget(true, cellController.Child);
        }
        else // иначе движемся в это место
        {
            //unitMover.TaskMoveToCell( cellController.position);
            unitMover.MoveToCell( cellController.position);
        }
        //SelectManager.Instance.Follow(this, 10f);
        //print("OnNextShortTouch " + TransformUrlUtil.GetTransformFullName(transform));
    }

    public override void SetUnitControllerFactory(UnitController unitController)
    {
        unitControllerFactory = unitController;
    }

    public override UnitController GetUnitControllerFactory()
    {
        return unitControllerFactory;
    }

    public override void ExtraDestroyAction()
    {
        //unitControllerFactory
    }

    public void MoveToDefaultCell(CellController fromCellController, CellController toCellController, Action action)
    {
        
        StartCoroutine(MoveToDefaultCellCoroutine(fromCellController, toCellController, action));

    }

    private IEnumerator MoveToDefaultCellCoroutine(CellController fromCellController, CellController toCellController, Action action)
    {
        bool roadFree = false;
        while (!roadFree)
        {
            roadFree = FindFreeHorizontalOrVerticalCells(fromCellController.position);
            
            //if (roadFree)
            //    yield return null;
            //else
                yield return new WaitForSeconds(0.5f);
        }

        UnitMover unitMover = GetComponent<UnitMover>();
        unitMover.MoveToCell( toCellController.position, action);

    }

    //public void UpdateTargetCell(CellController fromCellController, CellController toCellController)
    //{
    //    UnitMover unitMover = GetComponent<UnitMover>();
    //    //unitMover.target = toCellController.position;
    //    //unitMover.StopAllCoroutines();
    //    unitMover.MoveToCell(fromCellController.position, toCellController.position);

    //}

    private bool FindFreeHorizontalOrVerticalCells(Vector2Int pos)
    {
        if (CellsManager.Instance.InRangeArray(pos.x + 1, pos.y) && CellsManager.Instance.cellControllersArr[pos.x + 1, pos.y].ChildEmpty())
            return true;
        if (CellsManager.Instance.InRangeArray(pos.x - 1, pos.y) && CellsManager.Instance.cellControllersArr[pos.x - 1, pos.y].ChildEmpty())
            return true;        
        if (CellsManager.Instance.InRangeArray(pos.x , pos.y + 1) && CellsManager.Instance.cellControllersArr[pos.x , pos.y + 1].ChildEmpty())
            return true;        
        if (CellsManager.Instance.InRangeArray(pos.x , pos.y - 1) && CellsManager.Instance.cellControllersArr[pos.x , pos.y - 1].ChildEmpty())
            return true;

        return false;
    }

    public override bool IsOn()
    {
        return true; // всегда активен
    }

    public void SetNoAtFactory()
    {
        UnitMover unitMover = GetComponent<UnitMover>();
        unitMover.atFactory = false;
    }


    internal override DeviceController.SituationResult GetSituationResult(List<UnitController> units, List<UnitController> enemyUnits)
    {        
        return tankDeviceTargetController.GetSituationResult(units, enemyUnits);
    }

    internal override void ApplyAction(DeviceController.SituationResult situationResult)
    {
        if (situationResult.situationResultType == DeviceController.SituationResultType.Move)
        {
            UnitMover unitMover = GetComponent<UnitMover>();
            unitMover.MoveToCell(situationResult.position);
        }
        ////else if (situationResult.situationResultType == DeviceController.SituationResultType.NewTarget)
        ////    Debug.LogError("NewTarget");
        ////else if (situationResult.situationResultType == DeviceController.SituationResultType.Stop)
        ////    Debug.LogError("Stop");
        else
            Debug.LogError("New option - " + situationResult.ToString() + "  " + TransformUrlUtil.GetTransformFullName(transform));

    }

}
