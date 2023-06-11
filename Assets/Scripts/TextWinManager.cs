using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextData
{
    public int id;
    public string text;
}

[System.Serializable]
public class TextDataCollection
{
    public List<TextData> items;
}

public class TextWinManager : MonoBehaviour
{
    [SerializeField] 
    
    private string dataPath = "TextWin/textWin";
    private TextDataCollection textDataCollection;
    
    public static TextWinManager instance = null;

    //Create singleton for Save Manager
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        
        LoadTextData();
    }

    private void LoadTextData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(dataPath);
        if (textAsset != null)
        {
            string json = textAsset.text;
            textDataCollection = JsonUtility.FromJson<TextDataCollection>(json);
        }
        else
        {
            Debug.LogError("Cannot find file at " + dataPath);
        }
    }

    public string GetTextById(int id)
    {
        foreach (TextData textData in textDataCollection.items)
        {
            if (textData.id == id)
            {
                return textData.text;
            }
        }

        return null;
    }
    
    
    
}