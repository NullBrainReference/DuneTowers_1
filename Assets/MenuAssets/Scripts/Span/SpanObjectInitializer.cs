using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System;
using Lean.Localization;
using System.Collections;
using System.Collections.Generic;


public class SpanObjectInitializer : MonoBehaviour
{
    [System.Serializable]
    public class PeriodButton
    {
        public string key;
        public float period = 60;

    }

    public List<PeriodButton> periodYandexButtons;

    void Awake()
    {
        foreach(PeriodButton periodButton in periodYandexButtons)
        {
            SpanObjectUpdater.InitValue(periodButton.key, periodButton.period);
        }
    }

}