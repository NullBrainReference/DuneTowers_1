using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValuesUpdater : MonoBehaviour
{
    [SerializeField] private TopPanelController controller;

    public void UpdateEvent()
    {
        controller.UpdateValuesStandalone();
    }
}
