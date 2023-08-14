using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextItemUI : MonoBehaviour
{
    [SerializeField] private Text textUI;
    [SerializeField] private Animator animator;

    [SerializeField] private int maxIndex;

    private TextItem textItem;


    private void Start()
    {
        SetTextItemRnd();
        SetText();
    }

    public void SetTextItemRnd()
    {
        int index = Random.Range(0, maxIndex + 1);

        textItem = TextItem.LoadText(index);
    }

    public void SetText()
    {
        if (LeanLocalization.GetFirstCurrentLanguage() == "Russian")
            textUI.text = textItem.TextRu;
        else
            textUI.text = textItem.TextEn;
    }

    public void PlayOpen()
    {
        animator.Rebind();
        animator.Play("TextOpen");
    }

    public void PlayClose()
    {
        animator.Rebind();
        animator.Play("TextClose");
    }
}
