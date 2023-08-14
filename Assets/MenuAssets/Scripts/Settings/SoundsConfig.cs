using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundsConfig
{
    [SerializeField] private float volumeMusic;
    [SerializeField] private float volumeUnits;

    [SerializeField] private bool isOn;

    public float VolumeMusic { get { return volumeMusic; } }
    public float VolumeUnits { get { return volumeUnits; } }
    public bool IsOn { get { return isOn; } }

    public void InitDefaults()
    {
        volumeMusic = 1;
        volumeUnits = 1;

        isOn = true;

        Save();
    }

    public void SetVolumeMusic(float value)
    {
        volumeMusic = value;
    }

    public void SetVolumeUnits(float value)
    {
        volumeUnits = value;
    }

    public void SetVolume(bool isOn)
    {
        this.isOn = isOn;
    }

    public void Save()
    {
        string soundString = JsonUtility.ToJson(this);

        PlayerPrefs.SetString("SoundsConfig", soundString);
        Debug.Log("SoundsConfig Saved");
    }

    public static SoundsConfig Load()
    {
        string soundsConfig = PlayerPrefs.GetString("SoundsConfig");
        SoundsConfig player = JsonUtility.FromJson<SoundsConfig>(soundsConfig);

        return player;
    }
}
