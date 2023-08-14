using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DeviceController;

public enum UnitLayer { None, Building, Ground, Air}

public class UnitController : MonoBehaviour
{

    private const int player = 0;

    //public struct SelectObject 
    //{
    //    public GameObject selectObj;
    //    public UnitController nitController;
    //}

    public UnitLayer unitLayer;
    public bool isAllowedToMoveOver;
    //public bool isAllowedToBuildOver;

    public UnitStats unitStats;

    public enum UnitType { Stationary=0, Mobile=1, Temp=2, Foundation=3, Obstruction=4}

    public int PlayerNo;

    public UnitType unitType = UnitType.Stationary;

    public float health ;

    public float powerBullet = 5;

    private float speedAdd = 0;

    public bool isInvincible;

    [SerializeField] protected CannonController cannon;

    public bool HasTarget { 
        get {
            if (cannon == null) return false;

            if (cannon.targets.Count > 0) return true;

            return false;
        } 
    }

    [SerializeField] protected bool isSelectCell = false;

    public int Health { get { return unitStats.Armor; } }

    public float PowerBullet { get { return unitStats.Damage; } }

    [Range(0, 0.5f)]
    public float protection = 0.2f;
    //public float regeneration = 5; //per sec 

   
    public GameObject explosionPrefab;
    public GameObject selectPrefab;
    public GameObject smokeObj;
    public GameObject flameObj;

    [SerializeField]
    private CellController cellControllerBase;

    public CellController CellControllerBase { set { cellControllerBase = value; } get { return cellControllerBase; } }

    //public Renderer[] renderers;
    
    public static GameObject selectObj;
    //public UnitController parent;

    public virtual UnitController GetUnitControllerFactory()
    {
        throw new NotImplementedException("Realase in child");
    }


    public virtual void SetUnitControllerFactory(UnitController unitController)
    {
        throw new NotImplementedException("Realase in child");
    }

    /// <summary>
    /// текущее здоровье 
    /// </summary>
    //[SerializeField]
    [SerializeField] private float curHealth;
    private float CurHealth {
        set {
            
            curHealth = value;
        }
        get {
            
            return curHealth;
        }
    }

    /// <summary>
    /// КПД
    /// </summary>
    /// <returns></returns>
    public float Efficiency
    {
        get
        {
            if (CurHealth <= 0) return 0;
            //return (health / CurHealth) / 2 + 0.5f + speedAdd;

            return 1 + speedAdd;
        }
    }

    public float CurHealthPercent
    {
        get { return CurHealth / health; }
    }

    public bool IsPlayer
    {
        get
        {
            return PlayerNo == player;
        }
    }

    public float CurPowerBullet { get { return powerBullet; } }
    private float CurProtection { get { return protection; } }
    //private float Curregeneration { get { return regeneration; } }
    private bool initialized;

    public UnitController() : base()
    {

        
        
        //Initialization();
        //curHealth = health;
        
        //UnitManager.InitActions.Add(
        //        () =>
        //        {
        //            foreach (Renderer renderer in renderers)
        //            {
        //                if (renderer.GetType() == typeof(SpriteRenderer))
        //                    (renderer as SpriteRenderer).color = UnitManager.Instance.playerColors[PlayerNo];
        //                else if (renderer.GetType() == typeof(MeshRenderer))
        //                {
        //                    MeshRenderer meshRenderer = (renderer as MeshRenderer);
        //                    meshRenderer.sharedMaterials[0].color = UnitManager.Instance.playerColors[PlayerNo];
        //                }
        //            }
        //        }
        //    );
    }



    public void Start()
    {
        //DeviceController.AddUnit(this);
        curHealth = health;
        if (this.GetType() != typeof(SelectCellController))
            UnitsCollector.AcyncAddUnitController(PlayerNo, this);

        CanvasManager.Instance.UpdateCost();

        ObstructionsManager.AcyncAddUnitController(this);

        gameObject.name += $" - {UnityEngine.Random.Range(0, 1000)}";

        StartUpdate();
    }

    public void InitUnitStats()
    {
        health = unitStats.Armor;
        curHealth = health;
        speedAdd = unitStats.GetEfficiency();
    }

    protected virtual void StartUpdate()
    {

    }

    public virtual void OnBuildInit()
    {

    }

    public bool IsEnemy(UnitController other)
    {
        return PlayerNo != other.PlayerNo && other.unitType != UnitType.Obstruction ;
    }

    public bool IsEnemy(int playerNo)
    {
        return PlayerNo != playerNo;
    }

    /// <summary>
    /// unit работает
    /// </summary>
    public virtual bool IsOn()
    {
        throw new NotImplementedException("Реализовать в потомке - " + TransformUrlUtil.GetTransformFullName(transform));
    }

    public List<ActionUnit> destroyLiseners = new List<ActionUnit>();
    

    // Атака Юнита
    public void HitUnit(float value)
    {
        IBuildable buildable = this as IBuildable;

        float damage = value;

        if (buildable != null)
        {
            if (buildable.IsBuilding)
                damage *= 2;
        }

        CurHealth -= damage * (1 - CurProtection);
        if (CurHealthPercent < 0.5f)
            if (smokeObj)
                smokeObj.SetActive(true);

        if (CurHealthPercent < 0.25f)
            if (flameObj)
                flameObj.SetActive(true);


        UpdateSlider();
        if (CurHealth < 0)
        {
            DestroyUnit();
        };       
    }

    protected virtual void UpdateSlider()
    {
        throw new NotImplementedException("Реализовать в потомке - " + TransformUrlUtil.GetTransformFullName(transform));
    }

    // регенерация
    public void StartRegeneration()
    {
        throw new NotImplementedException("реализовать через корутину - " + TransformUrlUtil.GetTransformFullName(transform));
    }

    public void DestroyUnit()
    {
        if (isInvincible)
            return;

        Explosion();
        
        foreach (ActionUnit actionUnit in destroyLiseners)
        {
            actionUnit.action.Invoke();
        }

        //if (CellControllerBase != null)
        //{
        //    CellControllerBase.SetChild(null);
        //    CellControllerBase.ClearPlate();
        //}

        if (PlayerNo == 1)
        {
            AchievementsManager.Instance.DestroyUpdate(1);
            GameStats.Instance.Profile.StatIncrement(ProfileStat.Kill);
        }
        else
        {
            GameStats.Instance.Profile.StatIncrement(ProfileStat.Death);
        }

        HeadquartersUnitController.Instance.ReareateSelectCells();

        Destroy(gameObject); //TODO fix null ref
    }

    public virtual void ExtraDestroyAction()
    {

    }

    public void OnDestroy()
    {
        if (isSelectCell) return;

        ExtraDestroyAction();

        //if (CellControllerBase == null) 
        //    return;

        if (UnitsCollector.Instance(PlayerNo) == null) 
            return;
        UnitsCollector.Instance(PlayerNo).RemoveUnit(this);

        //if (CellControllerBase.CheckPlate() && unitStats.Unit == Unit.Plate)
        //{
        //    CellControllerBase.RemovePlate(this);
        //    return;
        //}
        //else if (unitStats.Unit == Unit.Plate)
        //{
        //    //CellControllerBase.SetChild(null, unitStats.Unit);
        //    CellControllerBase.RemoveChild2(this);
        //    //CellControllerBase.RemovePlate(this);
        //    return;
        //}

        CanvasManager.Instance.sellButton.Turn(false, true, 0, this);

        if (CellControllerBase?.Child == this)
            CellControllerBase.RemoveChild2(this); //CellControllerBase.SetChild(null, unitStats.Unit);

        if (unitStats.Unit == Unit.Tower ||
            unitStats.Unit == Unit.Plate ||
            unitStats.Unit == Unit.Drill ||
            unitStats.Unit == Unit.Fabric)
            ScanerManager.Instance.UpdateScaner(this.cellControllerBase);

        if (unitStats.Unit == Unit.Fabric)
        {
            TankFactoryUnitController factory = this as TankFactoryUnitController;
            CanvasManager.Instance.currUnitCreator = factory.unitCreator;
            CanvasManager.Instance.UpdateCost();

            if (factory.unitCreator == CanvasManager.Instance.currUnitCreator)
            {
                CanvasManager.Instance.currUnitCreator = null;
                CanvasManager.Instance.SwitchFabricPanel(false);
            }
        }
    }

    private void Explosion()
    {
        GameObject exp = Instantiate(explosionPrefab, transform.parent);
        exp.transform.position = transform.position;
    }

    public virtual void OnShortTouch(CellController cellController, UnitController xUnitController)
    {
        if (PlayerNo == 1)
            return;

        Select();

        if (PlayerNo != 0)
            return;

        if (isSelectCell)
            return;

        if (unitStats.Unit == Unit.Fabric)
        {
            TankFactoryUnitController factory = this as TankFactoryUnitController;
            CanvasManager.Instance.currUnitCreator = factory.unitCreator;

            CanvasManager.Instance.SwitchFabricPanel(true);
            CanvasManager.Instance.productionController.InitGroup(factory.unitCreator.productionType);
        }
        else
        {
            CanvasManager.Instance.SwitchFabricPanel(false);
        }

        switch (unitStats.Unit)
        {
            case Unit.Fabric:
                CanvasManager.Instance.sellButton.Turn(true, false, unitStats.battlePrice / 2, this);
                break;
            case Unit.Tower:
                CanvasManager.Instance.sellButton.Turn(true, false, unitStats.battlePrice / 2, this);
                break;
            case Unit.Plate:
                CanvasManager.Instance.sellButton.Turn(false, false, 0, null);
                //CanvasManager.Instance.sellButton.Turn(true, false, unitStats.battlePrice / 2, this);
                break;
            case Unit.Drill:
                CanvasManager.Instance.sellButton.Turn(true, false, unitStats.battlePrice / 2, this);
                break;
            case Unit.Tank:
                CanvasManager.Instance.sellButton.Turn(false, false, 0, null);
                break;
            case Unit.Helicopter:
                CanvasManager.Instance.sellButton.Turn(false, false, 0, null);
                break;
            case Unit.Base:
                CanvasManager.Instance.sellButton.Turn(false, false, 0, null);
                break;
        }
        //SelectManager.Instance.SelectObject(transform);
        //throw new NotImplementedException("Реализовать в потомке -" + TransformUrlUtil.GetTransformFullName(transform));
    }

    public virtual void OnLongTouch(CellController cellController, UnitController xUnitController)
    {
        throw new NotImplementedException("Реализовать в потомке -" + TransformUrlUtil.GetTransformFullName(transform));
    }

    public virtual void OnNextShortTouch(CellController cellController)
    {
        throw new NotImplementedException("Реализовать в потомке -" + TransformUrlUtil.GetTransformFullName(transform));
    }

    public virtual void OnNextLongTouch(CellController cellController)
    {
        throw new NotImplementedException("Реализовать в потомке -" + TransformUrlUtil.GetTransformFullName(transform));
    }

    internal virtual void Init(Dictionary<string,object> paramDict)
    {
        
    }

    private void Select()
    {
        Unselect();

        selectObj = Instantiate(selectPrefab, transform);

        selectObj.transform.position = new Vector3(transform.position.x, transform.position.y, selectObj.transform.position.z);
        selectObj.transform.rotation = transform.rotation; 

    }

    public virtual void Unselect()
    {
        if (selectObj)
        {
            Animator animator = selectObj.GetComponent<Animator>();
            animator.SetBool("SelectOff", true);
        }
    }

    internal virtual void ApplyAction(SituationResult situationResult)
    {
        throw new NotImplementedException("Реализовать в потомке -" + TransformUrlUtil.GetTransformFullName(transform));
    }


    internal virtual SituationResult GetSituationResult(List<UnitController> units, List<UnitController> enemyUnits)
    {
        throw new NotImplementedException("Реализовать в потомке -" + TransformUrlUtil.GetTransformFullName(transform));
    }
}
