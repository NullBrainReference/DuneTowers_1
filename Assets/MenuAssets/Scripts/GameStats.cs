using UnityEngine;

public enum GameResult { None, Win, Lose}

public class GameStats : MonoBehaviour
{
    public PlayerBase player;
    public LevelStats level;

    [SerializeField] private PlayerProfile profile;

    public bool isTowerDefence = false;

    public int minGoldOutcome;
    public int maxGoldOutcome;

    public int minComponentsOutcome;
    public int maxComponentsOutcome;

    public int goldOutcome;
    public int armorOutcome;
    public int powerOutcome;

    public int maxCredits = 3000;

    public bool isGameOver = false;

    public int upgradeScore;

    [SerializeField] private float defenceIncomeMultiplier;
    [SerializeField] private float defenceComponentsMultiplier;

    [SerializeField] private float defenceTimeLimit;

    public GameResult gameResult = GameResult.None;

    public PlayerProfile Profile
    {
        get {
            if (profile == null) 
            { 
                profile = PlayerProfile.LoadOrInitProfile();
            }
            
            return profile;
        }
    }

    public OutcomePanelController outcomePanelController;
    public BattleTimer battleTimer;

    public static GameStats Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        profile = PlayerProfile.LoadOrInitProfile();
    }

    private void FixedUpdate()
    {
        if (outcomePanelController == null)
            return;

        if (isTowerDefence == false) 
            return;

        if (isGameOver) 
            return;

        AchievementsManager.Instance.DefenceTimeUpdate(battleTimer.TotalTime);

        if (battleTimer.TotalTime >= defenceTimeLimit)
        {
            isGameOver = true;

            //ShowBannerController.Instance.CallBanner();

            CountOutcomes();
            CallOutcomePanel(1);
        }
    }

    //public void LoadOrInitProfile()
    //{
    //    profile = PlayerProfile.Load();
    //
    //    if (profile == null)
    //    {
    //        profile = new PlayerProfile();
    //        profile.InitCleanProfile(1200);
    //        profile.Save();
    //    }
    //}

    public void SetStats(LevelStats level, PlayerStats playerStats)
    {
        ClearOutcome();

        isGameOver = false;

        this.level = level;
        this.player = playerStats;

        playerStats.SavePlayer();

        gameResult = GameResult.None;

        GetLvlMaxCredits(level.id);
        //CountOutcomes();
        profile = PlayerProfile.LoadOrInitProfile();
    }

    public void CountOutcomes()
    {
        if (!isTowerDefence)
        {
            float multiplier = battleTimer.GetStage() <= 1 ? battleTimer.GetStage() : 1;

            int componentsOutcome =
                minComponentsOutcome +
                (int)((float)(maxComponentsOutcome - minComponentsOutcome) * multiplier);

            powerOutcome = Random.Range(1, componentsOutcome);
            armorOutcome = componentsOutcome - powerOutcome;

            goldOutcome =
                minGoldOutcome +
                (int)((float)(maxGoldOutcome - minGoldOutcome) * multiplier);
        }

        if (isTowerDefence)
            AddComponentsBattleIncome();

        outcomePanelController.SetValues(armorOutcome, powerOutcome, goldOutcome);
    }

    public void GetLvlMaxCredits(int lvl)
    {
        MapStats map = MapStats.Load(lvl);

        int maxCredits = 0;

        foreach (CellStats cell in map.stats)
        {
            if (cell.credits > maxCredits) 
                maxCredits = cell.credits;
        }

        this.maxCredits = maxCredits;
    }

    public void PassOutcome()
    {
        if (outcomePanelController == null) return;

        outcomePanelController.SetValues(armorOutcome, powerOutcome, goldOutcome);
    }

    public void ClearOutcome()
    {
        goldOutcome = 0;
        armorOutcome = 0;
        powerOutcome = 0;
    }

    public void CallOutcomePanel(int playerNo)
    {
        outcomePanelController.InitDefeatValue(playerNo);
        outcomePanelController.PlayShow();
    }

    public PlayerBase GetPlayer(int playerNo)
    {
        if (playerNo == 0)
        {
            return this.player;
        }
        else
        {
            return this.level;
        }
    }

    public UnitStats GetUnit(Unit unit, int playerNo)
    {
        if (playerNo == 0) 
        {
            return this.player.GetUnit(unit);
        }
        else
        {
            return this.level.GetUnit(unit);
        }
    }

    public int GetPrice(Unit unit, int playerNo)
    {
        return GetUnit(unit, playerNo).battlePrice;
    }

    public void AddEnemyLevel()
    {
        level.StatsLvlIncrement();
    }

    public float GetGoldIncomeMultiplier()
    {
        return defenceIncomeMultiplier;
    }

    public void AddComponentsBattleIncome()
    {
        int coins = (int)(battleTimer.TotalTime * defenceComponentsMultiplier);

        for (int i = 0; i < coins; i++)
        {
            float rnd = Random.Range(0f, 1f);

            if (rnd >= 0.5f)
            {
                powerOutcome++;
            }
            else
            {
                armorOutcome++;
            }
        }
    }
}
