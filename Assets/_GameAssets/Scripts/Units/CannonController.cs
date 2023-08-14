using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{

    public UnitController unitController;
    public List<UnitController> targets;
    public UnitController curTarget;
    public Transform bulletStart;
    public GameObject bulletPrefab;
    public GameObject bulletStartSmokePrefab;
    [Range(0f, 5f)]
    public float deviation = 1f;

    [Range(0f, 1f)]
    public float bulletSmokeDistance = 0.3f;

    public bool randomRotation = true;
    public float attackPeriod = 1;  // attack per sec
    public float aimingSpeed = 1;
    [SerializeField]
    private float minHealthPercentToChangeTarget = 0.2f;

    [SerializeField]
    private List<Vector2Int> ignoreCells;

    private float curPeriod = 0;

    public float AimSpeed { get { return aimingSpeed * unitController.unitStats.GetSpeed(); } }

    //public KnobUnitController knobUnitController;

    private Quaternion zeroQuaternion = Quaternion.Euler(Vector3.zero);
    private bool atGunpoint;

    private bool isPlayerOrder = false;

    [SerializeField]
    private UnitSoundManager soundManager;

    private bool CanAttack 
    { 
        get 
        {
            bool isBuilding = false;

            if (unitController.unitStats.Unit != Unit.Tank && unitController.unitStats.Unit != Unit.Helicopter)
            {
                IBuildable iBuild = (IBuildable)unitController;
                isBuilding = iBuild.IsBuilding;
            }

            return atGunpoint && unitController.IsOn() && !isBuilding;
        } 
    }

    private bool CanAim {
        get
        {
            bool isBuilding = false;

            if (unitController.unitStats.Unit != Unit.Tank && unitController.unitStats.Unit != Unit.Helicopter)
            {
                IBuildable iBuild = (IBuildable)unitController;
                isBuilding = iBuild.IsBuilding;
            }

            return !isBuilding;
        }
    }

    private void Start()
    {
        if (randomRotation)
        {
            Vector3 rotation = transform.rotation.eulerAngles;

            if (unitController.PlayerNo == 0)
            {
                rotation.z = UnityEngine.Random.Range(-60f, 60f);
            }
            else
            {
                rotation.z = UnityEngine.Random.Range(120f, 200f);
            }
            //rotation.z = Random.Range(-180, 180);
            Quaternion quaternion = transform.rotation;
            quaternion.eulerAngles = rotation;
            transform.rotation = quaternion;
        }

        if (unitController.unitType != UnitController.UnitType.Mobile)
            FillIgnoreCells();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print(collision.gameObject.name);

        UnitController other = collision.GetComponent<UnitController>();
        if (other == null) return;

        if (unitController.IsEnemy(other))
        {
            //print(unitController.gameObject.name + " вижу врага");

            targets.Add(other);
            //добавляем что нужно выполнить, если враг уничтожен.
            other.destroyLiseners.Add(
                new ActionUnit(
                    unitController,
                    () =>
                    {
                        try
                        {
                            if (this == null) return;
                            targets.Remove(other);


                            //curTarget = null;
                            //print(unitController.gameObject.name + " враг уничтожен");

                            if (curTarget == other)
                            {
                                ChangeTarget(true);
                                isPlayerOrder = false;
                            }
                            else
                            {
                                ChangeTarget(false);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                )
            );


            //ChangeTarget(false);
            //if (targets.Count == 1 && other.unitStats.Unit == Unit.Plate)
            //    ChangeTarget(true);
            //else if (curTarget != null && other.unitStats.Unit != Unit.Plate)
            //    ChangeTarget(true);

            ChangeTargetDeside(other);
        }
        else
        {
            //print(unitController.gameObject.name + " вижу Свой");
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        UnitController other = collision.GetComponent<UnitController>();
        if (other == null) return;

        if (unitController.IsEnemy(other))
        {
            //print(unitController.gameObject.name + " враг ушёл");

            //curTarget = null;
            targets.Remove(other);
            other.destroyLiseners.RemoveAll(x => x.Equals(unitController));
            ChangeTarget(true);
        }
        else
        {
            //print(unitController.gameObject.name + " Свой ушёл");
        }


            
        
    }

    public void ClearMissedTargets()
    {
        List<UnitController> removeList = new List<UnitController>();

        foreach (var target in targets)
        {
            if (target == null)
                removeList.Add(target);
        }

        foreach (var target in removeList)
        {
            targets.Remove(target);
        }
    }

    private void ChangeTargetDeside(UnitController other)
    {
        ClearMissedTargets();

        if (curTarget == null && other.unitStats.Unit == Unit.Plate)
            ChangeTarget(true);
        else if (curTarget != null && other.unitStats.Unit == Unit.Plate)
            ChangeTarget(false);
        else if (isPlayerOrder == false)
            ChangeTarget(true);
    }

/// <summary>
/// изменение цели
/// </summary>
/// <param name="force">Если требуется обязательно обновить цель(При уничтожении, выходу из зоны или для пересмотрения цели)</param>
    public void ChangeTarget(bool force, UnitController unitController = null)
    {
        if (force) 
            curTarget = null;


        if (curTarget != null)
            return;

        soundManager.StopRotation();

        StopAllCoroutines();
        
        if (targets.Count != 0)
        {
            if (unitController == null) 
            { 
                curTarget = FindTarget(); // тут должен быть модуль принятия решения, какую цель атаковать.
            }
            else
            {
                curTarget = unitController;
                isPlayerOrder = true;
            }

            StartCoroutine(Aiming());
            StartCoroutine(AttackUnit()); 
        }
        else
        {
            if (!randomRotation)
                StartCoroutine(SetZeroDirCoroutine());
        }
    }

    //public void SetZeroDir()
    //{
    //    StartCoroutine(SetZeroDirCoroutine());
    //}
    

    private IEnumerator SetZeroDirCoroutine()
    {
        
        soundManager.PlayRotation();

        while (Quaternion.Angle(zeroQuaternion, transform.localRotation) > 2 && curTarget == null)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, zeroQuaternion, AimSpeed);
            yield return new WaitForEndOfFrame();
        }
        
        soundManager.StopRotation();
    }


    private IEnumerator Aiming()
    {
        if (soundManager.IsAimPlaying == false)
            soundManager.PlayRotation();

        //print("Aiming start");
        while (curTarget != null)
        {
            while (!CanAim)
            {
                yield return new WaitForSeconds(0.1f);
            }
            atGunpoint = LookAt2DUtility.LookAt2D(transform, curTarget.transform.position, AimSpeed);
            yield return new WaitForEndOfFrame();
        }

        soundManager.StopRotation();
        print("Aiming stop");
    }

    private void FixedUpdate()
    {
        curPeriod -= Time.deltaTime;
    }

    private IEnumerator AttackUnit()
    {
        //float z = Quaternion.Angle(curTarget.transform.rotation, unitController.transform.rotation);
        //IBuildable iBuild = (IBuildable)unitController;
        //print("AttackUnit");
        //print("Quaternion.Angle=" + Quaternion.Angle(curTarget.transform.rotation, unitController.transform.rotation));
        int i = 0;
        while (curTarget != null)
        {
            //while (iBuild.IsBuilding == true)
            //{
            //    yield return new WaitForSeconds(0.1f);
            //}

            while (curTarget != null && !CanAttack)
            {
                yield return new WaitForSeconds(0.1f);
            }
            i++;
            //print("Атака " + i + " " + unitController.name + "=>" + curTarget.name);

            while (curPeriod > 0)
            {
                yield return new WaitForSeconds(curPeriod);
            }
            

            GameObject bullet = Instantiate(bulletPrefab, TempObjectsManager.Instance.transform);
            bullet.transform.position = bulletStart.position;


            Vector2 startPosition = bulletStart.position;//* Random.Range(-1, accuracy);
            Vector2 direction = (startPosition - (Vector2)transform.position).normalized;

            direction = Quaternion.AngleAxis(UnityEngine.Random.Range(-deviation, deviation), Vector3.forward) * direction;
            direction.Normalize();

            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.direction = direction;
            bulletController.powerBullet = unitController.PowerBullet;
            bulletController.parentUnitController = unitController;

            GameObject bulletSmoke = Instantiate(bulletStartSmokePrefab, TempObjectsManager.Instance.transform);
            bulletSmoke.transform.position = (Vector2)bulletStart.position + direction * bulletSmokeDistance;
            BulletSmokeController bulletSmokeController = bulletSmoke.GetComponent<BulletSmokeController>();
            bulletSmokeController.direction = direction;

            soundManager.PlayShoot();

            //curTarget.HitUnit(unitController.CurPowerBullet);
            curPeriod = attackPeriod;

            yield return new WaitForSeconds(attackPeriod);
            //speedAttack
            //curTarget
        }

        

    }

    public bool CheckTarget(UnitController unitController)
    {
        if (unitController == null) return false;
        return targets.Exists(x => x.Equals(unitController));
    }


    private UnitController FindTarget()
    {
        if (targets == null) 
            return null;
        //TODO got null ref tower cannon didn't shoot 
        targets.RemoveAll(x => x.CellControllerBase == null);
        var actualTargets = targets.FindAll(x => !ignoreCells.Contains(x.CellControllerBase.position));

        var firstTargets = actualTargets.FindAll(x => x.CurHealthPercent <= minHealthPercentToChangeTarget);

        if (firstTargets.Count > 0)
        {
            firstTargets.Sort((x, y) => y.CurHealthPercent.CompareTo(x.CurHealthPercent));
            return firstTargets[0];
        }

        float minDistance = 999999f;
        UnitController newTarget = actualTargets.Count > 0 ? actualTargets[0] : null;
        foreach (var target in actualTargets)
        {
            //TODO throws nullRef ex
            if (target?.CellControllerBase == null)
                continue;
            if (unitController?.CellControllerBase == null)
                continue;
            //TODO fix null ref
            var distance = Vector2Int.Distance(target.CellControllerBase.position, unitController.CellControllerBase.position);

            if (minDistance > distance)
            {
                minDistance = distance;
                newTarget = target;
            }
        }

        return newTarget;
    }

    public List<CellController> cells2;
    private void FillIgnoreCells()
    {
        List<CellController> cells = new List<CellController>();
        FillIgnoreCells(unitController.CellControllerBase, 1, ref cells);

        cells2 = cells;
        //var vector = unitController.CellControllerBase.position - cells[0].position;
        //Vector2 vector2 = Vector2.one * 3;
        foreach (var cell in cells) {
            bool block = false;

            for (var distance = 0f; distance <= 1f && !block; distance+=0.2f)
            {
                var vector2 = Vector2.Lerp(unitController.CellControllerBase.position, cell.position, distance);
                //var vector2int = new Vector2Int((int)vector2.x, (int)vector2.y);

                if (!block)
                    block = CellsManager.Instance.cellControllersArr[(int)vector2.x, (int)vector2.y].Child?.GetType() == typeof(RockUnitController);
            }
            if (block && !ignoreCells.Contains(cell.position))
                ignoreCells.Add(cell.position);
        }

    }

    private void FillIgnoreCells(CellController cellController, int distance, ref List<CellController> result)
    {
        if (distance >= 4)
            return;

        //Debug.Log("_FillIgnoreCells_");
        //if (cellController?.nearCells == null)
        //    Debug.Log("_nearCells = null_");

        foreach (var cellControllerNear in cellController.nearCells)
        {
            if (!result.Contains(cellControllerNear))
                result.Add(cellControllerNear);
            FillIgnoreCells(cellControllerNear, distance+1, ref result);
        }
    }


    


    //public bool LookAt2D(Vector3 lookTarget)
    //{
    //    Quaternion dir = GetDirection(lookTarget, transform.position);

    //    transform.rotation = Quaternion.RotateTowards(transform.rotation, dir, aimingSpeed);

    //    return Quaternion.Angle(dir, transform.rotation) < 2;
    //}

    //public Quaternion GetDirection(Vector3 lookTarget, Vector3 position)
    //{



    //    // the direction we want the X axis to face (from this object, towards the target)
    //    Vector3 xDirection = (lookTarget - transform.position).normalized;

    //    // Y axis is 90 degrees away from the X axis
    //    Vector3 yDirection = Quaternion.Euler(0, 0, 90) * xDirection;


    //    // Z should stay facing forward for 2D objects
    //    Vector3 zDirection = Vector3.forward;

    //        return Quaternion.LookRotation(zDirection, yDirection);



    //    // apply the rotation to this object
    //    //transform.rotation = Quaternion.LookRotation(zDirection, yDirection);
    //    //return Quaternion.LookRotation(zDirection, yDirection);
    //}



}
