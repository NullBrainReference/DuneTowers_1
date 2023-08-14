using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveToDefaultCell 
{
    //public CellController DefaultPosition { set; get; }
    public void MoveToDefaultCell(CellController fromCellController, CellController ToCellController, Action action);
    public void SetNoAtFactory();
    //public void UpdateTargetCell(CellController fromCellController, CellController ToCellController);
}

