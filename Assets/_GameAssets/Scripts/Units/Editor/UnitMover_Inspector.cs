//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UnitMover), true)]
public class UnitMover_Inspector : Editor
{

    public Vector2Int _target = Vector2Int.zero;
    private string qwe = "qwe";
    public override void OnInspectorGUI()
    {

        var targerClass = (UnitMover)target;
        

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Move") == true)
        {
            targerClass.MoveToCell(_target);
        }
        if (GUILayout.Button("field") == true)
        {


            var obstructions = new List<Vector2Int>();
            obstructions.AddRange(ObstructionsManager.Instance.allPositions);
            obstructions.AddRange(UnitsCollector.Instance(0).GetAllUnitObstructionPositions());
            obstructions.AddRange(UnitsCollector.Instance(1).GetAllUnitObstructionPositions());
            //CourseSearcher courseSearcher = new CourseSearcher(
            //   MapManager.Instance.fieldSize,
            //   targerClass.UnitController.CellControllerBase.position,
            //   targerClass.Target,
            //   obstructions
            //   );

            //CourseSearcher courseSearcher = new CourseSearcher(MapManager.Instance.fieldSize);

            //courseSearcher.FillField(targerClass.UnitController.CellControllerBase.position, targerClass.target, ObstructionsManager.Instance.allPositions);
            //foreach (var cell in courseSearcher.MasToList())
            //{
            //    var textMesh = CellsManager.Instance.cellControllersArr[cell.x, cell.y].textMesh;
            //    textMesh.gameObject.SetActive(true);
            //    textMesh.text = cell.z.ToString();
            //}

        }
        if (GUILayout.Button("way ignore enemy") == true)
        {

            //CourseSearcher courseSearcher = new CourseSearcher(MapManager.Instance.fieldSize);

            //courseSearcher.FillField(targerClass.UnitController.CellControllerBase.position, targerClass.target, ObstructionsManager.Instance.allPositions);
            //var list = targerClass.FillWay(dict, mainEnemyUnitsCollector.bases[0].CellControllerBase.position);


            var way = targerClass.CalcWay(_target, true);
            int i = 0;
            foreach (var vector3 in way)
            {
                var cell = CellsManager.Instance.cellControllersArr[vector3.x, vector3.y];
                cell.textMesh.text = "●" + i;
                i++;
                cell.textMesh.gameObject.SetActive(true);
            }

            //Debug.Log("courseSearcher=" + courseSearcher.GetWay().Count);
            //foreach (var cell in courseSearcher.GetWay())
            //{
            //    Debug.Log("cell=" + cell);
            //    var textMesh = CellsManager.Instance.cellControllersArr[cell.x, cell.y].textMesh;
            //    textMesh.gameObject.SetActive(true);
            //    textMesh.text = "*";
            //}
        }
        if (GUILayout.Button("way not ignore enemy") == true)
        {

            var way = targerClass.CalcWay(_target, false);
            int i = 0;
            foreach (var vector3 in way)
            {
                var cell = CellsManager.Instance.cellControllersArr[vector3.x, vector3.y];
                cell.textMesh.text = "●" + i;
                i++;
                cell.textMesh.gameObject.SetActive(true);
            }

        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        //try
        {
            GUILayout.Label("Target");
            var target = targerClass.CurTarget?.target;
            int x = target?.x ?? _target.x;
            int y = target?.y ?? _target.y;

            x = int.Parse(GUILayout.TextField($"{x}", 25));
            y = int.Parse(GUILayout.TextField($"{y}", 25));
            _target.x = x;
            _target.y = y;

        }
        //catch { }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        base.OnInspectorGUI();
        serializedObject.Update();
    }


}
