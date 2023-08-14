using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.PlayerSettings;

public class ScanerManager : MonoBehaviour
{
    [SerializeField] private GameObject scanerPrefab;
    [SerializeField] private Transform container;

    [SerializeField] private Button button;

    [SerializeField] private float coolDown;
    private float currTime = 0;

    public List<SelectCellController> scaners;

    public bool IsScanning { get
        {
            if (scaners.Count > 0)
                return true;

            return false;
        } 
    }

    [SerializeField] private float fieldHeight;

    public static ScanerManager Instance;

    private void Awake()
    {
        Instance = this;

        scaners = new List<SelectCellController>();
    }

    private void Update()
    {
        currTime -= Time.deltaTime;

        if (currTime > 0)
            button.interactable = false;
        else
            button.interactable = true;

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (IsScanning)
                RemoveScanField();
            else
                CreateScanField();
        }
    }

    public void HideScanerSlider(Vector2Int pos)
    {
        if (IsScanning == false) return;

        scaners.Find(x => x.CellControllerBase.position == pos)?.HideCreditSlider();
    }

    public void UpdateScaner(CellController cell)
    {
        if (IsScanning == false) return;

        scaners.Find(x => x.CellControllerBase.position == cell.position).InitCreditsValue(cell);
    }

    public void RemoveScaner(Vector2Int pos)
    {
        SelectCellController scaner = scaners.Find(x => x.CellControllerBase.position == pos);

        if (scaner != null)
        {
            scaners.Remove(scaner);
            Destroy(scaner.gameObject);
        }
    }

    public void CreateScaner(CellController cell)
    {
        Vector3 pos = new Vector3(
                cell.transform.position.x,
                cell.transform.position.y,
                cell.transform.position.z + fieldHeight
                );
        GameObject newObj = Instantiate(scanerPrefab, pos, cell.transform.rotation, container);
        SelectCellController selectCell = newObj.GetComponent<SelectCellController>();

        selectCell.CellControllerBase = cell;

        selectCell.InitCreditsValue(cell);

        if (cell.Child != null)
        {
            switch (cell.Child.unitStats.Unit)
            {
                case Unit.None:
                    selectCell.HideCreditSlider();
                    break;
                case Unit.Tower:
                    selectCell.HideCreditSlider();
                    break;
                case Unit.Base:
                    selectCell.HideCreditSlider();
                    break;
                case Unit.Fabric:
                    selectCell.HideCreditSlider();
                    break;
                case Unit.Drill:
                    selectCell.HideCreditSlider();
                    break;
            }
        }

        scaners.Add(selectCell);
    }

    public void CreateScanField()
    {
        if (currTime > 0)
            return;

        SimpleSoundsManager.Instance.PlayClick();

        if (scaners.Count != 0)
        {
            RemoveScanField();
            currTime = coolDown;
            return;
        }

        currTime = coolDown;

        foreach (CellController cell in CellsManager.Instance.cellControllers)
        {
            Vector3 pos = new Vector3(
                cell.transform.position.x,
                cell.transform.position.y,
                cell.transform.position.z + fieldHeight
                );
            GameObject newObj = Instantiate(scanerPrefab, pos, cell.transform.rotation, container);
            SelectCellController selectCell = newObj.GetComponent<SelectCellController>();

            selectCell.CellControllerBase = cell;

            selectCell.InitCreditsValue(cell);

            if (cell.Child != null)
            {
                switch (cell.Child.unitStats.Unit)
                {
                    case Unit.None:
                        selectCell.HideCreditSlider();
                        break;
                    case Unit.Tower:
                        selectCell.HideCreditSlider();
                        break;
                    case Unit.Base:
                        selectCell.HideCreditSlider();
                        break;
                    case Unit.Fabric:
                        selectCell.HideCreditSlider();
                        break;
                    case Unit.Drill:
                        selectCell.HideCreditSlider();
                        break;
                }
            }

            scaners.Add(selectCell);
        }
    }

    public void RemoveScanField()
    {
        foreach (var scaner in scaners)
        {
            Destroy(scaner.gameObject);
        }

        scaners.Clear();
        //Debug.Log("Scanners Removed");
    }
}
