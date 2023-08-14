using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterDeviceTargetController : MonoBehaviour
{


    //public class EnemyItem
    //{
    //    public UnitController unitController;
    //    public Vector2Int Pos;// { get { return unitController.CellControllerBase.position; } }
    //    public int distance;
    //
    //    public float significance;
    //
    //    public EnemyItem(UnitController unitController, int distance, Vector2Int pos)
    //    {
    //        this.unitController = unitController;
    //        this.distance = distance;
    //        this.Pos = pos;
    //
    //        significance = GetSignificanceByDistance();
    //    }
    //
    //    private float GetSignificanceByDistance()
    //    {
    //        return GetSignificanceUnit() * (1f / distance);
    //    }
    //
    //    private float GetSignificanceUnit()
    //    {
    //        return 1;
    //        //switch (unitController.unitStats.Unit) {
    //        //    case Unit.Tank: return 1f;
    //        //    case Unit.Base: return 1.5f;
    //        //    case Unit.Fabric: return 1.5f;
    //        //    case Unit.Drill: return 1f;                
    //        //    case Unit.Tower: return 0.7f;
    //        //    default: return 0;
    //        //}            
    //    }
    //
    //
    //}
    [SerializeField] private UnitMover unitMover;

    //private IEnumerator Start()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    unitMover = GetComponent<UnitMover>();
    //
    //    //while (unitMover.atFactory)
    //    //{
    //    //    yield return new WaitForSeconds(0.5f);
    //    //}
    //    //yield return new WaitForSeconds(0.5f);
    //
    //
    //
    //    //var target = GetTargetPos();
    //    //unitMover.MoveToCell(target);
    //}

    //private Vector2Int GetTargetPos(List<UnitController> enemyUnits)
    //{
    //    return Vector2Int.zero;
    //}

    internal DeviceController.SituationResult GetSituationResult(List<UnitController> units, List<UnitController> enemyUnits)
    {

        if (unitMover.State != UnitMover.States.Stop)
            return null;

        if (unitMover.UnitController.HasTarget)
            return null;

        var airUnits = units.FindAll(x => x.unitStats.Unit == Unit.Helicopter);
        //var obstructions = enemyUnits.FindAll(x => x.unitType == UnitController.UnitType.Obstruction);
        var obstructions = ObstructionsManager.Instance.rocks;
        CourseSearcher courseSearcher = GetCourseSearcher(airUnits, obstructions);

        var enemyUnitsWithoutObstructions = enemyUnits.FindAll(x => x.unitType != UnitController.UnitType.Obstruction);

        List<TankDeviceTargetController.EnemyItem> enemyItems = new List<TankDeviceTargetController.EnemyItem>();

        foreach (var item in enemyUnitsWithoutObstructions)
        {
            if (item.CellControllerBase != null)
            {
                var pos = item.CellControllerBase.position;
                //var pos = GetBestTarget(courseSearcher, item.CellControllerBase.position, ObstructionsManager.Instance.rocks);
                enemyItems.Add(new TankDeviceTargetController.EnemyItem(item, courseSearcher.Mas[pos.x, pos.y], pos));// courseSearcher.Mas[pos.x, pos.y]));

            }
        }

        enemyItems.Sort((x, y) => y.significance.CompareTo(x.significance));

        var enemyAirUnits = enemyUnits.FindAll(x => x.unitStats.Unit == Unit.Helicopter);

        var allUnits = new List<UnitController>();
        allUnits.AddRange(airUnits);
        allUnits.AddRange(enemyAirUnits);

        var blocks = GetBlocks(allUnits);

        TankDeviceTargetController.EnemyItem enemyItem = GetEnemyItem(enemyItems, blocks);

        if (enemyItems.Count == 0)
            return null;

        var situationResult = new DeviceController.SituationResult();

        situationResult.unitController = unitMover.UnitController;
        situationResult.situationResultType = DeviceController.SituationResultType.Move;
        situationResult.significance = enemyItem.significance;
        situationResult.position = enemyItem.Pos;

        return situationResult;

    }


    private Vector3Int GetBestTarget(CourseSearcher courseSearcher, Vector2Int pos, List<UnitController> rocks)
    {

        bool IsRock(Vector2Int pos)
        {
            return rocks.Find(x => x.CellControllerBase.position == pos) != null;
        }

        List<Vector2Int> variants = new List<Vector2Int>();
        List<Vector2Int> rocksPos = new List<Vector2Int>();


        for (var dx = -2; dx <= 2; dx++)
            for (var dy = -2; dy <= 2; dy++)
            {
                Vector2Int curPos = new Vector2Int(pos.x + dx, pos.y + dy);
                if (courseSearcher.InRange(curPos) && !(dx == 0 && dy == 0))
                    if (!IsRock(curPos))
                        variants.Add(curPos);
                    else
                        if (MathF.Abs(dx) <= 1 || MathF.Abs(dy) <= 1)
                        rocksPos.Add(curPos);
            }

        List<Vector2Int> forRemove = new List<Vector2Int>();

        foreach (var rockPos in rocksPos)
        {
            if (rockPos.x != 0 && rockPos.y != 0)
            {
                //var signX = MathF.Sign(rockPos.x);
                //var signY = MathF.Sign(rockPos.y);
                var forRemove2 = variants.FindAll(m =>
                    ((rockPos.x < 0 && m.x <= rockPos.x) || (rockPos.x > 0 && m.x > rockPos.x))
                    && ((rockPos.y < 0 && m.y <= rockPos.y) || (rockPos.y > 0 && m.y > rockPos.y))
                );
                forRemove.AddRange(forRemove2);
            }
            else
            {
                var forRemove2 = variants.FindAll(m =>
                    ((rockPos.x < 0 && m.x <= rockPos.x) || (rockPos.x > 0 && m.x > rockPos.x))
                    && ((rockPos.y < 0 && m.y <= rockPos.y) || (rockPos.y > 0 && m.y > rockPos.y))
                );
                forRemove.AddRange(forRemove2);
            }

            foreach (var item in forRemove)
                variants.Remove(item);
        }




        var min = 999999;
        Vector2Int result = pos;
        foreach (var item in variants)

        //for (var dx = -2; dx <= 2; dx++)
        //    for (var dy = -2; dy <= 2; dy++)
        {
            Vector2Int curPos = new Vector2Int(pos.x + item.x, pos.y + item.y);
            if (courseSearcher.InRange(curPos))
                if (min > courseSearcher.Mas[curPos.x, curPos.y] && courseSearcher.Mas[curPos.x, curPos.y] > 0)
                {
                    min = courseSearcher.Mas[curPos.x, curPos.y];
                    result = curPos;
                }
        }
        return new Vector3Int(result.x, result.y, min);
    }

    public List<Vector2Int> GetBlocks(List<UnitController> units)
    {
        var result = new List<Vector2Int>();
        foreach (var unit in units)
            if (unit.CellControllerBase != null)
                result.Add(unit.CellControllerBase.position);
        return result;
    }


    public CourseSearcher GetCourseSearcher(List<UnitController> units, List<UnitController> obstructions)
    {
        var blocks = GetBlocks(units);
        blocks.AddRange(GetBlocks(obstructions));
        var position = unitMover.UnitController.CellControllerBase.position;
        CourseSearcher courseSearcher = new CourseSearcher(MapManager.Instance.fieldSize, position, blocks);
        //courseSearcher.FillNearFields(position.x, position.y, 0);

        return courseSearcher;
    }

    private bool CheckWay(TankDeviceTargetController.EnemyItem enemyItem, List<Vector2Int> blocks)
    {
        CourseSearcher courseSearcher = new CourseSearcher(MapManager.Instance.fieldSize);
        var pos = unitMover.UnitController.CellControllerBase.position;
        return courseSearcher.GetNextStep(pos, enemyItem.Pos, blocks) != pos;
    }


    private TankDeviceTargetController.EnemyItem GetEnemyItem(List<TankDeviceTargetController.EnemyItem> enemyItems, List<Vector2Int> blocks)
    {
        foreach (var item in enemyItems)
        {
            if (CheckWay(item, blocks))
                return item;
        }
        return default;
    }


}
