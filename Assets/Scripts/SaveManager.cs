using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Wrapper<T>
{
    public List<T> items;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;
    
    private List<int> numbersCompleted = new ();
    private string savePath;
    
    //Create singleton for Save Manager
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        
        savePath = Path.Combine(Application.persistentDataPath, "NumbersAchieved.json");
        Load();
    }
    
    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            Wrapper<int> wrapper = JsonUtility.FromJson<Wrapper<int>>(json);

            if (wrapper != null && wrapper.items.Count > 0)
            {
                numbersCompleted = wrapper.items;
            }
        }
        else
        {
            Debug.Log("Not save found");    
        }
    }
    
    public void Save(int numberCompleted)
    {
        if (!numbersCompleted.Contains(numberCompleted))
        {
            numbersCompleted.Add(numberCompleted);
            string json = JsonUtility.ToJson(new Wrapper<int> { items = numbersCompleted });

            File.WriteAllText(savePath, json);
        }
    } 
    
    public List<int> GetCompletedNumbers()
    {
        return numbersCompleted;
    }
    
}
