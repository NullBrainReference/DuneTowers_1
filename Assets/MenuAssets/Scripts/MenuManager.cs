using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private PlayerStats playerStats;
    public PlayerStats PlayerStats { get { return playerStats; } }

    [SerializeField] private ProfileUI profileUI;
    public ProfileUI ProfileUI { get { return profileUI; } }

    public List<GameObject> gameObjects;
    public List<IStatsUi> objects;

    public UnitsScroll unitsScroll;

    [SerializeField] private int tankPrice;
    [SerializeField] private int helicopterPrice;
    [SerializeField] private int basePrice;
    [SerializeField] private int fabricPrice;
    [SerializeField] private int drillPrice;
    [SerializeField] private int towerPrice;
    [SerializeField] private int platePrice;

    [Space]

    [SerializeField] private int tankHealth;
    [SerializeField] private int helicopterHealth;
    [SerializeField] private int baseHealth;
    [SerializeField] private int fabricHealth;
    [SerializeField] private int drillHealth;
    [SerializeField] private int towerHealth;
    [SerializeField] private int plateHealth;

    [SerializeField] private TopPanelController topPanelController;

    [SerializeField] private MenuAttentionManager menuAttentionManager;

    public TopPanelController TopPanelController { get { return topPanelController; } }

    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        objects = new List<IStatsUi>();

        LoadOrInitPlayer();

        foreach (GameObject go in gameObjects)
        {
            IStatsUi ui = go.GetComponent<IStatsUi>();
            objects.Add(ui);
        }

        unitsScroll.InitScroll();
    }

    private void Start()
    {
        playerStats.CountSimpleStats();

        AchievementsManager.Instance.InitAchievements();
    }

    public void UiStatsUpdate()
    {
        foreach(IStatsUi statsUi in objects)
        {
            statsUi.UiUpdate();
        }

        menuAttentionManager.UpdateAttentions();
    }

    public void UiUpdateByTag(string tag)
    {
        foreach (IStatsUi statsUi in objects)
        {
            if (tag == statsUi.Tag)
                statsUi.UiUpdate();
        }

        menuAttentionManager.UpdateAttentions();
    }

    public void UiUpdateByTag(List<string> tags)
    {
        foreach (IStatsUi statsUi in objects)
        {
            if(tags.Contains(statsUi.Tag))
                statsUi.UiUpdate();
        }
    }

    public void AddUIStat(IStatsUi stats)
    {
        objects.Add(stats);
    }

    public void InitPlayerStats()
    {
        playerStats = new PlayerStats();

        playerStats.unitsStats = new List<UnitStats>
        {
            new UnitStats(Unit.Tank, 1, 1, 1),
            new UnitStats(Unit.Helicopter, 1, 1, 1),
            new UnitStats(Unit.Base, 1, 0, 1),
            new UnitStats(Unit.Fabric, 1, 0, 1),
            new UnitStats(Unit.Drill, 1, 0, 1),
            new UnitStats(Unit.Tower, 1, 1, 1),
            new UnitStats(Unit.Plate, 1, 0, 0)
        };

        playerStats.armorCoins = 4;
        playerStats.powerCoins = 4;
        playerStats.goldCoins = 1000;

        InitHealthBase();
        InitPrices();
    }

    public void InitPrices()
    {
        PlayerPrefs.SetInt("TankPrice", tankPrice);
        PlayerPrefs.SetInt("HelicopterPrice", helicopterPrice);
        PlayerPrefs.SetInt("BasePrice", basePrice);
        PlayerPrefs.SetInt("FabricPrice", fabricPrice);
        PlayerPrefs.SetInt("DrillPrice", drillPrice);
        PlayerPrefs.SetInt("TowerPrice", towerPrice);
        PlayerPrefs.SetInt("PlatePrice", platePrice);
    }

    public void InitHealthBase()
    {
        PlayerPrefs.SetInt("TankHealth", tankHealth);
        PlayerPrefs.SetInt("HelicopterHealth", helicopterHealth);
        PlayerPrefs.SetInt("BaseHealth", baseHealth);
        PlayerPrefs.SetInt("FabricHealth", fabricHealth);
        PlayerPrefs.SetInt("DrillHealth", drillHealth);
        PlayerPrefs.SetInt("TowerHealth", towerHealth);
        PlayerPrefs.SetInt("PlateHealth", plateHealth);
    }

    public void AddGoldCoins()
    {
        playerStats.goldCoins += 1000;
        UiStatsUpdate();
    }

    public void AddArmorCoins()
    {
        playerStats.armorCoins += 1000;
        UiStatsUpdate();
    }

    public void AddPowerCoins()
    {
        playerStats.powerCoins += 1000;
        UiStatsUpdate();
    }

    public void AddGoldCoins(int value)
    {
        playerStats.goldCoins += value;
        UiStatsUpdate();
    }

    public void AddArmorCoins(int value)
    {
        playerStats.armorCoins += value;
        UiStatsUpdate();
    }

    public void AddPowerCoins(int value)
    {
        playerStats.powerCoins += value;
        UiStatsUpdate();
    }

    public void AddEveryCoin(int gold, int power, int armor)
    {
        playerStats.goldCoins += gold;
        playerStats.powerCoins += power;
        playerStats.armorCoins += armor;

        SavePlayer();
        //UiStatsUpdate();
    }

    public void SavePlayer()
    {
        playerStats.SavePlayer();
    }

    public void LoadOrInitPlayer()
    {
        playerStats = new PlayerStats();
        playerStats = PlayerStats.GetPlayer();

        if (playerStats == null)
        {
            InitPlayerStats();
        }
        //else if (playerStats.unitsStats.Find(x => x.Unit == Unit.Helicopter) == null) //TODO_PC_Replace
        //{
        //    InitPlayerStats();
        //}

        playerStats.CountUnitStats();
    }
}
