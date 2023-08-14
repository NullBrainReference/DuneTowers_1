using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.IO;
#endif

using UnityEngine;

[System.Serializable]
public class TextItem
{
    [SerializeField] private int id;
    [SerializeField] private string textRu;
    [SerializeField] private string textEn;

    public int Id { get { return id; } }
    public string TextRu { get { return textRu; } }
    public string TextEn { get { return textEn; } }


#if UNITY_EDITOR
    public void WriteFile()
    {
        string editString = JsonUtility.ToJson(this);

        string path = Application.dataPath + @"/Resources/Texts/TextItem_" + id + ".txt";
        //AssetDatabase.GetAssetPath()
        Debug.Log(path);

        using (StreamWriter sw = new StreamWriter(path, false))
        {
            sw.WriteLine(editString);
        }
    }
#endif

    public static TextItem LoadText(int id)
    {
        TextItem textItem = new TextItem();

        var textFile = Resources.Load<TextAsset>(@"Texts\TextItem_" + id.ToString());

        if (textFile == null)
        {
            Debug.Log("TextItem_" + id.ToString() + "_Doesn't exist");
            return new TextItem();
        }

        textItem = JsonUtility.FromJson<TextItem>(textFile.text);

        return textItem;
    }
}
