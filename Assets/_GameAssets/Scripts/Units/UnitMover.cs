using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour
{

    #region Setup fields -------------------------------------------------------------------
    [SerializeField]
    public float turningSpeed = 0.5f;

    [SerializeField]
    public float movingSpeed = 0.01f;


    #endregion 



    #region analytical fields -------------------------------------------------------------------

    //[SerializeField]
    //private Vector2Int curTarget;
    [Space]

    [SerializeField]
    private States state;

    [SerializeField]
    private List<Vector2Int> way;
    public List<Vector2Int> Way { get { return way; } }

    [SerializeField]
    private Target? target;
    public Target? CurTarget { get { return target; } }


    [SerializeField]
    private Target? newTarget;

    [SerializeField]
    [Range(0f, 1f)]
    private float percent;

    [SerializeField]
    private Vector2Int nexPos;

    public bool atFactory;
    #endregion




    #region additional -------------------------------------------------------------------
    public enum States { AtFactory, Moving, Turning, Stop }
    private UnitController _unitController;
    public UnitController UnitController
    {
        get
        {
            if (_unitController == null)
                _unitController = GetComponent<UnitController>();

            return _unitController;
        }
    }
    private Vector2Int CurPos { get { return UnitController.CellControllerBase?.position ?? UnitController.GetUnitControllerFactory().CellControllerBase.position; } }

    public States State { private set { state = value; } get { return state; } }

    private Vector2Int xDirection = Vector2Int.zero;

    [SerializeField] private UnitSoundManager soundManager;

    public float MoveSpeed { get { return movingSpeed * UnitController.unitStats.GetSpeed(); } }
    public float TurnRate { get { return turningSpeed * UnitController.unitStats.GetSpeed(); } }
    private UnitLayer UnitLayer { get { return UnitController.unitLayer; } }


    [System.Serializable]
    public struct Target
    {
        public Vector2Int target;
        public Action action;

        public Target(Vector2Int target, Action action)
        {
            this.target = target;
            this.action = action;
        }
        public override string ToString()
        {
            return target.ToString();
        }
    }


    //public void PrintWay(List<Vector2Int> way, int value)
    //{
    //    Debug.Log(">>>-------------------");
    //    Debug.Log(value);
    //    foreach (var item in way)
    //        Debug.Log(item);
    //    Debug.Log("<<<-------------------");
    //}

    #endregion



    #region methods -------------------------------------------------------------------

    public void SetUpNonFactoryMover()
    {
        state = States.Stop;
    }

    public void MoveToCell(Vector2Int target, Action action = null)
    {

        if (state == States.AtFactory || state == States.Stop)
        {
            StopAllCoroutines();

            this.target = new Target(target, action);
            way.Clear();
            var newNextPos = GetNextPos(false);
            if (newNextPos != null)
                StartCoroutine(TurningCoroutine((Vector2Int)newNextPos));
            else if (UnitController.PlayerNo == 0)
                StartCoroutine(WaitingCoroutine(target));
        }
        else
        {
            this.newTarget = new Target(target, action);
        }
    }

    //public void ForceOneCellMove()
    //{
    //
    //}

    public List<Vector2Int> CalcWay(Vector2Int target, bool ignoreEnemy)
    {
        bool isAir = UnitController.unitStats.Unit == Unit.Helicopter;

        List<Vector2Int> way;

        //if (isAir && state == States.AtFactory)
        //{
        //    way = CalcWayBase(target, isAir, false);
        //}
        //else
        //{
        way = CalcWayBase(target, isAir, true);

        if (ignoreEnemy)
            if (way.Count == 0 || !way.Contains(CurPos))
                way = CalcWayBase(target, isAir, false);
        //}

        if (way.Count == 0 || !way.Contains(CurPos))
            way.Clear();

        if (way.Contains(CurPos))
            way.Remove(CurPos);

        return way;
    }

    private List<Vector2Int> CalcWayBase(Vector2Int target, bool isAir, bool useEnemy)
    {
        //print(UnitController.gameObject.name + " - CalcWay");
        //Obstructions.Clear();
        var obstructions = new List<Vector2Int>();

        if (isAir)
        {
            //obstructions.AddRange(ObstructionsManager.Instance.allPositions);
            obstructions.AddRange(UnitsCollector.Instance(UnitController.PlayerNo).GetAirObstructionPositions());
        }
        else
        {
            obstructions.AddRange(ObstructionsManager.Instance.allPositions);
            obstructions.AddRange(UnitsCollector.Instance(UnitController.PlayerNo).GetAllUnitObstructionPositions());
        }

        //obstructions.AddRange(UnitsCollector.Instance(UnitController.PlayerNo).GetAllUnitObstructionPositions());
        if (useEnemy)
        {
            if (isAir)
                obstructions.AddRange(UnitsCollector.Instance(UnitsCollector.players.Find(
                    x => !x.Equals(UnitController.PlayerNo))).GetAirObstructionPositions()
                    );
            else
                obstructions.AddRange(UnitsCollector.Instance(UnitsCollector.players.Find(
                    x => !x.Equals(UnitController.PlayerNo))).GetAllUnitObstructionPositions()
                    );
        }

        CourseSearcher courseSearcher = new CourseSearcher(
           MapManager.Instance.fieldSize,
           CurPos,
           target,
           obstructions
           );

        //Obstructions = obstructions;
        //Obstructions.Sort((a, b) => {
        //    int res = a.x.CompareTo(b.x);
        //    return res == 0 ? a.y.CompareTo(b.y) : res;
        //});

        var way = courseSearcher.GetWay();

        return way;
    }


    

    private float GetSpeedMultiplier(Vector2Int nextPos)
    {
        float speedMultiplier = 1;

        int dX = Math.Abs(CurPos.x - nextPos.x);
        int dY = Math.Abs(CurPos.y - nextPos.y);

        if (dX == dY) //diagonal
            speedMultiplier = 0.71f;
        return speedMultiplier;
    }





    private Vector2Int? GetNextPos(bool removeItem)
    {
        if (newTarget != null || way.Count == 0)
        {
            if (newTarget != null)
            {
                target = newTarget;
                newTarget = null;
            }

            way = CalcWay(((Target)target).target,true);
            //PrintWay(way, 5);
        }


        var targetPos = ((Target)target).target;
        var dV = targetPos - CurPos;
        if(MathF.Abs(dV.x) <= 1 && MathF.Abs(dV.y) <= 1 && (MathF.Abs(dV.x) + MathF.Abs(dV.y) == 1))
        {
            if (way.Count == 0)
                way.Add(targetPos);

            CellController targetCellController = CellsManager.Instance.cellControllersArr[targetPos.x, targetPos.y];
            if (targetCellController.IsFreeToMoveOver(UnitController))
                return targetPos;
        }

        if (CurPos == target?.target)
            return null;

        //if (Way.Count == 0)
        //    CalcWay();

        if (way.Contains(CurPos))
            way.Remove(CurPos);

        if (way.Count == 0)
            return null;

        var nexPos = way[0];

        CellController nextCellController = CellsManager.Instance.cellControllersArr[nexPos.x, nexPos.y];
        if (!nextCellController.ChildEmpty(UnitController.unitStats.Unit))
            way = CalcWay(((Target)target).target,false);

        //PrintWay(way, 6);

        if (way.Count == 0)
            return null;

        nexPos = way[0];
        this.nexPos = nexPos;

        //if (removeItem)
        //    way.Remove(way[0]);
        ////nextCellController = CellsManager.Instance.cellControllersArr[nexPos.x, nexPos.y];
        return nexPos;
    }


    private bool UnitArrived(Vector2Int nextPos)
    {
        //if (CurPos == nextPos)
        //    return true;

        //TODO causes one cell moving stop issue
        if (way.Count == 0)
            return true;


        if (target != null)
        {
            var targetPos = ((Target)target).target;
            var dV = targetPos - CurPos;
            if (MathF.Abs(dV.x) <= 1 && MathF.Abs(dV.y) <= 1)
            {
                CellController targetCellController = CellsManager.Instance.cellControllersArr[targetPos.x, targetPos.y];
                if (!targetCellController.IsFreeToMoveOver(UnitController))
                    return true;
            }

        }




        //CellController nextCellController = CellsManager.Instance.cellControllersArr[nextPos.x, nextPos.y];

        //var targetPos = ((Target)target).target;

        //CellController targetCellController = CellsManager.Instance.cellControllersArr[targetPos.x, targetPos.y];
        //targetCellController.Child?.PlayerNo == 
        //targetLocal.target

        //var dV = CurPos - nextPos;


        //nextCellController.
        return false;
    }

    //    //private bool UnitArrived()
    //    //{
    //    //    //if (atFactory)
    //    //        return false;
    //    //    //var dV = target - GetCurPos1();
    //    //    //return MathF.Abs(dV.x) < 2 && MathF.Abs(dV.y) < 2;
    //   //}

    #endregion



    #region coroutines -------------------------------------------------------------------

    private IEnumerator TurningCoroutine(Vector2Int nextPos)
    {

        if (soundManager.IsMovePlaying == false)
            soundManager.PlayMove();

        //print("TurningCoroutine");
        State = States.Turning;
        CellController nextCellController = CellsManager.Instance.cellControllersArr[nextPos.x, nextPos.y];

        Vector2 globalNextPos = nextCellController.transform.position;

        var canMoving = (xDirection == CurPos - nextPos);
        while (!canMoving)
        {
            yield return new WaitForEndOfFrame();
            canMoving = LookAt2DUtility.LookAt2D(transform, globalNextPos, TurnRate * Time.deltaTime);
        }

        xDirection = CurPos - nextPos;
        //TODO one cell moving issue UnitArrived returned True
        if (UnitArrived(nextPos))
        {
            soundManager.StopMove();
            State = States.Stop;
        }
        else if (newTarget != null)
        {
            var newNextPos = GetNextPos(false);

            if (newNextPos != null)
            {
                StartCoroutine(TurningCoroutine((Vector2Int)newNextPos));
            }
            else 
            {
                State = States.Stop;
                soundManager.StopMove();


                if (UnitController.PlayerNo == 0)
                    StartCoroutine(WaitingCoroutine(nextPos));
            }
        }
        else
        {
            var newNextPos = GetNextPos(false);
            if (newNextPos != null)
            {
                StartCoroutine(MovingCoroutine((Vector2Int)newNextPos));
            }
            else
            {
                State = States.Stop;
                soundManager.StopMove();

                if (UnitController.PlayerNo == 0)
                    StartCoroutine(WaitingCoroutine(nextPos));
            }
        }
    }

    private IEnumerator WaitingCoroutine(Vector2Int nextPos)
    {
        State = States.Stop;

        CellController nextCellController = CellsManager.Instance.cellControllersArr[nextPos.x, nextPos.y];
        Vector2 globalNextPos = nextCellController.transform.position;

        if (target?.action != null)
        {
            var action = ((Target)target).action;
            action.Invoke();
            target = new Target(((Target)target).target, null);
        }

        var newNextPos = GetNextPos(true);

        while (newNextPos == null)
        {
            newNextPos = GetNextPos(true);

            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(TurningCoroutine((Vector2Int)newNextPos));
    }


    private IEnumerator MovingCoroutine(Vector2Int nextPos)
    {
        if (soundManager.IsMovePlaying == false)
            soundManager.PlayMove();

        State = States.Moving;


        CellController curCellController = CellsManager.Instance.cellControllersArr[CurPos.x, CurPos.y];
        Vector2 globalPos0 = curCellController.transform.position;
        //print("globalPos0 - " + globalPos0);

        CellController nextCellController = CellsManager.Instance.cellControllersArr[nextPos.x, nextPos.y];
        Vector2 globalNextPos = nextCellController.transform.position;
        //print("globalNextPos - " + globalNextPos);

        float speedMultiplier = GetSpeedMultiplier(nextPos); //diagonal slowly

        var curCellChild = curCellController.Child;
        if (curCellChild != null && curCellChild?.GetType() != typeof(TankFactoryUnitController))
            curCellController.RemoveChild2(UnitController); //SetChild(null, UnitController.unitStats.Unit);
        nextCellController.SetChild2(UnitController);

        percent = 0;
        while (percent < 1)
        {
            yield return new WaitForEndOfFrame();
            percent += speedMultiplier * MoveSpeed * Time.deltaTime;

            transform.localPosition = Vector3.Lerp(globalPos0, globalNextPos, percent);
            //print(curPos + " - " + percent);
        }

        if (target?.action != null)
        {
            var action = ((Target)target).action;
            action.Invoke();
            target = new Target(((Target)target).target, null);
        }

        var newNextPos = GetNextPos(true);



        if (newNextPos != null)
        {
            StartCoroutine(TurningCoroutine((Vector2Int)newNextPos));
        }
        else
        {
            State = States.Stop;
            soundManager.StopMove();
        }

    }

    #endregion
}


//public class UnitMover : MonoBehaviour
//{

//    //private class TaskMoving
//    //{
//    //    //public Vector2Int xPos;
//    //    public Vector2Int target;
//    //    public Action action;

//    //    public TaskMoving(Vector2Int target, Action action)
//    //    {
//    //        //this.xPos = xPos;xPos
//    //        this.target = target;
//    //        this.action = action;
//    //    }
//    //}

//    //public enum States { None, Moving, Turning, Stop }

//    //[SerializeField]
//    //private Vector2Int target;

//    //public Vector2Int Target { private set { target = value; } get { return target; } }

    
//    //private UnitController _unitController;
//    //public UnitController UnitController
//    //{
//    //    get
//    //    {
//    //        if (_unitController == null)
//    //            _unitController = GetComponent<UnitController>();
           
//    //        return _unitController;
//    //    }
//    //}


//    //[SerializeField]
//    //private Vector2Int? xPos;


//    //private TaskMoving taskMoving;
//    ////public CannonController cannonController;


//    //[Range(0f, 1f)]
//    //private float percent;

//    //public float turningSpeed = 0.5f;
//    //public float movingSpeed = 0.01f;
//    //public bool canMoving;

//    //public bool atFactory = true;


//    //public Vector2Int xDirection = Vector2Int.zero;

//    //public States state;
//    //public States State
//    //{
//    //    set
//    //    {
//    //        state = value;
//    //        if (state == States.Stop && UnitController.CellControllerBase.Child != UnitController)
//    //            UnitController.CellControllerBase.SetChild(UnitController);

//    //    }
//    //    get { return state; }
//    //}


//    //private Action action;

//    //public List<Vector2Int> Way;


//    //public float MoveSpeed { get { return movingSpeed * UnitController.unitStats.GetSpeed(); } }
//    //public float TurnRate { get { return turningSpeed * UnitController.unitStats.GetSpeed(); } }


//    //public void TaskMoveToCell(Vector2Int target, Action action = null)
//    //{
//    //    taskMoving = new TaskMoving(target, action);
//    //    if (State == States.Stop || State == States.None)
//    //    {
//    //        CalcWay();
//    //        MoveToCell(taskMoving.target, taskMoving.action);
//    //    }

//    //}


//    //public void MoveToCell(Vector2Int target, Action action = null)
//    //{
//    //    taskMoving = null;

//    //    if (action != null)
//    //        this.action = action;

//    //    this.target = target;

        

//    //    //TODO fix turn & move issue
//    //    //StopAllCoroutines();
//    //    //state = State.Stop;
//    //    //if (state == State.Turning)
//    //    //    StartCoroutine(TurnNextCellCoroutine(xPos));

//    //    if (xPos == null)
//    //        xPos = GetCurPos1();

//    //    if (UnitArrived())
//    //        State = States.Stop;
//    //    else
//    //        if (State != States.Moving)
//    //            StartCoroutine(MoveNextCellCoroutine((Vector2Int)xPos));
//    //    //else if (state == State.Moving)
//    //    //    state = State.Stop;


//    //}


//    //private IEnumerator MoveNextCellCoroutine(Vector2Int curPos)
//    //{

        
//    //    //print("MoveNextCellCoroutine");
//    //    //CourseSearcher courseSearcher = new CourseSearcher(MapManager.Instance.fieldSize);

//    //    percent = 0;

//    //    bool nextCellControllerFree = false;
//    //    Vector2Int? nextPos = default;
//    //    int increment = 0;
//    //    CellController xCellController = UnitController.CellControllerBase;

//    //    while (!nextCellControllerFree && (increment < 5 || atFactory))
//    //    {
//    //        //while (isTurning == true)
//    //        //{
//    //        //    Debug.Log("MoveNextCellCoroutine");
//    //        //    yield return new WaitForEndOfFrame();
//    //        //}

//    //        increment++;
//    //        //List<Vector2Int> blocks = new List<Vector2Int>();
//    //        //List<CellController> cellControllers = CellsManager.Instance.cellControllers.FindAll(
//    //        //    x =>
//    //        //    (!x.ChildEmpty() || (x.Child?.IsEnemy(UnitController) ?? false))
//    //        //    && (!x.Child?.Equals(UnitController) ?? false)
//    //        //);
//    //        //foreach (CellController cellController2 in cellControllers)
//    //        //    blocks.Add(cellController2.position);

//    //        //nextPos = courseSearcher.GetNextStep(xPos, target, blocks);


//    //        nextPos = GetNextPos(curPos);
//    //        if (nextPos != null)
//    //        {
//    //            CellController nextCellController = CellsManager.Instance.cellControllersArr[((Vector2Int)nextPos).x, ((Vector2Int)nextPos).y];

//    //            if (nextCellController.ChildEmpty())
//    //            {
//    //                // that it is not reset when moving from the factory
//    //                if (UnitController.unitType == UnitController.UnitType.Mobile && UnitController.CellControllerBase != null)
//    //                    UnitController.CellControllerBase.SetChild(null);

//    //                nextCellController.SetChild(UnitController);
//    //                UnitController.CellControllerBase = nextCellController;

//    //                nextCellControllerFree = true;
//    //            }


//    //            //if (nextCellControllerFree)
//    //            yield return null;
//    //        }
//    //        else
//    //            yield return new WaitForSeconds(0.5f);
//    //    }


//    //    //if (taskMoving != null)
//    //    //{
//    //    //    state = State.Stop;
//    //    //    MoveToCell(taskMoving.xPos, taskMoving.target, taskMoving.action);
//    //    //}
//    //    //else
//    //    if (nextCellControllerFree && !UnitArrived())
//    //    {
//    //        StartCoroutine(TurningCoroutine((Vector2Int)nextPos, curPos));
//    //    }
//    //    else
//    //    {
//    //        State = States.Stop;
//    //    }
//    //}



//    //private IEnumerator TurningCoroutine(Vector2Int nextPos, Vector2Int xPos)
//    //{
//    //    //print("TurningCoroutine");
//    //    State = States.Turning;
//    //    CellController nextCellController = CellsManager.Instance.cellControllersArr[nextPos.x, nextPos.y];

//    //    Vector2 nextPosV = nextCellController.transform.position;

//    //    canMoving = (xDirection == xPos - nextPos);
//    //    while (!canMoving)
//    //    {
//    //        yield return new WaitForEndOfFrame();
//    //        canMoving = LookAt2DUtility.LookAt2D(transform, nextPosV, TurnRate * Time.deltaTime);
//    //    }
//    //    //print("Aiming stop");
//    //    xDirection = UnitController.CellControllerBase.position - nextPos;

//    //    if (taskMoving != null && !UnitArrived())
//    //    {
//    //        State = States.Stop;
//    //        MoveToCell(taskMoving.target, taskMoving.action);
//    //    }
//    //    else
//    //    {            
//    //        StartCoroutine(MovingCoroutine(nextCellController, xPos));
//    //    }
//    //}


//    //private IEnumerator MovingCoroutine(CellController nextCellController, Vector2Int xPosMatrix)
//    //{

//    //    State = States.Moving;

//    //    CellController xCellController = CellsManager.Instance.cellControllersArr[xPosMatrix.x, xPosMatrix.y];
//    //    Vector2 pos0 = xCellController.transform.position;

//    //    if (nextCellController.Child?.Equals(UnitController) ?? false)
//    //    {
//    //        float speedMultiplier = 1;

//    //        int delX = Math.Abs(nextCellController.position.x - xCellController.position.x);
//    //        int delY = Math.Abs(nextCellController.position.y - xCellController.position.y);

//    //        if (delX == delY)
//    //        {
//    //            speedMultiplier = 0.71f;
//    //            //Debug.Log("Diogolal speed " + speedMultiplier * MoveSpeed);
//    //        }

//    //        Vector2 nextPosV = nextCellController.transform.position;
//    //        percent = 0;

//    //        while (percent < 1)
//    //        {
//    //            yield return new WaitForEndOfFrame();
//    //            percent += speedMultiplier * MoveSpeed * Time.deltaTime;
//    //            transform.localPosition = pos0 + (nextPosV - pos0) * percent;
//    //        }

//    //    }

//    //    if (action != null)
//    //        action.Invoke();
//    //    action = null;

//    //    xPos = GetCurPos1();
//    //    if (taskMoving != null)
//    //    {
//    //        State = States.Stop;
//    //        MoveToCell(taskMoving.target, taskMoving.action);
//    //    }
//    //    else if (!(UnitController.CellControllerBase.position).Equals(target))
//    //    {
//    //        StartCoroutine(MoveNextCellCoroutine(UnitController.CellControllerBase.position));
//    //    }
//    //    else
//    //    {

//    //        State = States.Stop;
//    //    }


//    //}

//    //private Vector2Int GetCurPos1()
//    //{
//    //    return UnitController.CellControllerBase?.position ?? UnitController.GetUnitControllerFactory().CellControllerBase.position;
//    //}

//    ////public List<Vector2Int> Obstructions;
//    //public void CalcWay()
//    //{
//    //    print(UnitController.gameObject.name + " - CalcWay");
//    //    //Obstructions.Clear();
//    //    var obstructions = new List<Vector2Int>();
//    //    obstructions.AddRange(ObstructionsManager.Instance.allPositions);
//    //    obstructions.AddRange(UnitsCollector.Instance(0).GetAllUnitObstructionPositions());
//    //    obstructions.AddRange(UnitsCollector.Instance(1).GetAllUnitObstructionPositions());
//    //    CourseSearcher courseSearcher = new CourseSearcher(
//    //       MapManager.Instance.fieldSize,
//    //       GetCurPos1(),
//    //       target,
//    //       obstructions
//    //       );

//    //    //Obstructions = obstructions;
//    //    //Obstructions.Sort((a, b) => {
//    //    //    int res = a.x.CompareTo(b.x);
//    //    //    return res == 0 ? a.y.CompareTo(b.y) : res;
//    //    //});

//    //    Way = courseSearcher.GetWay();
//    //    Way.Remove(GetCurPos1());
//    //}

//    //private Vector2Int? GetNextPos(Vector2Int curPos)
//    //{
//    //    if (curPos == target)
//    //        return null;

//    //    if (Way.Count == 0)
//    //        CalcWay();

//    //    if (Way.Count == 0)
//    //        return null;

//    //    var nexPos = Way[0];

//    //    CellController nextCellController = CellsManager.Instance.cellControllersArr[nexPos.x, nexPos.y];
//    //    if (!nextCellController.ChildEmpty())
//    //        CalcWay();

//    //    if (Way.Count == 0)
//    //        return null;

//    //    nexPos = Way[0];
//    //    Way.Remove(Way[0]);
//    //    //nextCellController = CellsManager.Instance.cellControllersArr[nexPos.x, nexPos.y];
//    //    return nexPos;
//    //}


//    //private bool UnitArrived()
//    //{
//    //    //if (atFactory)
//    //        return false;
//    //    //var dV = target - GetCurPos1();
//    //    //return MathF.Abs(dV.x) < 2 && MathF.Abs(dV.y) < 2;
//   //}
//}

