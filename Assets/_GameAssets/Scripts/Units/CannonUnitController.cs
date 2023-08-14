using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonUnitController : UnitController, IBuildable
{

    [Space]
    
    public SliderUnit sliderUnit;
    public CannonController cannonController;
    public KnobUnitController knobUnitController;

    [SerializeField] private SliderUnit buildSlider;
    public SliderUnit BuildSlider { get { return buildSlider; } }

    [SerializeField] private float buildPoints;
    public float BuildPoints { get { return buildPoints; } set { buildPoints = value; } }

    private float buildProgress = 0;
    public float BuildProgress { get { return buildProgress; } set { buildProgress = value; } }

    public bool IsBuilding { get; set; } = false;


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    print(collision.gameObject.name);

    //    UnitController other = collision.GetComponent<UnitController>();
    //    if (other == null) return;

    //    if (IsEnemy(other))
    //    {
    //        print("враг");
    //    }
    //    else
    //    {
    //        print("Свой");
    //    }

    //}



    //void OnTriggerEnter2D(Collider2D col)
    //{
    //    Debug.Log("GameObject1 collided with " + col.name);

    //}

    private void Awake()
    {
        unitStats = GameStats.Instance.GetUnit(Unit.Tower, PlayerNo);
        InitUnitStats();

        //UnitsCollector.Instance(PlayerNo).AddUnit(this);
    }

    private void FixedUpdate()
    {
        if (IsBuilding)
        {
            Build();
        }
    }

    public override void OnBuildInit()
    {
        //Debug.Log("OnBuild Init");
        BuildSlider.gameObject.SetActive(true);
        IsBuilding = true;
    }

    protected override void UpdateSlider()
    {
        //print("curHealth / health = " + (curHealth / health));
        //if (sliderUnit != null)
            sliderUnit.SetValue(CurHealthPercent);
    }


    //public override void OnShortTouch(CellController cellController, UnitController xUnitController)
    //{
    //    //SelectManager.Instance.SelectObject(transform);
    //    print("OnLongTouch " + TransformUrlUtil.GetTransformFullName(transform));
    //}

    public override void OnLongTouch(CellController cellController, UnitController xUnitController)
    {
        if (!IsPlayer) 
            return;

        if (!selectObj)
            return;

        UnitController unitController2 = selectObj.transform.parent.GetComponent<UnitController>();

        if (this == unitController2)
        {
            //SelectManager.Instance.SelectObject(transform);
            knobUnitController.UpdateStatusUnitActive();
        }
    }

    public override void OnNextShortTouch(CellController cellController)
    {
        CanvasManager.Instance.sellButton.Turn(false, false, 0, null);

        if (!IsPlayer)
            return;

        cellController.Select();
        Unselect();
        //UnitMover unitMover = GetComponent<UnitMover>();

        if (cannonController.CheckTarget(cellController.Child)) // если он в целях, то нацеливаем на него
        {


            cannonController.ChangeTarget(true, cellController.Child);
        }

        //SelectManager.Instance.Follow(this, 10f);
        //print("OnNextShortTouch " + TransformUrlUtil.GetTransformFullName(transform));
    }


    public override bool IsOn()
    {
        return knobUnitController == null || knobUnitController.isOn;
    }

    internal override DeviceController.SituationResult GetSituationResult(List<UnitController> units, List<UnitController> enemyUnits)
    {
        return null;// base.GetSituationResult(units, enemyUnits);
    }

    internal override void ApplyAction(DeviceController.SituationResult situationResult)
    {
        if (situationResult.situationResultType == DeviceController.SituationResultType.Build)
            MapManager.CreateUnit(situationResult.prefab, situationResult.position);
        //else if (situationResult.situationResultType == DeviceController.SituationResultType.NewTarget)
        //    Debug.LogError("NewTarget");
        //else if (situationResult.situationResultType == DeviceController.SituationResultType.Stop)
        //    Debug.LogError("Stop");
        else
            Debug.LogError("New option - " + situationResult.ToString() +  "  " + TransformUrlUtil.GetTransformFullName(transform));

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
