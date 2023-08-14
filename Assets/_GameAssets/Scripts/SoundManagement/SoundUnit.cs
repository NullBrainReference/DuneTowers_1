using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType { Shoot, Move, Rotor }

[System.Serializable]
public class SoundUnit
{
    public SoundType soundType;
    public Unit unitType;

    public AudioSource source;

    public bool IsOver
    {
        get
        {
            if (source == null) return true;

            if (source.isPlaying) return false;

            return true;
        }
    }

    public void Play()
    {
        source.Play();
    }
}
