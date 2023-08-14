using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitch : MonoBehaviour
{
    [SerializeField] private GameObject targetPanel;

    public void Switch()
    {
        targetPanel.SetActive(!targetPanel.activeInHierarchy);
        SimpleSoundsManager.Instance.PlayClick();
    }
}
