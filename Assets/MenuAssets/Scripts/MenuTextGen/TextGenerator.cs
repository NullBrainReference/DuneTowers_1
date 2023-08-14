using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextGenerator : MonoBehaviour
{
    [SerializeField] private TextItem editText;
#if UNITY_EDITOR
    public void WriteEdItem()
    {
        editText.WriteFile();
    }

    public void LoadEdItem()
    {
        editText = TextItem.LoadText(editText.Id);
    }
#endif
}
