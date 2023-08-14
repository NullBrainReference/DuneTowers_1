using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class SpawnersManager : MonoBehaviour
{
    public float startWaveDelay;
    public float spawnRateBase;

    public float waveCooldown;

    public int wavesCount;
    public int unitsInWave;
    public int unitsInWaveLimit;
    public int unitsInWaveSpawned;

    public bool isSpawningAllowed;

    [SerializeField] private float curTime;
    [SerializeField] private float helicopterChance;

    [SerializeField] private GameObject spawnerPrefab;
    [SerializeField] private GameObject spawnersContainer;

    public List<Spawner> spawners;

    public void Start()
    {
        //InitTimer();
        //InitSpawning();
    }

    private void FixedUpdate()
    {
        curTime -= Time.deltaTime;

        if (curTime <= 0)
        {
            Spawn();
        }

        //if (UnitsCollector.Instance(1).GetAllUnits().Count == 0 && curTime <= 0)
        //{
        //
        //}
    }

    public void Spawn()
    {
        if (isSpawningAllowed == false)
            return;

        if (unitsInWaveSpawned >= unitsInWave)
        {
            unitsInWaveSpawned = 0;

            if (unitsInWave < unitsInWaveLimit)
                unitsInWave++;

            wavesCount++;
            GameStats.Instance.Profile.StatIncrementMax( ProfileStat.Wave ,wavesCount);
            GameStats.Instance.AddEnemyLevel();

            curTime = waveCooldown;

            return;
        }

        ProductionType productionType = GetUnitType();

        foreach (Spawner spawner in spawners)
        {
            if (spawner.IsSpawnerFree)
            {
                spawner.Spawn(productionType);
                unitsInWaveSpawned++;
                curTime = spawnRateBase;

                return;
            }
        }
    }

    private ProductionType GetUnitType()
    {
        ProductionType productionType = ProductionType.Tank;

        float chance = Random.Range(0f, 1f);

        if (helicopterChance >= chance) 
            productionType = ProductionType.Helicopter;

        return productionType;
    }

    public void AddSpawner(Spawner spawner)
    {
        this.spawners.Add(spawner);
    }

    public void InitTimer()
    {
        curTime = startWaveDelay;
    }

    public void InitSpawning()
    {
        isSpawningAllowed = true;
    }

    public void InitInMapManager()
    {
        InitTimer();
        InitSpawning();
    }
}
