using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : TouchControllerBase
{

    public UnitController xUnitController;
    protected override bool GameActive()
    {
        return true;
    }

    protected override void OnTouch(TypeTouch typeTouch, GameObject hitObject)
    {
        //SelectManager.Instance.UnselectObject();

        CellController cellController = GetCellController(hitObject);

        //Debug.Log("Entered_in_Touch");

        if (cellController)
        {
            //Debug.Log("Entered_in_CellController");
            //SelectManager.Instance.SelectObject(cellController.transform);

            // Если это юнит
            UnitController unitController = cellController.Child;

            if (typeTouch != TypeTouch.LongTouch)
            {
                if (xUnitController?.PlayerNo == unitController?.PlayerNo && unitController?.unitType != UnitController.UnitType.Foundation) // если объект свой 
                {
                    xUnitController?.Unselect();
                    xUnitController = null;
                }
            }


            SelectCellController selectCellController = cellController.SelectCellController;
            if (selectCellController)
            {
                //Debug.Log("Entered_in_if_sellectController");
                if (unitController == null)
                {
                    selectCellController.OnShortTouch(cellController, xUnitController);
                    //print("SelectCellController_null");
                }
                else if (selectCellController.unitStats.Unit == Unit.Plate)
                {
                    if (unitController.unitStats.Unit == Unit.Plate)
                    selectCellController.OnShortTouch(cellController, xUnitController);
                    //print("SelectCellController_Plate");
                }
                else if (unitController.unitStats.Unit == Unit.Plate)
                {
                    selectCellController.OnShortTouch(cellController, xUnitController);
                    //print("SelectCellController_Plate_2");
                }
            }
            //if (selectCellController && unitController == null)
            //{
            //    selectCellController.OnShortTouch(cellController, xUnitController);
            //    print("SelectCellController");
            //}



            if (xUnitController != null && !xUnitController.Equals(unitController) &&
                (
                    xUnitController?.GetType() == typeof(TankUnitController) 
                    || xUnitController?.GetType() == typeof(HelicopterUnitController) 
                    || xUnitController?.GetType() == typeof(CannonUnitController)
                    || xUnitController?.GetType() == typeof(TankFactoryUnitController)
                ))
            {
                //если инициализировано движение юнитом (танком)
                //if (xUnitController != null && unitController != null && !unitController.Equals(xUnitController) && xUnitController?.GetType() == typeof(TankUnitController))
                //{
                //print("xTouch");
                if (xUnitController == null) 
                    return;

                    if (typeTouch == TypeTouch.ShortTouch)
                        xUnitController.OnNextShortTouch(cellController);
                    else if (typeTouch == TypeTouch.LongTouch)
                        xUnitController.OnNextLongTouch(cellController);

                
                    //SelectManager.Instance.UnselectObject(1f);

                    xUnitController = null;
                
                //}
            }
            else
            {

                if (unitController)
                {
                    //Debug.Log("Entered_in_if_unitController_ShortTouch");
                    //print(unitController.name + " 222   " + typeTouch + ": " + hitObject.name);
                    if (typeTouch == TypeTouch.ShortTouch)
                        unitController.OnShortTouch(cellController, xUnitController);
                    else if (typeTouch == TypeTouch.LongTouch)
                        unitController.OnLongTouch(cellController, xUnitController);

                    xUnitController = unitController;
                    //SelectManager.Instance.UnselectObject(10f);
                }
                else
                {
                    //SelectManager.Instance.UnselectObject(1f);
                    xUnitController = null;
                }
                //Другие типы в рамках CellController
                //ObjController objController = cellController.child2;
                //..............

            }
            
        }

        

        //Если это xxxController
        //xxxController xxxController = GetComponent<xxxController>();
        //..............

        //print(typeTouch +  ": " + hitObject.name);
    }

    private CellController GetCellController(GameObject hitObject)
    {
        CellController result = default;
        UnitMover unitMover = hitObject.GetComponent<UnitMover>();

        if (unitMover)
        {
            //Если это UnitMover
            result = unitMover.UnitController.CellControllerBase;
        }
        else
        {
            //Если это CellController
            result = hitObject.GetComponent<CellController>();
        }

        return result;
    }


}
