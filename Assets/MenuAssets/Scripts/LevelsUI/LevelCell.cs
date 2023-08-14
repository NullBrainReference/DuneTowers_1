using UnityEngine;
using UnityEngine.UI;

public class LevelCell : MonoBehaviour
{
    [SerializeField] private Text levelText;
    [SerializeField] private Text levelScore;

    [SerializeField] private GameObject[] raitingMasks;
    [SerializeField] private GameObject lockMask;
    [SerializeField] private GameObject arrow;

    [SerializeField] private GameObject mapSector;

    [SerializeField] private string levelTitle;

    [SerializeField] private BriefingController briefingPanel;

    private Image mapImage;
    private bool isLocked;

    [SerializeField] private bool isCustom;
    //[SerializeField] private bool isRandomLevel;

    [SerializeField] public LevelStats levelStats;

    public ScoreUI scoreUI;

    private void Awake()
    {
        if (mapSector != null)
            mapImage = mapSector.GetComponent<Image>();

        if (isCustom)
            UnlockForced();

        //if (isRandomLevel)
        //{
        //    if (LevelResult.Load(24).passed)
        //    {
        //        levelStats.unitsStats = PlayerStats.GetPlayer().unitsStats;
        //        lockMask.SetActive(false);
        //    }
        //}
    }

    public void UnlockForced()
    {
        levelStats = LevelStats.LoadFromFile(levelStats.id);
        isLocked = false;
        lockMask.SetActive(false);
    }

    public void OnClick()
    {
        if (this.isLocked) 
            return;

        briefingPanel.Show(levelStats);

        SimpleSoundsManager.Instance.PlayClick();
    }

    //public void TowerDefenceStart()
    //{
    //    levelStats = new LevelStats();
    //
    //    levelStats.DefaultsTest(1);
    //
    //    GameStats.Instance.isTowerDefence = true;
    //
    //    GameStats.Instance.SetStats(levelStats, MenuManager.Instance.PlayerStats);
    //    SceneManager.LoadScene("mainScene", LoadSceneMode.Single);
    //}

    public void SetStats(LevelStats levelStats)
    {
        this.levelStats = levelStats;
    }

    public void SetLevel(int id, int userScore)
    {
        this.SetStats(LevelStats.LoadFromFile(id));

        if (isCustom == false)
        {
            levelText.text = id.ToString();
            levelScore.text = userScore.ToString();


            bool passed = LevelResult.Load(id - 1).passed;

            lockMask.SetActive(!passed);
            if (!passed)
            {
                ShadowMapSector();
            }
            else
            {
                PlayerPrefs.SetInt("LastLevel", id - 1);
                LightMapSector();
            }

            arrow.SetActive(!isLocked);
            this.isLocked = !passed;
        }

        if (scoreUI != null)
            scoreUI.CountScore();
    }

    public void SetLevel(int id, int userScore, bool isLocked)
    {
        this.SetStats(LevelStats.LoadFromFile(id));

        if (isCustom == false)
        {

            levelText.text = id.ToString();
            levelScore.text = userScore.ToString();

            lockMask.SetActive(isLocked);
            if (isLocked)
            {
                ShadowMapSector();
            }
            else
            {
                LightMapSector();
            }

            arrow.SetActive(!isLocked);
            this.isLocked = isLocked;
        }

        if (scoreUI != null)
            scoreUI.CountScore();
    }

    private void ShadowMapSector()
    {
        if (mapSector != null)
            mapImage.color = new Color(0, 0, 0, 0.5f);
    }

    private void LightMapSector()
    {
        if (mapSector != null)
            mapImage.color = new Color(255, 255, 255, 0.0f);
    }
}
