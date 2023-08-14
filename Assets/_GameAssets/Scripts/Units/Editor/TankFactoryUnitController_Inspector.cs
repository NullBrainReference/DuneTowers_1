using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TankFactoryUnitController), true)]
public class TankFactoryUnitController_Inspector : Editor
{
    public override void OnInspectorGUI()
    {

        var targerClass = (TankFactoryUnitController)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear") == true)
        {
            //var list = targerClass.FillWay();

            foreach (var cell in CellsManager.Instance.cellControllers)
            {
                cell.textMesh.text = "";
                cell.textMesh.gameObject.SetActive(true);
            }
        }

        if (GUILayout.Button("matrix") == true)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("otherWays", new List<Vector2Int>());
            var mainEnemyUnitsCollector = UnitsCollector.Instance(1).mainEnemyUnitsCollector;

            var list = targerClass.FillWay(dict, mainEnemyUnitsCollector.bases[0].CellControllerBase.position);

            foreach (var vector3 in list)
            {
                var cell = CellsManager.Instance.cellControllersArr[vector3.x, vector3.y];
                cell.textMesh.text += "\n" + vector3.z.ToString();
                cell.textMesh.gameObject.SetActive(true);
            }
        }

        if (GUILayout.Button("Way") == true)
        {
            //var list = targerClass.FillWay();

            //foreach (var vector3 in list)
            //{
            //    var cell = CellsManager.Instance.cellControllersArr[vector3.x, vector3.y];
            //    cell.textMesh.text = vector3.z.ToString();
            //    cell.textMesh.gameObject.SetActive(true);
            //}


            foreach (var vector3 in targerClass.Way)
            {
                var cell = CellsManager.Instance.cellControllersArr[vector3.x, vector3.y];
                cell.textMesh.text = "●";
                cell.textMesh.gameObject.SetActive(true);
            }

        }

        //if (GUILayout.Button("recalc") == true)
        //{
        //    //CourseSearcher courseSearcher = new CourseSearcher(MapManager.Instance.fieldSize, targerClass.CellControllerBase.position, Vector2Int.zero, TestGetPlayerStationaryPositions());

        //    //foreach (var vector3 in courseSearcher.MasToList())
        //    //{
        //    //    var cell = CellsManager.Instance.cellControllersArr[vector3.x, vector3.y];
        //    //    cell.textMesh.text = vector3.z.ToString();
        //    //    cell.textMesh.gameObject.SetActive(true);
        //    //}

        //    //var way = courseSearcher.GetWay();

        //    targerClass.FillWay();
        //    //foreach (var vector3 in targerClass.Way)
        //    //{
        //    //    var cell = CellsManager.Instance.cellControllersArr[vector3.x, vector3.y];
        //    //    cell.textMesh.text = "X";
        //    //    cell.textMesh.gameObject.SetActive(true);
        //    //}

        //}



        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        base.OnInspectorGUI();
        serializedObject.Update();
    }


    //private List<Vector2Int> TestGetPlayerStationaryPositions()
    //{
    //    List<CellController> playerStationaryCells = CellsManager.Instance.cellControllers.FindAll(x => x.PlayerStationaryUnit(1));

    //    List<Vector2Int> playerStationaryPositions = new List<Vector2Int>();
    //    foreach (CellController cellController in playerStationaryCells)
    //        playerStationaryPositions.Add(cellController.position);
    //    return playerStationaryPositions;
    //}
}
