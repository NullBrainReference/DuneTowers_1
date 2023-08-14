using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject[,] cells;

    public Vector2Int fieldSize;

    public TerrainObjectsPlacer objectsPlacer;

    public SpawnersManager spawnersManager;

    public static MapManager Instance { get; private set; }

    public bool Initialazed { private set; get; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        MapStats map = MapStats.Load(GameStats.Instance.level.id);

        fieldSize = new Vector2Int(map.xSize, map.ySize);

        cells = CellsManager.Instance.CreateMap(fieldSize, map);

        objectsPlacer.PlaceLevelObjects(map);

        if (GameStats.Instance.isTowerDefence)
            spawnersManager.InitInMapManager();

        Initialazed = true;
    }

    public static void CreateUnit(GameObject prefab, Vector2Int position)
    {
        GameObject unit = Instantiate(prefab, UnitManager.Instance.transform);
        UnitController unitController = unit.GetComponent<UnitController>();

        GameObject cell = Instance.cells[position.x, position.y];
        CellController cellController = cell.GetComponent<CellController>();

        cellController.SetChild2(unitController);
        cellController.Child.transform.localPosition = cellController.transform.position;
    }
}
