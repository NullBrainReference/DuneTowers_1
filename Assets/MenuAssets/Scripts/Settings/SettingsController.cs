using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private GameObject settingsGroup;

    [SerializeField] private Text languageText;
    [SerializeField] private Text soundText;
    [SerializeField] private Button okButton;

    [SerializeField] private Animator animator;

    [SerializeField] private LeanPhrase On;
    [SerializeField] private LeanPhrase Off;

    [SerializeField] private OutcomeBackgroundController backgroundController;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider unitsSlider;


    private void Start()
    {
        UpdateSettingsButtonsLocalization();

        musicSlider.value = SimpleSoundsManager.Instance.soundConfig.VolumeMusic;
        unitsSlider.value = SimpleSoundsManager.Instance.soundConfig.VolumeUnits;
    }

    public void UpdateSettingsButtonsLocalization()
    {
        int lngIndex;

        if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
            lngIndex = 1;
        else
            lngIndex = 0;

        if (SimpleSoundsManager.Instance.isOn)
        {
            soundText.text = On.Entries[lngIndex].Text;
        }
        else
        {
            soundText.text = Off.Entries[lngIndex].Text;
        }
    }

    public void SwitchLanguage()
    {
        SimpleSoundsManager.Instance.PlayClick();

        if (LeanLocalization.GetFirstCurrentLanguage() == "Russian") 
            LeanLocalization.SetCurrentLanguageAll("English");
        else
            LeanLocalization.SetCurrentLanguageAll("Russian");

        UpdateSettingsButtonsLocalization();
    }

    public void SetVolumeMusic(float value)
    {
        SimpleSoundsManager.Instance.SetVolumeMusic(value);
    }

    public void SetVolumeUnits(float value)
    {
        SimpleSoundsManager.Instance.SetVolumeUnits(value);
    }

    public void SwitchSound()
    {
        SimpleSoundsManager.Instance.PlayClick();

        if (SimpleSoundsManager.Instance.isOn)
        {
            SimpleSoundsManager.Instance.TurnOff();
            //soundText.text = Off.Entries[0].Text;
        }
        else
        {
            SimpleSoundsManager.Instance.TurnON();
            //soundText.text = On.Entries[0].Text;
        }

        UpdateSettingsButtonsLocalization();
    }

    public void ShowPanel()
    {
        SimpleSoundsManager.Instance.PlayClick();

        settingsGroup.SetActive(true);
        okButton.interactable = true;

        backgroundController.Open();

        PlayShow();
    }

    public void HidePanel()
    {
        SimpleSoundsManager.Instance.PlayClick();
        settingsGroup.SetActive(false);

        SimpleSoundsManager.Instance.SaveVolumeSettings();
        animator.Rebind();
    }

    public void PlayShow()
    {
        animator.Rebind();
        //animator.Play("SettingsShow");
    }

    public void PlayHide()
    {
        okButton.interactable = false;
        backgroundController.PlayClose();
        animator.Play("SettingsHide");
    }
}
