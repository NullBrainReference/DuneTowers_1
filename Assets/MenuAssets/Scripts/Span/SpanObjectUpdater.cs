using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System;
using Lean.Localization;
using System.Collections;


[RequireComponent(typeof(Button))]
public class SpanObjectUpdater : MonoBehaviour, IAttention
{
    
    [SerializeField] private LeanLocalizedTimeSpanFormat textTimer;
    
    [SerializeField] private float period = 60;
    [SerializeField] private string key;
    
    [SerializeField][LeanTranslationNameCaption] private string localizedSpanHours;
    [SerializeField][LeanTranslationNameCaption] private string localizedSpanMinutes;
    [SerializeField][LeanTranslationNameCaption] private string localizedSpanSeconds;
    //[SerializeField][LeanTranslationNameCaption] private string localizedSpanFree;
    [SerializeField] private GameObject playGroup;
    [SerializeField] private GameObject watchGroup;


    //[SerializeField] private Image imageIcon;
    //[SerializeField] private Sprite iconTimerSprite;
    //[SerializeField] private Sprite iconMoveSprite;

    private Button button;
    
    private string NextTimeString
    {
        get {
            string val = PlayerPrefs.GetString(key, "");
            if (val == "")
                PlayerPrefs.SetString(key, DateTime.Now.AddMinutes(period).ToString("F"));
    
            return PlayerPrefs.GetString(key);
        }
        set => PlayerPrefs.SetString(key, value);        
    }

    public SpanObjectInitializer.PeriodButton PeriodButton { get { return new SpanObjectInitializer.PeriodButton { key = key, period = period }; } }

    private TimeSpan GetTimeSpan()
    {
        return (DateTime.Parse(NextTimeString) - DateTime.Now);
    }
    
    
    
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(InitValue);
    }
    
    private void OnDestroy()
    {
        button.onClick.RemoveListener(InitValue);
    }
    
    
    
    private void InitValue()
    {
        PlayerPrefs.SetString(key, DateTime.Now.AddMinutes(period).ToString("F"));
        UpdateSpanText();
    }
    
    public static void InitValue(string _key, float _period)
    {
        if (PlayerPrefs.GetString(_key, "") == "")
            PlayerPrefs.SetString(_key, DateTime.Now.AddMinutes(_period).ToString("F"));
    }
    
    private void Start()
    {
        //StartCoroutine(UpdateSpanText());
        InvokeRepeating("UpdateSpanText", 0f, 1f);
    }
    
    private void UpdateSpanText()
    {
    
        TimeSpan timeSpan = GetTimeSpan();
    
        textTimer.value = timeSpan;
    
    
        button.interactable = timeSpan.TotalSeconds < 1;

        if (watchGroup != null)
            if (timeSpan.TotalSeconds < 1)
            {
                watchGroup.SetActive(false);
                playGroup.SetActive(true);
            }
            else
            {
                watchGroup.SetActive(true);
                playGroup.SetActive(false);
            }
    
    
        if (timeSpan.TotalHours >= 1) { 
            textTimer.TranslationName = localizedSpanHours;
        }
        else if (timeSpan.TotalMinutes >= 1) {
                textTimer.TranslationName = localizedSpanMinutes;
        }
        else if (timeSpan.TotalSeconds >= 1) {
            textTimer.TranslationName = localizedSpanSeconds;
        }
        else {
            //textTimer.TranslationName = localizedSpanFree;
            playGroup.SetActive(true);
            watchGroup.SetActive(false);
        }
    
        
        textTimer.UpdateLocalization();
    
        //if (timeSpan.TotalHours >= 1)
        //    textTimer.text = string.Format("{0:%h} hours {0:%m} minutes", GetTimeSpan());
        //else if (timeSpan.TotalMinutes >= 1)
        //    textTimer.text = string.Format("{0:%m} minutes {0:%s} seconds", GetTimeSpan());
        //else if (timeSpan.TotalSeconds >= 1)
        //    textTimer.text = string.Format("{0:%s} seconds", GetTimeSpan());
        //else
        //    textTimer.text = "free";
    }

    bool IAttention.Check()
    {
        TimeSpan timeSpan = GetTimeSpan();

        if (timeSpan.TotalSeconds < 1)
        {
            return true;
        }

        return false;
    }

}