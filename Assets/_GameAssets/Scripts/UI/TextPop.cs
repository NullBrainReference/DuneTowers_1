using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextPop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject nameText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        nameText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        nameText.SetActive(false);
    }
}
