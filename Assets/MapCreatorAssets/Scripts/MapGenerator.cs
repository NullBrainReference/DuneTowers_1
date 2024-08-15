using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum HQ_place_status {None, HQ_0, HQ_1} 
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject content;

    [SerializeField] private TMP_InputField xText;
    [SerializeField] private TMP_InputField yText;
    [SerializeField] private TMP_InputField levelText;

    [SerializeField] private TMP_InputField ccText;

    [SerializeField] private CanvasScaler scaler;

    public List<MapCell> mapCells;

    public MapStats mapStats;

    public TextAsset textAsset;

    private int x = 0;
    private int y = 0;

    public int lvl = 0;

    public HQ_place_status hq = HQ_place_status.None;
    public Unit currUnit = Unit.None;
    public int playerNo = 0;

    public bool isCCEdit = false;
    public int cc_count = 0;

    public List<SelectableButton> selectableButtons = new List<SelectableButton>();

    public int X { get { return x; } }

    public static MapGenerator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mapStats = new MapStats();

        //xText.text = 16.ToString();
        //yText.text = 10.ToString();

        //levelText.text = 1.ToString();

        LoadUserMap();

        if (mapStats.xSize <= 1)
            CreateField16x10();
    }

    public void CreateField()
    {
        ClearCells();

        try
        {
            x = Convert.ToInt32(xText.text);
            y = Convert.ToInt32(yText.text);

            lvl = Convert.ToInt32(levelText.text);

            mapStats.xSize = x;
            mapStats.ySize = y;
        }
        catch
        {

        }

        CreateField(x, y);
    }

    private void CreateField(int x, int y)
    {
        ClearCells();

        this.x = x;
        this.y = y;

        mapStats.xSize = x;
        mapStats.ySize = y;

        Debug.Log("Scale factor = " + scaler.scaleFactor);

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject newCell = Instantiate(cellPrefab, content.transform);
                MapCell mapCell = newCell.GetComponent<MapCell>();
                mapCell.x = i;
                mapCell.y = j;

                mapCells.Add(mapCell);

                RectTransform rectTransform = newCell.GetComponent<RectTransform>();

                float yScale = scaler.referenceResolution.y / Screen.height;

                rectTransform.localScale = new Vector2(yScale, yScale);

                rectTransform.position = new Vector2(
                    i * rectTransform.sizeDelta.x, //+ xDifference * i,
                    j * rectTransform.sizeDelta.y //+ xDifference * j
                    );
            }
        }
    }

    public void CreateField16x10()
    {
        CreateField(16, 10);
    }

    public void CreateField20x8()
    {
        CreateField(20, 8);
    }

    public void RecreateField()
    {
        ClearCells();

        x = mapStats.xSize;
        y = mapStats.ySize;
        lvl = mapStats.lvl;

        levelText.text = lvl.ToString();

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject newCell = Instantiate(cellPrefab, content.transform);
                MapCell mapCell = newCell.GetComponent<MapCell>();

                foreach (var item in mapStats.stats)
                    if (item.x == i && item.y == j)
                    {
                        mapCell.cellStats = item;
                        mapCell.FillCellbyStats();
                        break;
                    }

                mapCells.Add(mapCell);

                RectTransform rectTransform = newCell.GetComponent<RectTransform>();

                float yScale = scaler.referenceResolution.y / Screen.height;
                rectTransform.localScale = new Vector2(yScale, yScale);

                rectTransform.position = new Vector2(
                    i * rectTransform.sizeDelta.x,
                    j * rectTransform.sizeDelta.y
                    );
            }
        }
    }

    public void ClearCells()
    {
        foreach (var item in mapCells)
        {
            Destroy(item.gameObject);
        }

        mapCells.Clear();
    }

    public void SelectHQ_0() 
    {
        hq = HQ_place_status.HQ_0;
        currUnit = Unit.None;

        DropCCEdit();
    }

    public void SelectHQ_1()
    {
        hq = HQ_place_status.HQ_1;
        currUnit = Unit.None;

        DropCCEdit();
    }

    public void dropHQ()
    {
        hq = HQ_place_status.None;
        currUnit = Unit.None;

        DropCCEdit();

        DropSelects();
    }

    public void SelectTanker_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Drill;
        playerNo = 0;

        DropCCEdit();
    }

    public void SelectTanker_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Drill;
        playerNo = 1;

        DropCCEdit();
    }

    public void SelectTank_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Tank;
        playerNo = 0;

        DropCCEdit();
    }

    public void SelectTank_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Tank;
        playerNo = 1;

        DropCCEdit();
    }

    public void SelectHeli_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Helicopter;
        playerNo = 0;

        DropCCEdit();
    }

    public void SelectHeli_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Helicopter;
        playerNo = 1;

        DropCCEdit();
    }

    public void SelectFactory_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Fabric;
        playerNo = 0;

        DropCCEdit();
    }

    public void SelectFactory_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Fabric;
        playerNo = 1;

        DropCCEdit();
    }

    public void SelectPlate_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Plate;
        playerNo = 0;

        DropCCEdit();
    }

    public void SelectPlate_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Plate;
        playerNo = 1;

        DropCCEdit();
    }

    public void SelectTower_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Tower;
        playerNo = 0;

        DropCCEdit();
    }

    public void SelectTower_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Tower;
        playerNo = 1;

        DropCCEdit();
    }

    public void SelectSpawner()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Spawner;
        playerNo = 1;

        DropCCEdit();
    }

    public void SelectCCEdit()
    {
        isCCEdit = true;
        try
        {
            cc_count = Convert.ToInt32(ccText.text);
        }
        catch
        {
            Debug.Log("cc_failed");
            cc_count = 1000;
        }
    }

    public void SelectCCEdit_1000()
    {
        isCCEdit = true;
        cc_count = 0;
    }

    public void SelectCCEdit_2000()
    {
        isCCEdit = true;
        cc_count = 1000;
    }

    public void SelectCCEdit_3000()
    {
        isCCEdit = true;
        cc_count = 2000;
    }

    public void SelectCCEdit_5000()
    {
        isCCEdit = true;
        cc_count = 3000;
    }

    public void SelectCCEdit_7000()
    {
        isCCEdit = true;
        cc_count = 5000;
    }

    public void SelectCCEdit_12000()
    {
        isCCEdit = true;
        cc_count = 8000;
    }

    public void DropCCEdit()
    {
        isCCEdit = false;
    }

    public void DropSelects()
    {
        foreach (var item in selectableButtons)
        {
            item.DropSelect();
        }
    }

    public void SaveMap()
    {
        //textAsset = new TextAsset();
        mapStats.lvl = lvl;
        mapStats.SetStats(mapCells);

        mapStats.SaveMap();
    }

    public void SaveUserMap()
    {
        mapStats.lvl = GameStats.Instance.userMapId;
        mapStats.SetStats(mapCells);

        mapStats.SaveUserMap();

        SceneManager.LoadScene("HorisontalMenu", LoadSceneMode.Single);
    }

    public void LoadUserMap()
    {
        string map = PlayerPrefs.GetString("UserMap_" + GameStats.Instance.userMapId);

        if (map == "") return;

        mapStats = JsonUtility.FromJson<MapStats>(map);

        RecreateField();
    }

#if UNITY_EDITOR
    public void LoadMap()
    {
        //TODO WebGL fails EditorUtility replace on build

        string fileContent = "";
        
        string path = EditorUtility.OpenFilePanel("Choose level", "", "txt");
        if (path.Length != 0)
        {
            fileContent = File.ReadAllText(path);
            mapStats = JsonUtility.FromJson<MapStats>(fileContent);
            RecreateField();
        }
    }
#endif
}
