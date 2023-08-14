using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseSearcher
{

    public class CellSercher
    {
        public int x;
        public int y;
        public int z;
        public CellSercher(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }



    public const int maxDistance = 99999;
    public int[,] Mas { private set; get; }
    private bool[,] mas2;
    private Vector2Int size;
    private Vector2Int startPoint;
    private Vector2Int endPoint;
    private List<Vector2Int> blocks;
    private bool useDiagonal = true;
    List<Vector2Int>? otherWays;

    private List<Vector2Int> dNeighbors = new List<Vector2Int>();
    private List<Vector2Int> dNeighborsWithoutDiagonal = new List<Vector2Int>();

    public CourseSearcher(Vector2Int size)
    {
        InitCourseSearcher(size);
    }

    public CourseSearcher(Vector2Int size, Vector2Int startPoint, Vector2Int endPoint, List<Vector2Int> blocks, bool useDiagonal = true, List<Vector2Int>? otherWays = null)
    {
        InitCourseSearcher(size);
        //FillField(startPoint, endPoint, blocks);
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.blocks = blocks;
        this.useDiagonal = useDiagonal;
        this.otherWays = otherWays;
        FillField(startPoint, endPoint, blocks);
    }

    public CourseSearcher(Vector2Int size, Vector2Int startPoint, List<Vector2Int> blocks, bool useDiagonal = true)
    {
        InitCourseSearcher(size);
        this.startPoint = startPoint;
        this.blocks = blocks;
        this.useDiagonal = useDiagonal;
        FillField(startPoint, null, blocks);
    }

    private void InitCourseSearcher(Vector2Int size)
    {
        this.size = size;

        Mas = new int[size.x, size.y];
        mas2 = new bool[size.x, size.y];

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Mas[x, y] = -1;
            }




        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
                if (dx != 0 || dy != 0)
                {
                    dNeighbors.Add(new Vector2Int(dx, dy));
                    if (MathF.Abs(dx) + MathF.Abs(dy) == 1)
                        dNeighborsWithoutDiagonal.Add(new Vector2Int(dx, dy));
                }
        dNeighbors.Sort((a, b) => (MathF.Abs(a.x) + MathF.Abs(a.y)).CompareTo(MathF.Abs(b.x) + MathF.Abs(b.y)));


    }


    public Vector2Int GetNextStep(Vector2Int startPoint, Vector2Int? _endPoint, List<Vector2Int> _blocks = null)
    {
        var nearOtherWay = GetNearOtherWay(startPoint);

        if (nearOtherWay != null)
            return (Vector2Int)nearOtherWay;

        Vector2Int nextStep;

        if (_endPoint != null)
            endPoint = (Vector2Int)_endPoint;

        if (_blocks != null)
        {
            blocks = _blocks;
            FillField(startPoint, endPoint, _blocks);
        }


        if (CheckNearEndPoint(startPoint, endPoint))
        {
            nextStep = endPoint;

            //FillValuesForTest(startPoint, endPoint, 0, nextStep);
        }
        else
        {
            int minDist = GetNearMinDist(endPoint.x, endPoint.y);
            FindMinDist(endPoint.x, endPoint.y, minDist);
            nextStep = GetNextPos(startPoint);

            //FillValuesForTest(startPoint, endPoint, minDist, nextStep);
        }
        

        return blocks.Exists(n => n.x == nextStep.x && n.y == nextStep.y) ? startPoint : nextStep;

    }

    public void FillField(Vector2Int startPoint, Vector2Int? endPoint, List<Vector2Int> blocks)
    {
        foreach (Vector2Int block in blocks)
        {
            Mas[block.x, block.y] = -2;
        }
        Mas[startPoint.x, startPoint.y] = -3;
        if (endPoint != null)
            Mas[((Vector2Int)endPoint).x, ((Vector2Int)endPoint).y] = -4;


        FillNearFields(startPoint.x, startPoint.y, 0);
    }

    private bool CheckNearEndPoint(Vector2Int startPoint, Vector2Int endPoint)
    {

        Vector2Int dVector = startPoint - endPoint;
        if (!(Mathf.Abs(dVector.x) <= 1 && Mathf.Abs(dVector.y) <= 1))
            return false;

        if ((Mathf.Abs(dVector.x) + Mathf.Abs(dVector.y)) == 1)
        {
            return true;
        }
        else // диагональ
        {
            return CheckField(startPoint.x, startPoint.y, endPoint.x, endPoint.y);
            
        }
    }

    

    public void FillNearFields(int x, int y, int dist)
    {
        if (InRange(x, y))
        {
            if ((Mas[x, y] == -1) || (Mas[x, y] > dist) || (dist == 0))
            {
                Mas[x, y] = dist;
                for (int x1 = -1; x1 <= 1; x1++)
                    for (int y1 = -1; y1 <= 1; y1++)
                    {
                        bool fillNearField = useDiagonal ?
                            (x1 == 0 || y1 == 0) || CheckField(x, y, x + x1, y + y1)
                            : (x1 == 0 || y1 == 0);

                        if (fillNearField)
                            FillNearFields(x + x1, y + y1, dist + 1);
                    }
            }
        }
    }

    public bool InRange(Vector2Int pos)
    {
        return InRange(pos.x, pos.y);
    }

    private bool InRange(int x, int y)
    {
        return (x >= 0) && (x < size.x) && (y >= 0) && (y < size.y);
    }

    private bool CheckField(int x0, int y0, int x, int y)
    {
        //if (!useDiagonal)
        //    return false;

        bool b1 = InRange(x, y0) ? Mas[x, y0] != -2 : false;
        bool b2 = InRange(x0, y) ? Mas[x0, y] != -2 : false;

        return b1 && b2;
    }
    
    private void FindMinDist(int x0, int y0, int distValue)
    {

        if (InRange(x0, y0) && distValue > 0)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    int x = x0 + dx;
                    int y = y0 + dy;
                    if (InRange(x, y))
                        if (!(dx == 0 && dy == 0) && (Mas[x, y] == distValue) && (!mas2[x, y]) && CheckField(x0, y0, x, y))
                        {
                            mas2[x, y] = true;
                            FindMinDist(x, y, distValue - 1);
                        }
                }
        }

    }

    public int GetNearMinDist(int x, int y)
    {
        int result = maxDistance;
        for (int x1 = -1; x1 <= 1; x1++)
            for (int y1 = -1; y1 <= 1; y1++)
                if (!(x1 == 0 && y1 == 0))
                    if (InRange(x + x1, y + y1)) {
                        if (result > Mas[x + x1, y + y1] && Mas[x + x1, y + y1] > 0 && CheckField(x, y, x + x1, y + y1))
                        {
                            result = Mas[x + x1, y + y1];
                        }
                    }
        return result;
    }

    private Vector2Int GetNextPos(Vector2Int pos)
    {
        Vector2Int result = pos;
    
        for (int x1 = -1; x1 <= 1; x1++)
            for (int y1 = -1; y1 <= 1; y1++)
                if (Mathf.Abs(x1) + Mathf.Abs(y1) == 1)
                    if (InRange(pos.x + x1, pos.y + y1) && CheckField(pos.x, pos.y, pos.x + x1, pos.y + y1)) 
                        if (mas2[pos.x + x1, pos.y + y1])
                        {
                            return new Vector2Int(pos.x + x1, pos.y + y1);
                        }

        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
                //if (!(x1 == 0 && y1 == 0))
                if (Mathf.Abs(dx) + Mathf.Abs(dy) == 2)
                    if (InRange(pos.x + dx, pos.y + dy) && CheckField(pos.x, pos.y, pos.x + dx, pos.y + dy))
                        if (mas2[pos.x + dx, pos.y + dy])
                        {
                            return new Vector2Int(pos.x + dx, pos.y + dy);
                        }


        return result;
    }

    private bool IsDiagonal(int dx, int dy)
    {
        return (MathF.Abs(dx) + MathF.Abs(dy)) == 2;
    }


    public List<Vector3Int> MasToList()
    {
        var result = new List<Vector3Int>();
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                result.Add(new Vector3Int(x,y,Mas[x,y]));

        return result;

    }

    public List<Vector2Int> Mas2ToList()
    {
        var result = new List<Vector2Int>();
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (mas2[x,y])
                    result.Add(new Vector2Int(x, y));

        return result;

    }


    public List<Vector2Int> GetWay()
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int min = 99999;
        foreach(var dV in dNeighbors) {
                Vector2Int newPos = new Vector2Int(endPoint.x + dV.x, endPoint.y + dV.y);
                if (InRange(newPos) && Mas[newPos.x, newPos.y] > 0)
                    if (min > Mas[newPos.x, newPos.y] && CheckdNeighborsWithoutDiagonal(newPos, dV))
                        min = Mas[newPos.x, newPos.y];
        }

        GetWay_Find(endPoint, min, ref result);
        result.Reverse();

        result.Add(endPoint);

        return result;
    }

    private bool CheckdNeighborsWithoutDiagonal(Vector2Int newPos, Vector2Int dV)
    {
        if (MathF.Abs(dV.x) + MathF.Abs(dV.y) != 2)
            return true;


        if (dV.x < 0 && dV.y < 0)
            return Mas[newPos.x + 1, newPos.y] > 0 && Mas[newPos.x, newPos.y + 1] > 0;

        if (dV.x > 0 && dV.y > 0)
            return Mas[newPos.x - 1, newPos.y] > 0 && Mas[newPos.x, newPos.y - 1] > 0;

        if (dV.x < 0 && dV.y > 0)
            return Mas[newPos.x , newPos.y - 1] > 0 && Mas[newPos.x + 1, newPos.y] > 0;

        if (dV.x > 0 && dV.y < 0)
            return Mas[newPos.x , newPos.y + 1] > 0 && Mas[newPos.x - 1, newPos.y] > 0;

        return true;
    }

    public void GetWay_Find(Vector2Int curPos, int curValue, ref List<Vector2Int> result)
    {
        if (startPoint == curPos || curValue > (size.x + size.y))
            return;

        foreach (var dV in dNeighbors)
        {
            
            Vector2Int newPos = new Vector2Int(curPos.x + dV.x, curPos.y + dV.y);
            if (InRange(newPos))
                if(Mas[newPos.x, newPos.y] == curValue)
                {
                    result.Add(newPos);
                    GetWay_Find(newPos, curValue - 1, ref result);
                    return;
                }
        }
    }



    private Vector2Int? GetNearOtherWay(Vector2Int pos)
    {
        if (otherWays == null)
            return default;

        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
                if ((dx == 0 || dy == 0)) {
                    var dPos = new Vector2Int(pos.x + dx, pos.y + dy);
                    if (InRange(dPos))
                        if (otherWays.Contains(dPos))
                            return dPos;
                }

        return default;
    }



}
