using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObjects : MonoBehaviour
{
    [System.Serializable]
    public struct SetPos
    {
        public Vector2Int position;
        public UnitController unitController;

        public SetPos(Vector2Int position, UnitController unitController)
        {
            this.position = position;
            this.unitController = unitController;
        }

    }

    public List<SetPos> sets;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(UpdateObjectsIEnumerator());
    }

    public void TestUpdateObjects()
    {
        foreach (SetPos setPos in sets)
        {
            MapManager mapManager = FindObjectOfType<MapManager>();
            
            setPos.unitController.transform.position = new Vector3(setPos.position.x - mapManager.fieldSize.x / 2, setPos.position.y - mapManager.fieldSize.y / 2, 0);
        }
    }

    private IEnumerator UpdateObjectsIEnumerator()
    {
        yield return new WaitForSeconds(0f);
        while (MapManager.Instance.cells == null) {
            yield return  new WaitForSeconds(0.1f);
        }

        foreach (SetPos setPos in sets) {
            UnitMover unitMover = setPos.unitController?.GetComponent<UnitMover>();

            GameObject cell = MapManager.Instance.cells[setPos.position.x, setPos.position.y];
            CellController cellController = cell.GetComponent<CellController>();
            if (unitMover != null)
            {
                unitMover.UnitController.CellControllerBase = cellController;
                unitMover.transform.localPosition = unitMover.UnitController.CellControllerBase.transform.position;
                unitMover.UnitController.CellControllerBase.SetChild2(setPos.unitController);
            }
            else
            {
                cellController.SetChild2(setPos.unitController);
                cellController.Child.transform.localPosition = cellController.transform.position;
            }
        }
    }


}
