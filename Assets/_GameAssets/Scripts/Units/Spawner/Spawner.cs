using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class Spawner : TankFactoryUnitController
{
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject helicopterPrefab;

    private UnitController curUnitController;

    [SerializeField] private float curTime;
    [SerializeField] private float coolDown;

    private bool factoryFree;

    public bool IsSpawnerFree { 
        get {
            if (CellControllerBase.Child != this)
                return false;
            if (curTime > 0)
                return false;

            return true;
        } 
    }

    public void Spawn(ProductionType productionType)
    {
        curTime = coolDown;

        GameObject unitObj;

        if (productionType == ProductionType.Tank)
            unitObj = Instantiate(tankPrefab, CellControllerBase.transform.position, CellControllerBase.transform.rotation, UnitManager.Instance.transform);
        else
            unitObj = Instantiate(helicopterPrefab, CellControllerBase.transform.position, CellControllerBase.transform.rotation, UnitManager.Instance.transform);

        curUnitController = unitObj.GetComponent<UnitController>();

        curUnitController.SetUnitControllerFactory(this);
        this.CellControllerBase.SetFabricChild(curUnitController);

        factoryFree = false;

        StartCoroutine(MoveToDefaultCell());
    }

    private void FixedUpdate()
    {
        curTime -= Time.deltaTime;
    }

    private IEnumerator MoveToDefaultCell()
    {
        IMoveToDefaultCell newUnitIMoveToDefaultCell = ((IMoveToDefaultCell)curUnitController);
        IDefaultPosition unitIDefaultPosition = ((IDefaultPosition)this);

        CellController defaultPosition = null;
        while (defaultPosition == null)
        {
            defaultPosition = unitIDefaultPosition.DefaultPosition;
            if (defaultPosition != null)
            {
                newUnitIMoveToDefaultCell.MoveToDefaultCell(
                    this.CellControllerBase,
                    unitIDefaultPosition.DefaultPosition,
                    () => {
                        factoryFree = true;
                        newUnitIMoveToDefaultCell.SetNoAtFactory();
                        curUnitController.gameObject.layer = LayerMask.NameToLayer("MobileUnit");
                    }
                    );
            }
            yield return new WaitForSeconds(0.5f);

        }
    }
}
