using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellStats
{
    public MapCellType cellType = MapCellType.None;
    public int credits = 2000;

    public HQ_place_status hq = HQ_place_status.None;
    public Unit unit = Unit.None;
    public int playerNo;

    public int x;
    public int y;
}
