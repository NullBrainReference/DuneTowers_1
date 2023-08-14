using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsCollector : MonoBehaviour
{
    public List<SoundUnit> tankMoveSounds;
    public List<SoundUnit> tankShootSounds;
    //public List<SoundUnit> tankRotorSounds;

    public List<SoundUnit> helicopterMoveSounds;
    public List<SoundUnit> helicopterShootSounds;
    //public List<SoundUnit> helicopterRotorSounds;

    public List<SoundUnit> turretShootSounds;

    public List<SoundUnit> rotorSounds;

    [SerializeField] private int tankShootsLimit;
    [SerializeField] private int helicopterShootsLimit;
    [SerializeField] private int turretShootsLimit;

    [SerializeField] private int tankMoveLimit;
    [SerializeField] private int helicopterMoveLimit;

    [SerializeField] private int rotorLimit;

    [SerializeField] private float explosionCoolDown;
    private float explosionTime;

    public bool IsExplosionReady { get { return explosionTime < 0; } }

    public static SoundsCollector Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        tankMoveSounds = new List<SoundUnit>();
        tankShootSounds = new List<SoundUnit>();

        helicopterMoveSounds = new List<SoundUnit>();
        helicopterShootSounds = new List<SoundUnit>();

        turretShootSounds = new List<SoundUnit>();

        rotorSounds = new List<SoundUnit>();

        InvokeRepeating("ClearEndedSounds", 0, 0.3f);
    }

    public void FixedUpdate()
    {
        explosionTime -= Time.deltaTime;
    }

    public void SetExplosionToCoolDown()
    {
        explosionTime = explosionCoolDown;
    }

    public void AddSoundUnit(SoundUnit soundUnit)
    {
        if (soundUnit.unitType == Unit.Tank)
        {
            if (soundUnit.soundType == SoundType.Move)
            {
                //if (tankMoveSounds.Find(x => x.source == soundUnit.source) != null)
                //    return;

                if (tankMoveLimit > tankMoveSounds.Count)
                {
                    tankMoveSounds.Add(soundUnit);
                    soundUnit.Play();
                }
            }
            else if (soundUnit.soundType == SoundType.Shoot)
            {
                //if (tankShootSounds.Find(x => x.source == soundUnit.source) != null)
                //    return;

                if (tankShootsLimit > tankShootSounds.Count)
                {
                    tankShootSounds.Add(soundUnit);
                    soundUnit.Play();
                }
            }
        }
        else if (soundUnit.unitType == Unit.Helicopter)
        {
            if (soundUnit.soundType == SoundType.Move)
            {
                //if (helicopterMoveSounds.Find(x => x.source == soundUnit.source) != null)
                //    return;

                if (helicopterMoveLimit > helicopterMoveSounds.Count)
                {
                    helicopterMoveSounds.Add(soundUnit);
                    soundUnit.Play();
                }
            }
            else if (soundUnit.soundType == SoundType.Shoot)
            {
                //if (helicopterShootSounds.Find(x => x.source == soundUnit.source) != null)
                //    return;

                if (helicopterShootsLimit > helicopterShootSounds.Count)
                {
                    helicopterShootSounds.Add(soundUnit);
                    soundUnit.Play();
                }
            }
        }
        else if (soundUnit.unitType == Unit.Tower)
        {
            if (soundUnit.soundType == SoundType.Shoot)
            {
                //if (turretShootSounds.Find(x => x.source == soundUnit.source) != null)
                //    return;

                if (turretShootsLimit > turretShootSounds.Count)
                {
                    turretShootSounds.Add(soundUnit);
                    soundUnit.Play();
                }
            }
        }

        if (soundUnit.soundType == SoundType.Rotor)
        {
            //if (rotorSounds.Find(x => x.source == soundUnit.source) != null)
            //    return;

            if (rotorLimit > rotorSounds.Count)
            {
                rotorSounds.Add(soundUnit);
                soundUnit.Play();
            }
        }
    }

    public bool IsShootSoundInList(AudioSource source, Unit unit)
    {
        switch (unit)
        {
            case Unit.Tank:
                if (tankShootSounds.Find(x => x.source == source) == null)
                    return false;
                else
                    return true;

            case Unit.Helicopter:
                if (helicopterShootSounds.Find(x => x.source == source) == null)
                    return false;
                else
                    return true;

            case Unit.Tower:
                if (turretShootSounds.Find(x => x.source == source) == null)
                    return false;
                else
                    return true;
        }

        return false;
    }

    public bool IsMoveSoundInList(AudioSource source, Unit unit)
    {
        switch (unit)
        {
            case Unit.Tank:
                if (tankMoveSounds.Find(x => x.source == source) == null)
                    return false;
                else
                    return true;

            case Unit.Helicopter:
                if (helicopterMoveSounds.Find(x => x.source == source) == null)
                    return false;
                else
                    return true;
        }

        return false;
    }

    public bool IsRotorSoundInList(AudioSource source)
    {
        if (rotorSounds.Find(x => x.source == source) == null)
            return false;

        return true;
    }

    private void ClearEndedSounds()
    {
        foreach (var sound in tankMoveSounds)
        {
            if (sound.IsOver)
            {
                tankMoveSounds.Remove(sound);
                break;
            }
        }

        foreach (var sound in tankShootSounds)
        {
            if (sound.IsOver)
            {
                tankShootSounds.Remove(sound);
                break;
            }
        }

        foreach (var sound in helicopterMoveSounds)
        {
            if (sound.IsOver)
            {
                helicopterMoveSounds.Remove(sound);
                break;
            }
        }

        foreach (var sound in helicopterShootSounds)
        {
            if (sound.IsOver)
            {
                helicopterShootSounds.Remove(sound);
                break;
            }
        }

        foreach (var sound in turretShootSounds)
        {
            if (sound.IsOver)
            {
                turretShootSounds.Remove(sound);
                break;
            }
        }

        foreach (var sound in rotorSounds)
        {
            if (sound.IsOver)
            {
                rotorSounds.Remove(sound);
                break;
            }
        }

    }
}
