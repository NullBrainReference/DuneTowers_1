using System.Collections;
using UnityEngine;

public class UnitSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource movementSound;
    [SerializeField] private AudioSource aimingSound;

    [SerializeField] private bool isUnstopable;

    [SerializeField] private Unit unitType;

    private float moveVolumeDefault;

    public bool IsMovePlaying 
    { 
        get 
        {
            return movementSound.isPlaying;
        } 
    }

    public bool IsAimPlaying
    {
        get 
        { 
            return aimingSound.isPlaying; 
        }
    }

    private void Awake()
    {
        if (SimpleSoundsManager.Instance.isOn == false)
        {
            shootSound.volume = 0;
            movementSound.volume = 0;
            aimingSound.volume = 0;
        }
        else
        {
            float volume = SimpleSoundsManager.Instance.soundConfig.VolumeUnits;

            shootSound.volume *= volume;
            movementSound.volume *= volume;
            aimingSound.volume *= volume;
        }

        moveVolumeDefault = movementSound.volume;
    }

    public void FixedUpdate()
    {
        if (GameStats.Instance.gameResult == GameResult.Win || GameStats.Instance.gameResult == GameResult.Lose)
        {
            StopRotation();
            StopMoveForced();
        }
    }

    public void PlayMove()
    {
        if (SoundsCollector.Instance.IsMoveSoundInList(movementSound, unitType))
        {
            StopAllCoroutines();

            movementSound.volume = moveVolumeDefault;
            movementSound.Play();
            return;
        }

        StopAllCoroutines();

        SoundsCollector.Instance.AddSoundUnit(
            new SoundUnit { soundType = SoundType.Move, source = movementSound, unitType = this.unitType }
            );

        //movementSound.Play();
    }

    private IEnumerator MoveShotingDown()
    {
        while (movementSound.volume > 0)
        {
            movementSound.volume -= 0.01f;
            yield return new WaitForEndOfFrame();
        }

        movementSound.Stop();
        movementSound.volume = moveVolumeDefault;
    } 

    public void StopMove()
    {
        if (isUnstopable) return;

        StartCoroutine(MoveShotingDown());

        //movementSound.Stop();
    }

    private void StopMoveForced()
    {
        movementSound.Stop();
    }

    public void PlayRotation()
    {
        //if (SoundsCollector.Instance.IsRotorSoundInList(aimingSound))
        //    return;
        //
        //SoundsCollector.Instance.AddSoundUnit(
        //    new SoundUnit { soundType = SoundType.Rotor, source = aimingSound, unitType = this.unitType }
            //);
        //aimingSound.Play();
    }

    public void StopRotation()
    {
        aimingSound.Stop();
    }

    public void PlayShoot()
    {
        if (SoundsCollector.Instance.IsShootSoundInList(shootSound, unitType))
        {
            shootSound.Play();
            return;
        }

        SoundsCollector.Instance.AddSoundUnit(
            new SoundUnit { soundType = SoundType.Shoot, source = shootSound, unitType = this.unitType }
            );
        //shootSound.Play();
    }
}
