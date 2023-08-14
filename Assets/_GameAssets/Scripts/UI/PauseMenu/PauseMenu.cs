using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PauseButton pauseButton;

    public void ToMenu()
    {
        SimpleSoundsManager.Instance.PlayClick();
        Time.timeScale = 1;
        GameStats.Instance.gameResult = GameResult.Lose;
        SimpleSoundsManager.Instance.SwitchToMenuMusic();
        SceneManager.LoadScene("HorisontalMenu", LoadSceneMode.Single);
    }

    public void Retry()
    {
        SimpleSoundsManager.Instance.PlayClick();
        Time.timeScale = 1;
        GameStats.Instance.gameResult = GameResult.Lose;
        SimpleSoundsManager.Instance.SwitchToBattleMusic();
        SceneManager.LoadScene("mainScene", LoadSceneMode.Single);
    }

    public void Continue()
    {
        SimpleSoundsManager.Instance.PlayClick();
        pauseButton.UnPause();
    }
}
