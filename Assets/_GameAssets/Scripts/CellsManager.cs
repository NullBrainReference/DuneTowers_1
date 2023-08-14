using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CellsManager : MonoBehaviour
{
    private struct Cell
    {
        public int x;
        public int y;
        public int type;
        public Cell(int x, int y, int type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }

        public Vector2Int Vector2Int
        {
            get {return new Vector2Int(x, y); }
        }
    }


    [System.Serializable]
    public class CellSpriteVariants
    {
        public Sprite[] variants;
        public override string ToString()
        {
            return "cnt=" + variants.Length;
        }
    }

    [System.Serializable]
    public class CellMaterialVariants
    {
        public List<Material> variants;
    }

    public GameObject prefab;

    [Range(0f,1f)]
    public float percentSand;
    [Range(0f,1f)]
    public float percentStone;

    //public List<Sprite> sentSprites;
    [SerializeField]
    private List<CellSpriteVariants> cellSpriteSides;

    [Space]
    [SerializeField]
    private Material materialPrefab;
    //public List<Material> sentMaterials;
    [SerializeField]
    private List<CellMaterialVariants> cellMaterialSides;

    public List<CellController> cellControllers { private set; get; }
    public CellController[,] cellControllersArr { private set; get; }

    public static CellsManager Instance { get; private set; }

    public Vector2Int size;

    private float midX;
    private float midY;

    private void Awake()
    {
        InitMaterials();
        Instance = this;
    }

    private void InitMaterials()
    {
        //foreach(Sprite sprite in sentSprites)
        //{
        //    Material material = new Material(materialPrefab);
        //    material.mainTexture = sprite.texture;
        //    sentMaterials.Add(material);
        //}

        cellMaterialSides = new List<CellMaterialVariants>();
        foreach (CellSpriteVariants cellSpriteVariants in cellSpriteSides)
        {
            CellMaterialVariants cellMaterialVariants = new CellMaterialVariants();
            cellMaterialVariants.variants = new List<Material>();
            cellMaterialSides.Add(cellMaterialVariants);

            foreach (Sprite sprite in cellSpriteVariants.variants)
            {
                
                Material material = new Material(materialPrefab);
                material.mainTexture = sprite.texture;
                cellMaterialVariants.variants.Add(material);
            }
        }

    }

    public GameObject[,] CreateMap(Vector2Int size, MapStats map)
    {

        this.size = size;

        List<Cell> stonesList = CreateStones(size);

        GameObject[,] cells = FillMap(size, stonesList, map);

        FillNearCells(size, cells);



        cellControllers = new List<CellController>();
        cellControllersArr = new CellController[size.x, size.y];

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                CellController cellController = cells[x, y].GetComponent<CellController>();
                cellControllers.Add(cells[x, y].GetComponent<CellController>());
                cellControllersArr[x, y] = cellController;
            }

        return cells;
    }

    public void FillCreditsByStats(MapStats map)
    {
        foreach (CellController cell in cellControllers)
        {
            //cell
        }
    }

    private void FillNearCells(Vector2Int size, GameObject[,] cells)
    {

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                CellController cellController = cells[x, y].GetComponent<CellController>();
                AddNearCells(cellController, cells, size);
            }
    }

    private void AddNearCells(CellController cellController, GameObject[,] cells, Vector2Int size)
    {
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                int x = cellController.position.x + dx;
                int y = cellController.position.y + dy;
                if (InRange(x, 0, size.x-1) && InRange(y, 0, size.y-1) && !(dx == 0 && dy==0))
                {
                    cellController.AddNearCells(cells[x, y]);
                }
            } 
             
    }

    private GameObject[,] FillMap(Vector2Int size, List<Cell> stonesList, MapStats map)
    {
        midX = (float)size.x / 2f - 0.5f;
        midY = (float)size.y / 2f - 0.5f;
        //Debug.Log("midX=" + midX);
        GameObject[,] cells = new GameObject[size.x, size.y];

        //добавляем камни
        foreach (Cell vector2Int in stonesList)
        {
            cells[vector2Int.x, vector2Int.y] = CreateObject(vector2Int, "Stone", map);
        }



        // заполнить все остальное песком
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                if (cells[x, y] == null)
                {
                    cells[x, y] = CreateObject(new Cell(x, y, 0), "Send", map);

                    //GameObject prefab = GetPrefab();
                    //GameObject newObj = Instantiate(prefab, transform);
                    //newObj.transform.position = new Vector2(x - midX, y - midY);
                    //newObj.name = string.Format("{0} ({1},{2})", prefab.name, x, y);
                    //cells[x, y] = newObj;
                }
            }

        return cells;
    }


    

    private Material GetMaterial(int spriteNo)
    {
        return cellMaterialSides[spriteNo].variants[Random.Range(0, cellMaterialSides[spriteNo].variants.Count)];

        //if (spriteNo == 0)
        //{
        //    return sentSprites[Random.Range(0, sentSprites.Count)];
        //}
        //else if (1 <= spriteNo && spriteNo <= 99) // stone
        //{
        //    return stoneSprites[Random.Range(0, stoneSprites.Count)].side[spriteNo-1];
        //}
        //return null;
    }

    private List<Cell> CreateStones(Vector2Int size)
    {
        int countStone = Mathf.RoundToInt(((size.x * size.y)) * percentStone);
        int countStone90 = Mathf.RoundToInt((countStone * 0.9f) == 0 ? 1 : (countStone * 0.9f));
        //int[,] cellsStone = new int[sizeX, sizeY];
        List<Cell> stonesList = new List<Cell>();
        List<Cell> stonesList2 = new List<Cell>();

        // первые 10% распределяем случайно
        while (countStone90 < countStone)
        {
            int x = Random.Range(0, size.x - 1);
            int y = Random.Range(0, size.y - 1);
            stonesList2.Clear();
            if (!stonesList.Exists(i => i.x == x && i.y == y) && !stonesList2.Exists(i => i.x == x && i.y == y))
            {
                stonesList2.Add(new Cell(x, y, 5));
                countStone--;
            }
            stonesList.AddRange(stonesList2);

        }

        //распределяем остальные
        while (countStone > 0)
        {
            stonesList2.Clear();
            foreach (Cell vector3Int in stonesList)
            {
                int x = vector3Int.x + Random.Range(-1, 2);
                int y = vector3Int.y + Random.Range(-1, 2);

                if (0 <= x && x < size.x && 0 <= y && y < size.y) {
                    if ((x + y != 0) && !stonesList.Exists(i => i.x == x && i.y == y) && !stonesList2.Exists(i => i.x == x && i.y == y))
                    {
                        stonesList2.Add(new Cell(x, y, 5));
                        countStone--;
                    }
                }
            }
            stonesList.AddRange(stonesList2);
            //countStone--;
        }


        //Уточнение типа
        int[,] cells = new int[size.x, size.y];
        foreach (Cell vector3Int in stonesList)
        {
            cells[vector3Int.x, vector3Int.y] = vector3Int.type;
        }

        stonesList2.Clear();
        foreach (Cell vector3Int in stonesList)
        {
            bool right = (vector3Int.x + 1) < size.x && cells[vector3Int.x + 1, vector3Int.y] != 0;
            bool left = 0 <= (vector3Int.x - 1) && cells[vector3Int.x - 1, vector3Int.y] != 0;

            bool up = (vector3Int.y + 1) < size.y && cells[vector3Int.x, vector3Int.y+1] != 0;
            bool down = 0 <= (vector3Int.y - 1) && cells[vector3Int.x, vector3Int.y-1] != 0;
          
            stonesList2.Add(new Cell(vector3Int.x, vector3Int.y, GetDirectionNo(right, left, up, down)));
            //cells[vector3Int.x, vector3Int.y] = vector3Int.z;
        }



        return stonesList2;

    }


    private GameObject CreateObject(Cell cell, string name, MapStats map)
    {
        //GameObject prefab = GetPrefab();
        GameObject newObj = Instantiate(prefab, transform);
        newObj.transform.position = new Vector2(cell.x - midX, cell.y - midY);
        newObj.name = string.Format("{0} ({1},{2})", name, cell.x, cell.y);
        CellController cellController = newObj.GetComponent<CellController>();
        Material material = GetMaterial(cell.type);

        CellStats cellStats = new CellStats();

        foreach (CellStats stat in map.stats)
        {
            if (stat.x == cell.Vector2Int.x && stat.y == cell.Vector2Int.y)
            {
                cellStats = stat;
            }  
        }

        cellController.InitCell(cell.Vector2Int, material, cellStats);

        return newObj;
    }


    private int GetDirectionNo( bool right, bool left, bool up, bool down )
    {
        //return 5;
        if (right && left && up && down)
        {
            return 5;
        }
        else if (right && left && up)
        {
            return 2;
        }
        else if (right && left && down)
        {
            return 8;
        }
        else if (right && up && down)
        {
            return 4;
        }
        else if (left && up && down)
        {
            return 6;
        }
        else if (right && up)
        {
            return 1;
        }
        else if (left && up)
        {
            return 3;
        }
        else if (right && down)
        {
            return 7;
        }
        else if (left && down)
        {
            return 9;
        }
        else if (down && up)
        {
            return 10;
        }
        else if (left && right)
        {
            return 11;
        }
        else if (left)
        {
            return 13;
        }
        else if (right)
        {
            return 14;
        }
        else if (down)
        {
            return 15;
        }
        else if (up)
        {
            return 16;
        }
        else
        {
            return 12;
        }
    }

    private bool InRange(int value, int min, int max)
    {
        return min <= value && value <= max;
    }

    public bool InRangeArray(int x, int y)
    {
        return (x >= 0) && (x < size.x) && (y >= 0) && (y < size.y);
    }


#if UNITY_EDITOR
    private void Update()
    {
        percentSand = 1 - percentStone;
    }
#endif


}
