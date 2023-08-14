using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPanelController : MonoBehaviour
{


    public void OnCancel()
    {
        CanvasManager.Instance.isMenuOpened = false;
        DestroyPanel();
    }

    public void DestroyPanel()
    {
        CanvasManager.Instance.isMenuOpened = false;
        Destroy(gameObject);
    }

}
