using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover2 : MonoBehaviour
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

    [SerializeField]
    private Target? target;

    [SerializeField]
    private Target? newTarget;


    #endregion




    #region additional -------------------------------------------------------------------
    public enum States { AtFactory, Moving, Turning, Stop }
    private UnitController _unitController;
    private UnitController UnitController
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

    public float MoveSpeed { get { return movingSpeed * UnitController.unitStats.GetSpeed(); } }
    public float TurnRate { get { return turningSpeed * UnitController.unitStats.GetSpeed(); } }


    [System.Serializable]
    private struct Target
    {
        public Vector2Int target;
        public Action action;

        public Target(Vector2Int target, Action action)
        {
            this.target = target;
            this.action = action;
        }
    }

    #endregion



    #region methods -------------------------------------------------------------------

    public void SetNewTarget(Vector2Int target, Action action)
    {
        if (state == States.AtFactory || state == States.Stop)
        {
            this.target = new Target(target, action);
            var newNextPos = GetNextPos();
            if (newNextPos != null)
                StartCoroutine(TurningCoroutine((Vector2Int)newNextPos));
        }
        else
            this.newTarget = new Target(target, action);
    }


    private List<Vector2Int> CalcWay(Vector2Int target)
    {

        bool useLake = UnitController.unitStats.Unit != Unit.Helicopter;
        var way = CalcWayBase(target, useLake, true);

        if (way.Contains(CurPos))

        if (way.Count==0 || !way.Contains(CurPos))
              way = CalcWayBase(target, useLake, false);

        if (way.Contains(CurPos))
            way.Remove(CurPos);


        if (way.Count == 0 || !way.Contains(CurPos))
            way.Clear();

        return way;
    }

    private List<Vector2Int> CalcWayBase(Vector2Int target, bool isAir, bool useEnemy)
    {
        print(UnitController.gameObject.name + " - CalcWay");
        //Obstructions.Clear();
        var obstructions = new List<Vector2Int>();

        if (isAir)
        {
            obstructions.AddRange(ObstructionsManager.Instance.allPositions);
        }
        else
        {
            obstructions.AddRange(ObstructionsManager.Instance.rockPositions);
        }


        obstructions.AddRange(UnitsCollector.Instance(UnitController.PlayerNo).GetAllUnitObstructionPositions());
        if (useEnemy)
            obstructions.AddRange(UnitsCollector.Instance(UnitsCollector.players.Find(x => !x.Equals( UnitController.PlayerNo))).GetAllUnitObstructionPositions());

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





    private Vector2Int? GetNextPos()
    {
        if (newTarget != null || way.Count == 0)
        {
            if (newTarget != null)
            {
                target = newTarget;
                newTarget = null;
            }

            way = CalcWay(((Target)target).target);
        }

        if (CurPos == target?.target)
            return null;

        //if (Way.Count == 0)
        //    CalcWay();

        if (way.Count == 0)
            return null;

        var nexPos = way[0];

        CellController nextCellController = CellsManager.Instance.cellControllersArr[nexPos.x, nexPos.y];
        if (!nextCellController.ChildEmpty())
            way = CalcWay(((Target)target).target);

        if (way.Count == 0)
            return null;

        nexPos = way[0];
        way.Remove(way[0]);
        ////nextCellController = CellsManager.Instance.cellControllersArr[nexPos.x, nexPos.y];
        return nexPos;
    }


    private bool UnitArrived(Vector2Int nextPos)
    {

        if (way.Count == 0)
            return true;

        //CellController nextCellController = CellsManager.Instance.cellControllersArr[nextPos.x, nextPos.y];

        //var targetPos = ((Target)target).target;

        //CellController targetCellController = CellsManager.Instance.cellControllersArr[targetPos.x, targetPos.y];
        //targetCellController.Child?.PlayerNo == 
        //targetLocal.target

        //var dV = CurPos - nextPos;
        

        //nextCellController.
        return false;
    }

    #endregion



    #region coroutines -------------------------------------------------------------------

    private IEnumerator TurningCoroutine(Vector2Int nextPos)
    {

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

        if (UnitArrived(nextPos))
        {
            State = States.Stop;
        }
        else if (newTarget != null)
        {
            var newNextPos = GetNextPos();
            if (newNextPos != null)
                StartCoroutine(TurningCoroutine((Vector2Int)newNextPos));
            else
                State = States.Stop;
        }
        else
        {
            var newNextPos = GetNextPos();
            if (newNextPos != null)
                StartCoroutine(MovingCoroutine((Vector2Int)newNextPos));
            else
                State = States.Stop;
        }
    }



    private IEnumerator MovingCoroutine(Vector2Int nextPos)
    {
        State = States.Moving;

        
        CellController curCellController = CellsManager.Instance.cellControllersArr[CurPos.x, CurPos.y];
        Vector2 globalPos0 = curCellController.transform.position;

        CellController nextCellController = CellsManager.Instance.cellControllersArr[nextPos.x, nextPos.y];
        Vector2 globalNextPos = curCellController.transform.position;

        //curCellController.SetChild(null, UnitController.unitStats.Unit);
        curCellController.RemoveChild2(UnitController); //(null, UnitController.unitStats.Unit);
        nextCellController.SetChild2(UnitController);
        

        var speedMultiplier = GetSpeedMultiplier(nextPos); //diagonal slowly

        float percent = 0;
        while (percent < 1)
        {
            yield return new WaitForEndOfFrame();
            percent += speedMultiplier * MoveSpeed * Time.deltaTime;
            transform.localPosition = globalPos0 + (globalNextPos - globalPos0) * percent;
        }

        if (target?.action != null)
        {
            var action = ((Target)target).action;
            action.Invoke();
            target = new Target(((Target)target).target, null);
        }

        var newNextPos = GetNextPos();
        if (newNextPos != null)
            StartCoroutine(TurningCoroutine((Vector2Int)newNextPos));
        else
            State = States.Stop;

    }

    #endregion
}
