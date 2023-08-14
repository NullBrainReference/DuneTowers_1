using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsFieldManager: MonoBehaviour
{
    public void GenerateFlatMap()
    {
        foreach (MapCell cell in MapGenerator.Instance.mapCells)
            cell.EditCC(MapGenerator.Instance.cc_count);
    }

    public void GenerateNormalXMap()
    {
        foreach (MapCell cell in MapGenerator.Instance.mapCells)
        {
            int centerX = (MapGenerator.Instance.X / 2);
            int edValue = 0;
            if (cell.x <= centerX)
            {
                float mp = (float)cell.x / centerX;
                edValue = (int)(
                    (float)MapGenerator.Instance.cc_count * 0.5f +
                    (float)MapGenerator.Instance.cc_count * 0.5f * mp
                    );
                //Debug.Log(mp.ToString("0.00"));
            }
            else
            {
                float mp = Mathf.Abs((float)(MapGenerator.Instance.X - (float)cell.x) / centerX);
                edValue = (int)(
                    (float)MapGenerator.Instance.cc_count * 0.5f +
                    (float)MapGenerator.Instance.cc_count * 0.5f * mp
                    );
                //Debug.Log(mp.ToString("0.00 else part"));
            }
            cell.EditCC(edValue);
        }
    }

    public void GenerateRandomMap()
    {
        int[] stages = { 0, 1000, 2000, 3000 };
        float[] chances = { 70, 15, 10, 5 };

        float sumChance = 0;
        foreach (int ch in chances)
        {
            sumChance += ch;
        }

        foreach (MapCell cell in MapGenerator.Instance.mapCells)
        {
            float rnd = Random.Range(0, sumChance);
            float chance = 0;

            for (int i = 0; i < chances.Length; i++)
            {
                chance += chances[i];

                if (chance >= rnd)
                {
                    cell.EditCC(stages[i]);
                    break;
                }
            }
        }
            
    }

    public void GenerateCentralizedMap()
    {

    }
}
