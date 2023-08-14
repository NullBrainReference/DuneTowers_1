using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    //[SerializeField]
    //private UnitController child; //TODO repace

    //[SerializeField]
    //private UnitController plate; //TODO repace

    [SerializeField]
    private UnitController building;

    [SerializeField]
    private UnitController ground;

    [SerializeField]
    private UnitController air;

    [SerializeField]
    private float credits;
    [SerializeField]
    private float startCredits;

    public UnitController Child //TODO repace
    {
        get
        {
            //if (child != null)
            //    return child;
            //else if (child != null)
            //    return air;
            //else if (plate != null)
            //    return plate;
            if (air != null)
                return air;
            else if (ground != null)
                return ground;
            else if (building != null)
                return building;

            return null;
        }
        
    }

    //private void SetChilValue(UnitController value, Unit unit)
    //{
    //    child = value;
    //
    //    if (unit == Unit.Plate)
    //    {
    //        plate = value;
    //    } 
    //    else if (unit == Unit.Helicopter)
    //    {
    //        air = value;
    //    }
    //    
    //}

    public float Credits { 
        get { return credits; } 
        set 
        { 
            credits = value;
            if (credits < 0) credits = 0;
        } 
    }

    public float StartCredits { get { return startCredits; } }

    [field: SerializeField]
    public SelectCellController SelectCellController { private set; get; }



    [Space]
    public GameObject selectPrefab;

    [SerializeField]
    private MeshRenderer meshRenderer;

    public Vector2Int position { get; private set; }

    //public GameObject child;

    public List<CellController> nearCells = new List<CellController>();

    public TextMesh textMesh;

    public bool testSetToCell;

    //public void RemovePlate(UnitController unitController)
    //{
    //    if (unitController.unitStats.Unit == Unit.Plate)
    //        plate = null;
    //}

    //public bool CheckPlate()
    //{
    //    if (child != null && plate != null)
    //        return true;
    //
    //    return false;
    //}

    public void InitCell(Vector2Int position, Material material, CellStats cellStats)
    {
        this.position = position;
        meshRenderer.material = material;

        credits = cellStats.credits;

        startCredits = GameStats.Instance.maxCredits;
        if (startCredits <= 0) startCredits = 1000;
    }

    public void AddNearCells(GameObject gameObject)
    {
        nearCells.Add(gameObject.GetComponent<CellController>());
    }

    public bool IsFreeToBuild()
    {
        if (air != null || ground != null)
            return false;

        if (building != null)
            if (building.unitStats.Unit != Unit.Plate)
                return false;

        return true;
    }

    public bool HasBuilding()
    {
        if (building == null)
            return false;

        if (building.unitStats.Unit == Unit.Plate)
            return false;

        return true;
    }

    public bool IsFreeToMoveOver(UnitController unit)
    {
        if (unit.unitLayer == UnitLayer.Ground)
        {
            if (ground != null)
                return false;
            
            if (building == null)
                return true;
            else if (building.isAllowedToMoveOver)
                return true;
            
            //todo error when the second tank arrives (if set the arrival point for the factory)
        }
        else if (unit.unitLayer == UnitLayer.Air)
        {
            if (air == null)
                return true;
        }
    
        return false;
    }

    //public void SetChild(UnitController newUnitController, Unit? unit = null)
    //{
    //    if (unit == null)
    //        unit = newUnitController?.unitStats?.Unit;
    //
    //    if (Child != null && child?.unitStats.Unit == Unit.Plate)
    //    {
    //        plate = child;
    //    }
    //
    //    //if (Child != null && child?.unitStats.Unit == Unit.Fabric)
    //    //{
    //    //    plate = child;
    //    //}
    //    
    //    if (Child == null && unit == Unit.Helicopter)
    //    {
    //        air = newUnitController;
    //        newUnitController.CellControllerBase = this;
    //        return;
    //    }
    //
    //    SetChilValue(newUnitController, (Unit)unit);
    //
    //    if (newUnitController != null)
    //        newUnitController.CellControllerBase = this;
    //}

    //public void RemoveChild(UnitController unitController)
    //{
    //    if (unitController.unitStats.Unit == Unit.Plate)
    //    {
    //        plate = null;
    //        if (child?.unitStats.Unit == Unit.Plate)
    //            child = null;
    //    }
    //    else if (unitController.unitStats.Unit == Unit.Helicopter)
    //    {
    //        air = null;
    //        if (child?.unitStats.Unit == Unit.Helicopter)
    //            child = null;
    //    }
    //    else
    //    {
    //        child = null;
    //    }
    //}

    public void SetTerrainObject(UnitController unitController)
    {
        building = unitController;
        ground = unitController;

        unitController.CellControllerBase = this;
    }

    public UnitController GetChild(UnitLayer unitLayer)
    {
        switch (unitLayer)
        {
            case UnitLayer.Air:
                return air;
            case UnitLayer.Ground:
                return ground;
            case UnitLayer.Building:
                return building;
        }

        return null;
    }

    public void SetFabricChild(UnitController unitController)
    {
        if (unitController.unitStats.Unit == Unit.Tank)
            ground = unitController;
        else if (unitController.unitStats.Unit == Unit.Helicopter)
            air = unitController;
        unitController.CellControllerBase = this;
    }

    public void SetChild2(UnitController unitController)
    {
        switch (unitController.unitLayer)
        {
            case UnitLayer.Building:
                building = unitController;
                break;
            case UnitLayer.Ground:
                ground = unitController;
                break;
            case UnitLayer.Air:
                air = unitController;
                break;
        }

        unitController.CellControllerBase = this;
    }

    public void RemoveChild2(UnitController unitController)
    {
        switch (unitController.unitLayer)
        {
            case UnitLayer.Building:
                building = null;
                break;
            case UnitLayer.Ground:
                ground = null;
                break;
            case UnitLayer.Air:
                air = null;
                break;
        }
    }

    //public void ClearPlate()
    //{
    //    plate = null;
    //}

    public void SetSelect(SelectCellController selectCellController)
    {
        if (SelectCellController == null || selectCellController == null)
            this.SelectCellController = selectCellController;

        if (selectCellController != null)
            selectCellController.CellControllerBase = this;
    }

    public UnitController SelectController()
    {
        if (air != null)
            return air;
        else if (ground != null)
            return ground;
        else if (building != null)
            return building;

        return null;
    }

    public void Select()
    {
        Vector3 pos = transform.position;
        pos.z = -9;
        Instantiate(selectPrefab, pos, transform.rotation, transform);
    }

    // Update is called once per frame
    private void Update()
    {
        if (testSetToCell)
        {
            testSetToCell = false;

            Child.transform.localPosition = transform.position;
        }

    }

    public bool Equals(CellController other)
    {
        return (other.position.x == position.x) && (other.position.y == position.y);
    }

    public bool ChildForBuild(int PlayerNo)
    {
        if (Child != null && (Child.unitType == UnitController.UnitType.Stationary || Child.unitType == UnitController.UnitType.Foundation) && Child.PlayerNo == PlayerNo)
            return true;
        else if (ground != null && (ground.unitType == UnitController.UnitType.Stationary || ground.unitType == UnitController.UnitType.Foundation) && ground.PlayerNo == PlayerNo)
            return true;

        return false;
        //return Child != null && (Child.unitType == UnitController.UnitType.Stationary || Child.unitType == UnitController.UnitType.Foundation) && Child.PlayerNo == PlayerNo;
    }

    public bool PlayerStationaryUnit(int PlayerNo)
    {
        return Child != null && Child.PlayerNo == PlayerNo && Child.unitType == UnitController.UnitType.Stationary;
    }

    public bool IsObstructionUnit()
    {
        return Child != null && Child.unitType == UnitController.UnitType.Obstruction;
    }

    public bool ChildEmpty(Unit? unit = null) {
        bool empty = unit != null ? unit == Unit.Helicopter && Child?.GetType() == typeof(LakeUnitController) : false;
        return Child == null || Child.GetType() == typeof(FoundationSlabUnitController) || empty;
    }

    public bool ChildEmpty2()
    {
        bool empty = IsFreeToBuild();

        return empty;
    }

    public override string ToString()
    {
        return $"{position}, {Child.PlayerNo}: {Child?.name}";
    }

}
