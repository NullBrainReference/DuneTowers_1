using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSoundsManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioClick;
    [SerializeField] private AudioSource audioSwipe;
    [SerializeField] private AudioSource audioOutcome;
    [SerializeField] private AudioSource audioUpgrade;

    [SerializeField] private AudioSource menuPiece;
    [SerializeField] private List<AudioSource> battlePieces;

    [SerializeField] private AudioSource winPiece;
    [SerializeField] private AudioSource losePiece;

    [SerializeField] private AudioSource currPiece;

    [SerializeField] private float defaultBattleMusicVolume;
    [SerializeField] private float defaultMenuMusicVolume;

    public float explosionVolume;
    public float shootVolume;

    public bool isOn = true;

    public SoundsConfig soundConfig;

    public static SimpleSoundsManager Instance { get; private set; }

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

        soundConfig = SoundsConfig.Load();

        if (soundConfig == null)
        {
            soundConfig = new SoundsConfig();
            soundConfig.InitDefaults();
        }
    }

    private void Start()
    {
        defaultBattleMusicVolume = battlePieces[0].volume;
        defaultMenuMusicVolume = menuPiece.volume;

        UpdateMusicVolumeByConfig();

        if (soundConfig.IsOn)
        {
            TurnON();
        }
        else
        {
            TurnOff();
        }
    }

    public void SaveVolumeSettings()
    {
        soundConfig.Save();

        UpdateMusicVolumeByConfig();
    }

    private void UpdateMusicVolumeByConfig()
    {
        if (soundConfig.IsOn == false) return;

        menuPiece.volume = soundConfig.VolumeMusic * defaultMenuMusicVolume;

        winPiece.volume = soundConfig.VolumeMusic * defaultMenuMusicVolume;
        losePiece.volume = soundConfig.VolumeMusic * defaultMenuMusicVolume;

        audioClick.volume = soundConfig.VolumeUnits;
        audioSwipe.volume = soundConfig.VolumeUnits;
        audioOutcome.volume = soundConfig.VolumeUnits;
        audioUpgrade.volume = soundConfig.VolumeUnits;

        foreach (var battlePiece in battlePieces)
            battlePiece.volume = soundConfig.VolumeMusic * defaultBattleMusicVolume;
    }

    public void SetVolumeMusic(float value)
    {
        soundConfig.SetVolumeMusic(value);

        UpdateMusicVolumeByConfig();
    }

    public void SetVolumeUnits(float value)
    {
        soundConfig.SetVolumeUnits(value);

        UpdateMusicVolumeByConfig();
    }

    private void FixedUpdate()
    {
        if (currPiece == null)
            return;

        if (currPiece != menuPiece)
            InBattleSwitch();
    }

    public void PlayClick()
    {
        audioClick.Play();
    }

    public void PlaySwipe()
    {
        audioSwipe.Play();
    }

    public void PlayOutcome()
    {
        audioOutcome.Play();
    }

    public void PlayUpgrade()
    {
        audioUpgrade.Play();
    }

    public void TurnON()
    {
        isOn = true;

        soundConfig.SetVolume(true);

        UpdateMusicVolumeByConfig();
    }

    public void TurnOff()
    {
        audioClick.volume = 0;
        audioSwipe.volume = 0;
        audioOutcome.volume = 0;
        audioUpgrade.volume = 0;

        menuPiece.volume = 0;
        winPiece.volume = 0;
        losePiece.volume = 0;

        foreach (var piece in battlePieces)
            piece.volume = 0;

        isOn = false;

        soundConfig.SetVolume(false);
    }

    public void Pause()
    {
        menuPiece.Pause();

        audioOutcome.Pause();

        foreach (var piece in battlePieces)
            piece.Pause();
    }

    public void UnPause()
    {
        audioOutcome.UnPause();

        menuPiece.UnPause();

        foreach (var piece in battlePieces)
            piece.UnPause();
    }

    public void SwitchToBattleMusic()
    {
        //TODO remove everythere after tracks added
        if (currPiece == null)
            return;

        if (winPiece.isPlaying)
            winPiece.Stop();
        if (losePiece.isPlaying)
            losePiece.Stop();

        menuPiece.Stop();

        currPiece = battlePieces[0];
        currPiece.Play();
    }

    private void InBattleSwitch()
    {
        if (currPiece == null)
            return;

        if (currPiece.isPlaying == false)
        {
            for (int i = 0; i < battlePieces.Count; i++)
            {
                if (currPiece == battlePieces[i])
                {
                    if (i + 1 >= battlePieces.Count)
                    {
                        currPiece = battlePieces[0];
                        currPiece.Play();
                        return;
                    }
                    else
                    {
                        currPiece = battlePieces[i + 1];
                        currPiece.Play();
                        return;
                    }
                }
            }
        }
    }

    public void SwitchToMenuMusic()
    {
        if (currPiece == null)
            return;

        if (winPiece.isPlaying)
            winPiece.Stop();
        if (losePiece.isPlaying)
            losePiece.Stop();

        currPiece.Stop();

        currPiece = menuPiece;
        currPiece.Play();
    }

    public void PlayWinSound()
    {
        currPiece.Stop();

        winPiece.Play();
    }

    public void PlayLoseSound()
    {
        //Debug.Log("PlayLose");

        currPiece.Stop();

        losePiece.Play();
    }
}
