using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PickNumberHandler : MonoBehaviour
{
    [SerializeField] private int[] numbersToPickArray = new int [12];
    [SerializeField] private GameObject btnNumberToPick;
    [SerializeField] private Transform panelNumbersToPick;

    public static Action<int> onNumberPicked;
    
    void Start()
    {
        foreach (Transform item in panelNumbersToPick)
        {
            Destroy(item.gameObject);
        }

        List<int> numbersAchieved = SaveManager.instance.Load();

        for (int i = 0; i < numbersToPickArray.Length; i++)
        {
             GameObject currentNumberToPick = Instantiate(btnNumberToPick, panelNumbersToPick);
             currentNumberToPick.GetComponentInChildren<TextMeshProUGUI>().text = numbersToPickArray[i].ToString();
             var tempValue = i;         //Creates a temp value for i to send to the anonymous function
             currentNumberToPick.GetComponent<Button>().onClick.AddListener(() => onNumberPicked?.Invoke(numbersToPickArray[tempValue])) ;
             if (numbersAchieved != null && numbersAchieved.Contains(numbersToPickArray[i]))
             {
                 currentNumberToPick.GetComponent<Image>().color = new Color(209, 0, 20);
             }
        }
        
        
    }

    
}
