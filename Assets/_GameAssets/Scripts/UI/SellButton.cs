using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellButton : MonoBehaviour
{
    [SerializeField] private Text costText;

    private UnitController unitController;
    private float cost;
    
    public void Turn(bool on, bool isDestroyed, float cost = 0, UnitController unitController = null)
    {
        this.cost = cost;

        if (on == false)
        {
            if (isDestroyed)
            {
                if (this.unitController == unitController)
                {
                    gameObject.SetActive(on);
                    this.unitController = null;
                }
            }
            else
            {
                gameObject.SetActive(on);
                this.unitController = null;
            }

            return;
        }

        gameObject.SetActive(on);

        this.unitController = unitController;
        costText.text = cost.ToString() + "c";
    }

    public void Sell()
    {
        MoneyManager.Instance(0).AddMoney(cost);

        Destroy(unitController.gameObject);
    }
}
