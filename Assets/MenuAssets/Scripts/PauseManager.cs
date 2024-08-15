using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public void UnPause()
    {
        Time.timeScale = 1;

        SimpleSoundsManager.Instance.UnPause();
    }

    public void PauseForJS()
    {
        Time.timeScale = 0;

        SimpleSoundsManager.Instance.Pause();
    }

    public static void Pause()
    {
        Time.timeScale = 0;

        SimpleSoundsManager.Instance.Pause();
    }
}
