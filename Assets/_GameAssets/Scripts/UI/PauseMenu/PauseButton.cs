using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour
{
    private Button button;

    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnClick()
    {
        if (button.interactable)
        {
            SimpleSoundsManager.Instance.PlayClick();

            button.interactable = false;
            pauseMenu.SetActive(true);

            Time.timeScale = 0;
        }
    }

    public void UnPause()
    {
        button.interactable = true;
        pauseMenu.SetActive(false);

        Time.timeScale = 1;
    }
}
