using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableButton : MonoBehaviour
{
    [SerializeField] private GameObject mask;

    private Button button;

    private void Start()
    {
        button = gameObject.GetComponent<Button>();

        MapGenerator.Instance.selectableButtons.Add(this);
    }

    public void DropSelect()
    {
        mask.SetActive(false);
    }

    public void Select()
    {
        MapGenerator.Instance.DropSelects();
        mask.SetActive(true);
    }
}
