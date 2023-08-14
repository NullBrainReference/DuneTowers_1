using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUnitsIniter : MonoBehaviour
{
    public UnitController[] startUnitsControllers;

    private void Start()
    {
        startUnitsControllers = this.GetComponentsInChildren<UnitController>();

        foreach(UnitController cellController in startUnitsControllers)
        {
            if(cellController.unitStats.Unit == Unit.None) continue;
            if (cellController.IsPlayer)
                cellController.unitStats = GameStats.Instance.player.GetUnit(cellController.unitStats.Unit);

            else if (!cellController.IsPlayer)
                cellController.unitStats = GameStats.Instance.level.GetUnit(cellController.unitStats.Unit);

            cellController.InitUnitStats();
        }
    }
}
