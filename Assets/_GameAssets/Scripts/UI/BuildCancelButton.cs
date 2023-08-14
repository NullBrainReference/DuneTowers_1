using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCancelButton : MonoBehaviour
{
    public void OnClick()
    {
        HeadquartersUnitController.Instance.DestroySelectPlaces(true);
        this.gameObject.SetActive(false);
        MoneyManager.Instance(0).ReturnReservedMoney();
    }
}
