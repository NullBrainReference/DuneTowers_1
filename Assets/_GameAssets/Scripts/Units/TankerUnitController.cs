using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankerUnitController : UnitController, IBuildable
{

    [Space]
    public SliderUnit sliderUnit;

    [SerializeField]
    private SliderUnit creditsSliderUnit;

    [SerializeField]
    private float repeatRate;
    [SerializeField]
    private float time;

    public float profitBase;

    private float goldStock;

    private bool maxReached = false;

    [SerializeField] private Animator animator;

    [SerializeField] private SliderUnit buildSlider;
    public SliderUnit BuildSlider { get { return buildSlider; } }

    [SerializeField] private float buildPoints;
    public float BuildPoints { get { return buildPoints; } set { buildPoints = value; } }

    private float buildProgress = 0;
    public float BuildProgress { get { return buildProgress; } set { buildProgress = value; } }

    public bool IsBuilding { get; set; } = false;

    private float Profit { get { return profitBase * Efficiency; } }

    private new void Start()
    {
        base.Start();
        InvokeRepeating("AddMoney", time, repeatRate);
        InitCellValues();
    }

    private void Awake()
    {
        unitStats = GameStats.Instance.GetUnit(Unit.Drill, PlayerNo);
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
        animator.Play("BurStay");
        BuildSlider.gameObject.SetActive(true);
        IsBuilding = true;
    }

    protected override void UpdateSlider()
    {
        //print("curHealth / health = " + (curHealth / health));
        sliderUnit.SetValue(CurHealthPercent);
    }

    private void AddDefenceOutcome(float value)
    {
        if (GameStats.Instance.isTowerDefence == false)
            return;

        goldStock += value * GameStats.Instance.GetGoldIncomeMultiplier();
        
        if (goldStock >= 1)
        {
            GameStats.Instance.goldOutcome += (int)goldStock;
            goldStock -= (int)goldStock;
        }
    }

    private void AddMoney()
    {
        if (IsBuilding) 
            return;

        if (CellControllerBase.Credits >= Profit) 
        {
            MoneyManager.Instance(PlayerNo).AddMoney(Profit);
            AddDefenceOutcome(Profit);
            CellControllerBase.Credits -= Profit;
        }
        else if (CellControllerBase.Credits > 0)
        {
            MoneyManager.Instance(PlayerNo).AddMoney(CellControllerBase.Credits);
            AddDefenceOutcome(CellControllerBase.Credits);
            CellControllerBase.Credits = 0;
        }
        else
        {
            MoneyManager.Instance(PlayerNo).AddMoney(Profit * 0.01f);
            AddDefenceOutcome(Profit * 0.01f);
        }

        InitCellValues();
        //creditsSliderUnit.SetValue(CellControllerBase.Credits / CellControllerBase.StartCredits);
    }

    public override void OnLongTouch(CellController cellController, UnitController xUnitController)
    {
        if (!IsPlayer)
            return;

        //SelectManager.Instance.SelectObject(transform);
        print("OnLongTouch " + TransformUrlUtil.GetTransformFullName(transform));
    }



    internal override DeviceController.SituationResult GetSituationResult(List<UnitController> units, List<UnitController> enemyUnits)
    {
        return null; //base.GetSituationResult(units, enemyUnits);
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
            Debug.LogError("New option - " + situationResult.ToString() + "  " + TransformUrlUtil.GetTransformFullName(transform));

    }

    public void InitCellValues()
    {
        creditsSliderUnit.SetValue(CellControllerBase.Credits / CellControllerBase.StartCredits);
    }

    private void Build()
    {
        BuildProgress += Time.deltaTime;
        buildSlider.SetValue(BuildProgress / BuildPoints);

        if (BuildProgress >= BuildPoints)
        {
            animator.Play("BurStartAnimation");
            IsBuilding = false;
            buildSlider.gameObject.SetActive(IsBuilding);
        }
    }

}
