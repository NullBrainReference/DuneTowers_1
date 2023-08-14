using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        audioSource.volume *= SimpleSoundsManager.Instance.soundConfig.VolumeUnits;

        Play();
    }

    private void Play()
    {
        if (SimpleSoundsManager.Instance.isOn == false)
            return;

        if (SoundsCollector.Instance.IsExplosionReady == false)
            return;

        audioSource.Play();

        SoundsCollector.Instance.SetExplosionToCoolDown();
    }
}
