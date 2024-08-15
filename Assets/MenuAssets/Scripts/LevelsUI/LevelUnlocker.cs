using UnityEngine;

public class LevelUnlocker : MonoBehaviour
{
    [SerializeField] private int lvlId;

    private void Start()
    {
        if(PlayerPrefs.GetInt("Custom_" + lvlId) == 1)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        PlayerPrefs.SetInt("Custom_" + lvlId, 1);

        //Yandex.ShowReward();
        //PauseManager.Pause();

        this.gameObject.SetActive(false);
    }
}
