using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainObjectsPlacer : MonoBehaviour
{
    public GameObject[] rocks;
    public GameObject[] lakes;

    public GameObject HQ_0;
    public GameObject HQ_1;

    public GameObject tank_0;
    public GameObject tank_1;

    public GameObject helicopter_0;
    public GameObject helicopter_1;

    public GameObject tanker_0;
    public GameObject tanker_1;

    public GameObject fabric_0;
    public GameObject fabric_1;

    public GameObject plate_0;
    public GameObject plate_1;

    public GameObject tower_0;
    public GameObject tower_1;

    public GameObject spawner;

    public GameObject container;
    public GameObject mobsContainer;
    public GameObject spawnersContainer;

    public int objectsCount;
    public int currObjectsCount = 0;

    public int minX;
    public int maxX;

    public int minY;
    public int maxY;

    public List<Vector2Int> avoidPossitions;

    public void PlaceLevelObjects(MapStats map)
    {
        //MapStats map = MapStats.Load(GameStats.Instance.level.id);

        if (map.stats == null) return;

        foreach (var item in map.stats)
        {
            switch (item.cellType)
            {
                case MapCellType.Lake:
                    PlaceItem(lakes[0], true, new Vector2Int(item.x, item.y), container);
                    continue;
                case MapCellType.Rock:
                    PlaceItem(rocks[0], true, new Vector2Int(item.x, item.y), container);
                    continue;
            }

            if (item.hq == HQ_place_status.HQ_0)
            {
                PlaceItem(HQ_0, false, new Vector2Int(item.x, item.y), mobsContainer);
                continue;
            }
            else if (item.hq == HQ_place_status.HQ_1)
            {
                PlaceItem(HQ_1, false, new Vector2Int(item.x, item.y), mobsContainer);
                continue;
            }

            switch (item.unit)
            {
                case Unit.Plate:
                    if(item.playerNo == 0)
                        PlaceItem(plate_0, false, new Vector2Int(item.x, item.y), mobsContainer);
                    else if (item.playerNo == 1)
                        PlaceItem(plate_1, false, new Vector2Int(item.x, item.y), mobsContainer);
                    continue;

                case Unit.Tower:
                    if (item.playerNo == 0)
                        PlaceItem(tower_0, false, new Vector2Int(item.x, item.y), mobsContainer);
                    else if (item.playerNo == 1)
                        PlaceItem(tower_1, false, new Vector2Int(item.x, item.y), mobsContainer);
                    continue;

                case Unit.Fabric:
                    if (item.playerNo == 0)
                        PlaceItem(fabric_0, false, new Vector2Int(item.x, item.y), mobsContainer);
                    else if (item.playerNo == 1)
                        PlaceItem(fabric_1, false, new Vector2Int(item.x, item.y), mobsContainer);
                    continue;

                case Unit.Drill:
                    if (item.playerNo == 0)
                        PlaceItem(tanker_0, false, new Vector2Int(item.x, item.y), mobsContainer);
                    else if (item.playerNo == 1)
                        PlaceItem(tanker_1, false, new Vector2Int(item.x, item.y), mobsContainer);
                    continue;

                case Unit.Tank:
                    if (item.playerNo == 0)
                        PlaceItem(tank_0, false, new Vector2Int(item.x, item.y), mobsContainer);
                    else if (item.playerNo == 1)
                        PlaceItem(tank_1, false, new Vector2Int(item.x, item.y), mobsContainer);
                    continue;

                case Unit.Helicopter:
                    if (item.playerNo == 0)
                        PlaceItem(helicopter_0, false, new Vector2Int(item.x, item.y), mobsContainer);
                    else if (item.playerNo == 1)
                        PlaceItem(helicopter_1, false, new Vector2Int(item.x, item.y), mobsContainer);
                    continue;

                case Unit.Spawner:
                    PlaceItem(spawner, false, new Vector2Int(item.x, item.y), spawnersContainer);
                    continue;
            }
        }
    }

    public void PlaceRandomObjects()
    {
        int rocksCount = Random.Range(1, objectsCount);
        int lakesCount = objectsCount - rocksCount;

        PlaceRocks(rocksCount);
        PlaceLakes(lakesCount);
    }

    public void PlaceRocks(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2Int pos = GetRandomPoss();
            if (avoidPossitions.Contains(pos)) continue;
            
            avoidPossitions.Add(pos);
            PlaceItem(rocks[Random.Range(0, rocks.Length)], true, pos, container);
        }
    }

    public void PlaceLakes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2Int pos = GetRandomPoss();
            if (avoidPossitions.Contains(pos)) continue;

            avoidPossitions.Add(pos);
            PlaceItem(lakes[Random.Range(0, rocks.Length)], true, pos, container);
        }
    }

    private Vector2Int GetRandomPoss()
    {
        int x = Random.Range(minX, maxX);
        int y = Random.Range(minY, maxY);

        return new Vector2Int(x, y);
    }

    private void PlaceItem(GameObject item, bool isTerrain, Vector2Int pos, GameObject container)
    {
        GameObject newItem = Instantiate(item, container.transform);
        UnitController unitController = newItem.GetComponent<UnitController>();

        //UnitsCollector.AcyncAddUnitController(unitController.PlayerNo, unitController);

        GameObject cell = MapManager.Instance.cells[pos.x, pos.y];
        CellController cellController = cell.GetComponent<CellController>();
        if (unitController.unitStats.Unit != Unit.None)
            newItem.transform.rotation = cellController.transform.rotation;

        if (isTerrain)
            cellController.SetTerrainObject(unitController);
        else
            cellController.SetChild2(unitController);

        if (unitController.unitStats.Unit == Unit.Tank || unitController.unitStats.Unit == Unit.Helicopter)
        {
            UnitMover unitMover = newItem.GetComponent<UnitMover>();
            unitMover.atFactory = false;
            unitMover.SetUpNonFactoryMover();
        }

        cellController.Child.transform.localPosition = cellController.transform.position;
    }
}
