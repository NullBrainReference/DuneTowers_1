using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceController : MonoBehaviour
{
    [SerializeField]
    private int playerNo = 1;

    [SerializeField]
    public bool isProcess;

    [SerializeField]
    private ObstructionsManager obstructionsManager;

    [SerializeField]
    private UnitsCollector playerUnitsCollector;

    [SerializeField]
    private UnitsCollector enemyUnitsCollector;

    [SerializeField]
    //private List<UnitController> units;
    public List<UnitController> Units { get { return playerUnitsCollector.GetAllUnits(); }}

    public List<UnitController> Tanks { get { return playerUnitsCollector.tanks; }}

    public List<UnitController> Helicopters { get { return playerUnitsCollector.helicopters; } }

    public List<UnitController> Buildings { get { return playerUnitsCollector.GetStaticUnits(); } }

    [SerializeField]
    //private List<UnitController> enemyUnits;
    public List<UnitController> EnemyUnits { get {return enemyUnitsCollector.GetAllUnits(); } }

    [SerializeField]
    //private List<UnitController> obstructions;
    public List<UnitController> Obstructions {
        get {
            var result = new List<UnitController>();
            result.AddRange(obstructionsManager.rocks);
            result.AddRange(obstructionsManager.lakes);
            return result;
        } }

    private static Dictionary<int, DeviceController> Instances { get; set; } = new Dictionary<int, DeviceController>();

    //[SerializeField]
    //private static int DeviceCount { get { return 1; } }

    public static DeviceController Instance(int playerNo) { return Instances[playerNo]; }

    public SituationResult situationResultTest;
    //public bool test;

    //private void Update()
    //{
    //    if (test)
    //    {
    //        test = false;
    //        MainProcess();
    //    }
    //}

    

    private void Awake()
    {
        Instances[playerNo] = this;
    }

    private void Start()
    {
        float time = 3f;
        float timeDefence = 10f;
        float timeTanks = 1.5f;
        float timeHelicopters = 3f;
        float timeTanksDefence = 0.5f;

        StopAllCoroutines();

        if (GameStats.Instance.isTowerDefence)
        {
            StartCoroutine(RunProcess(timeDefence));
            StartCoroutine(RunTanksProcess(timeTanksDefence));
            StartCoroutine(RunHelicoptersProcess(timeHelicopters));
        }
        else
        {
            StartCoroutine(RunProcess(time));
            StartCoroutine(RunTanksProcess(timeTanks));
            StartCoroutine(RunHelicoptersProcess(timeHelicopters));
        }
    }

    //public static void AddUnit(UnitController unitController)
    //{

    //    foreach (var instance in Instances.Values)
    //    {
    //        if (unitController.PlayerNo != instance.playerNo)
    //        {
    //            if (unitController.unitType == UnitController.UnitType.Obstruction)
    //            { } //instance.obstructions.Add(unitController);
    //            else
    //                instance.enemyUnits.Add(unitController);
    //        }                
    //        else
    //            instance.units.Add(unitController);
    //    }
    //}

    

    //public static void RemoveUnit(UnitController unitController)
    //{
    //    foreach (var instance in Instances.Values)
    //    {
    //        if (unitController.PlayerNo != instance.playerNo)
    //            instance.enemyUnits.Remove(unitController);
    //        else
    //            instance.units.Remove(unitController);
    //    }
    //}

    public enum SituationResultType {
        Move, //for attack and retreat mobile unit
        Stop, //for factory, tank, turrels
        NewTarget, //for tank, turrels
        Build //for Headquarters
    }

    [System.Serializable]
    public class SituationResult
    {
        public UnitController unitController;
        public SituationResultType situationResultType; 
        public Vector2Int position;  //Build, Move
        public UnitController otherUnitController; // NewTarget
        public float significance;
        public GameObject prefab;
        public bool force;
        public Dictionary<string, object> paramDict = new Dictionary<string, object>();
        public Unit unit;
        public int battlePrice;
    }



    public void MainProcess()
    {
        List<SituationResult> situationResults = AssessmentSituation();

        Applying(situationResults);
    }

    public void TanksProcess()
    {
        List<SituationResult> situationResults = AssessmentSituationTanks();

        Applying(situationResults);
    }

    public void HelicopterProcess()
    {
        List<SituationResult> situationResults = AssessmentSituationHelicopters();

        Applying(situationResults);
    }

    public IEnumerator RunTanksProcess(float time)
    {
        isProcess = true;

        float res = 0;
        int cnt = 0;

        while (isProcess)
        {
            yield return new WaitForSeconds(time);
            System.DateTime dateTime = System.DateTime.Now;
            try
            {
                TanksProcess();
                res += (float)(System.DateTime.Now - dateTime).TotalMilliseconds / 1000f;
                cnt++;
            }
            catch
            {

            }

        }
        print(res / (float)cnt);
    }

    public IEnumerator RunHelicoptersProcess(float time)
    {
        isProcess = true;

        float res = 0;
        int cnt = 0;

        while (isProcess)
        {
            yield return new WaitForSeconds(time);
            System.DateTime dateTime = System.DateTime.Now;
            //try
            //{
                HelicopterProcess();
                res += (float)(System.DateTime.Now - dateTime).TotalMilliseconds / 1000f;
                cnt++;
            //}
            //catch
            //{
            //
            //}

        }
        print(res / (float)cnt);
    }

    public IEnumerator RunProcess(float time)
    {
        isProcess = true;

        float res = 0;
        int cnt = 0;

        while (isProcess)
        {
            yield return new WaitForSeconds(time);
            System.DateTime dateTime = System.DateTime.Now;
            try
            {
                MainProcess();
                res += (float)(System.DateTime.Now - dateTime).TotalMilliseconds / 1000f;
                cnt++;
            }
            catch
            {

            }
            
        }
        print(res / (float)cnt);
    }

    private List<SituationResult> AssessmentSituationTanks()
    {
        List<SituationResult> situationResults = new List<SituationResult>();
        var units = Tanks;
        foreach (UnitController unitController in units)
        {
            //try
            //{
            var situationResult = unitController?.GetSituationResult(units, EnemyUnits);
            if (situationResult != null)
                situationResults.Add(situationResult);
            //}
            //catch(System.Exception e)
            //{
            //    Debug.LogError(e.Message);
            //}
        }
        return situationResults;
    }

    private List<SituationResult> AssessmentSituationHelicopters()
    {
        List<SituationResult> situationResults = new List<SituationResult>();

        Helicopters.RemoveAll(x => x == null);

        var units = Helicopters;

        foreach (UnitController unitController in units)
        {
            var situationResult = unitController?.GetSituationResult(units, EnemyUnits);
            if (situationResult != null)
                situationResults.Add(situationResult);
        }
        return situationResults;
    }

    private List<SituationResult> AssessmentSituation()
    {
        List<SituationResult> situationResults = new List<SituationResult>();
        var units = Buildings;
        foreach(UnitController unitController in units)
        {
            var situationResult = unitController?.GetSituationResult(units, EnemyUnits);
            if (situationResult != null)
                    situationResults.Add(situationResult);
        }
        return situationResults;
    }



    private void Applying(List<SituationResult> situationResults)
    {
        List<SituationResult> results = new List<SituationResult>();
        results.Add(GetSituationResult(situationResults, SituationResultType.Move));
        results.Add(GetSituationResult(situationResults, SituationResultType.Build));
        results.Add(GetSituationResult(situationResults, SituationResultType.NewTarget));
        results.Add(GetSituationResult(situationResults, SituationResultType.Stop));

        SituationResult situationResult = GetSituationResult(results.FindAll(x=>x != null));


        situationResultTest = situationResult;
        if (situationResult == default)
            return;

        situationResult.unitController.ApplyAction(situationResult);
        
    }

    private SituationResult GetSituationResult(List<SituationResult> situationResults, SituationResultType? situationResultType = null)
    {
        if (situationResults == null)
            return default;

        List<SituationResult> results;
        if (situationResultType != null)
            results = situationResults.FindAll(x => x.situationResultType == (SituationResultType)situationResultType);
        else
            results = situationResults;

        results.Sort((x, y) => y.significance.CompareTo(x.significance));
        if (results.Count > 0)
        {
            int max = results.Count > 2 ? 2 : results.Count;
            return results[Random.Range(0, max)];
        }
        return default;
    }

}
