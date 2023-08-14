using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FactoryUnitCreator))]
public class TankFactoryUnitController : UnitController, IDefaultPosition, IBuildable
{
    [Space]
    public SliderUnit sliderUnit;

    public FactoryUnitCreator unitCreator;

    public KnobUnitController knobUnitController;

    [SerializeField] private SliderUnit buildSlider;
    public SliderUnit BuildSlider { get { return buildSlider; } }

    [SerializeField] private float buildPoints;
    public float BuildPoints { get { return buildPoints; } set { buildPoints = value; } }

    private float buildProgress = 0;
    public float BuildProgress { get { return buildProgress; } set { buildProgress = value; } }

    public bool IsBuilding { get; set; } = false;

    //private FactoryUnitCreator tankFactoryUnitCreator;

    [SerializeField]
    private CellController defaultPosition;

    [field: SerializeField]
    public List<Vector2Int> Way { private set; get; }

    private void Awake()
    {
        unitStats = GameStats.Instance.GetUnit(Unit.Fabric, PlayerNo);
        InitUnitStats();
    }

    private void FixedUpdate()
    {
        if (buildSlider == null)
            return;

        if (IsBuilding)
        {
            Build();
        }
    }

    public CellController DefaultPosition
    {
        get
        {
            if (defaultPosition == null)
            {
                List<CellController> nearCells = CellControllerBase.nearCells.FindAll(n => n.ChildEmpty2());

                if (nearCells.Count != 0)
                    return nearCells[0];
                else
                    return default(CellController);

            }
            return defaultPosition;
        }
        set
        {
            if (value == CellControllerBase)
                defaultPosition = null;
            else
                defaultPosition = value;
            //FactoryUnitCreator factoryUnitCreator = GetComponent<FactoryUnitCreator>();
            //factoryUnitCreator.UpdateDefaultPosition();
        }
    }
    //private void Start()
    //{
    //    tankFactoryUnitCreator = GetComponent<FactoryUnitCreator>();
    //}

    protected override void UpdateSlider()
    {
        //print("curHealth / health = " + (curHealth / health));
        sliderUnit.SetValue(CurHealthPercent);
    }



    public override void OnLongTouch(CellController cellController, UnitController xUnitController)
    {
        if (!selectObj) 
            return;

        if (PlayerNo == 1)
            return;

        UnitController unitController2 = selectObj.transform.parent.GetComponent<UnitController>();

        if (this == unitController2) {
            //SelectManager.Instance.SelectObject(transform);
            knobUnitController.UpdateStatusUnitActive();
        }
    }

    public override void OnNextLongTouch(CellController cellController)
    {

        //knobUnitController.UpdateStatusUnitActive();
        print("OnNextLongTouch " + TransformUrlUtil.GetTransformFullName(transform));
    }

    public override void OnNextShortTouch(CellController cellController)
    {
        CanvasManager.Instance.sellButton.Turn(false, false, 0, null);
        CanvasManager.Instance.SwitchFabricPanel(false);

        if (!IsPlayer)
            return;

        cellController.Select();
        Unselect();
        DefaultPosition = cellController;

        if (unitCreator.CurUnitController == null) 
            return;

        var unitMover = unitCreator.CurUnitController.GetComponent<UnitMover>();
        //if (unitMover?.atFactory == true) 
        if (unitMover?.State == UnitMover.States.AtFactory) 
            unitMover.MoveToCell(cellController.position);
        //print("OnNextShortTouch " + TransformUrlUtil.GetTransformFullName(transform));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool IsOn()
    {
        return knobUnitController.isOn;
    }

    internal override DeviceController.SituationResult GetSituationResult(List<UnitController> units, List<UnitController> enemyUnits)
    {
        return null;// base.GetSituationResult(units, enemyUnits);
    }

    internal override void ApplyAction(DeviceController.SituationResult situationResult)
    {

        //else
        if (situationResult.situationResultType == DeviceController.SituationResultType.NewTarget)
            Debug.LogError("NewTarget");
        //else if (situationResult.situationResultType == DeviceController.SituationResultType.Stop)
        //    Debug.LogError("Stop");
        else
            Debug.LogError("New option - " + situationResult.ToString() + "  " + TransformUrlUtil.GetTransformFullName(transform));

    }

    

    public List<Vector3Int> FillWay(Dictionary<string, object> paramDict, Vector2Int endPos)
    {

        CourseSearcher courseSearcher = new CourseSearcher(
            MapManager.Instance.fieldSize,
            CellControllerBase.position,
            endPos,
            GetObstructionPositions(),
            false,
            (List<Vector2Int>?)paramDict["otherWays"]);


        Way = courseSearcher.GetWay();
        return courseSearcher.MasToList();
    }

    private List<Vector2Int> GetObstructionPositions()
    {
        List<CellController> playerStationaryCells = CellsManager.Instance.cellControllers.FindAll(x => x.PlayerStationaryUnit(PlayerNo) || x.IsObstructionUnit());

        List<Vector2Int> playerStationaryPositions = new List<Vector2Int>();
        foreach (CellController cellController in playerStationaryCells)
            playerStationaryPositions.Add(cellController.position);
        return playerStationaryPositions;
    }

    internal override void Init(Dictionary<string, object> paramDict)
    {
        var mainEnemyUnitsCollector = UnitsCollector.Instance(PlayerNo).mainEnemyUnitsCollector;
        
        FillWay(paramDict, mainEnemyUnitsCollector.bases[0].CellControllerBase.position);
        foreach(CellController cell in CellControllerBase.nearCells)
        {
            if (Way.Contains(cell.position))
                defaultPosition = cell;
        }
        
    }

    public override void OnBuildInit()
    {
        //Debug.Log("OnBuild Init");
        BuildSlider.gameObject.SetActive(true);
        IsBuilding = true;
    }

    private void Build()
    {
        BuildProgress += Time.deltaTime;
        buildSlider.SetValue(BuildProgress / BuildPoints);

        if (BuildProgress >= BuildPoints)
        {
            IsBuilding = false;
            buildSlider.gameObject.SetActive(IsBuilding);
        }
    }
}
