using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;

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

    public int X { get { return x; } }

    public static MapGenerator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mapStats = new MapStats();

        xText.text = 16.ToString();
        yText.text = 10.ToString();

        levelText.text = 1.ToString();
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
                rectTransform.position = new Vector2(i * 120, j * 120);
            }
        }
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
                rectTransform.position = new Vector2(i * 120, j * 120);
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
    }

    public void SelectHQ_1()
    {
        hq = HQ_place_status.HQ_1;
        currUnit = Unit.None;
    }

    public void dropHQ()
    {
        hq = HQ_place_status.None;
        currUnit = Unit.None;
    }

    public void SelectTanker_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Drill;
        playerNo = 0;
    }

    public void SelectTanker_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Drill;
        playerNo = 1;
    }

    public void SelectTank_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Tank;
        playerNo = 0;
    }

    public void SelectTank_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Tank;
        playerNo = 1;
    }

    public void SelectHeli_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Helicopter;
        playerNo = 0;
    }

    public void SelectHeli_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Helicopter;
        playerNo = 1;
    }

    public void SelectFactory_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Fabric;
        playerNo = 0;
    }

    public void SelectFactory_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Fabric;
        playerNo = 1;
    }

    public void SelectPlate_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Plate;
        playerNo = 0;
    }

    public void SelectPlate_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Plate;
        playerNo = 1;
    }

    public void SelectTower_0()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Tower;
        playerNo = 0;
    }

    public void SelectTower_1()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Tower;
        playerNo = 1;
    }

    public void SelectSpawner()
    {
        hq = HQ_place_status.None;

        currUnit = Unit.Spawner;
        playerNo = 1;
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

    public void DropCCEdit()
    {
        isCCEdit = false;
    }

    public void SaveMap()
    {
        //textAsset = new TextAsset();
        mapStats.lvl = lvl;
        mapStats.SetStats(mapCells);

        mapStats.SaveMap();
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
