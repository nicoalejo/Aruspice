using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PickNumberHandler : MonoBehaviour
{
    [SerializeField] private int finalNumber = 60;
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

        List<int> numbersAchieved = SaveManager.instance.GetCompletedNumbers();
        //If all numbers are achieved, show the final screen
        if (numbersAchieved.Count >= 12)
        {
            GameObject currentNumberToPick = Instantiate(btnNumberToPick, panelNumbersToPick);
            currentNumberToPick.GetComponentInChildren<TextMeshProUGUI>().text = "Final";
            currentNumberToPick.GetComponent<Button>().onClick.AddListener(() => onNumberPicked?.Invoke(finalNumber));       
        }
        else
        {
            for (int i = 0; i < numbersToPickArray.Length; i++)
            {
                GameObject currentNumberToPick = Instantiate(btnNumberToPick, panelNumbersToPick);
                currentNumberToPick.GetComponentInChildren<TextMeshProUGUI>().text = numbersToPickArray[i].ToString();
                var tempValue = i;         //Creates a temp value for i to send to the anonymous function
                currentNumberToPick.GetComponent<Button>().onClick.AddListener(() => onNumberPicked?.Invoke(numbersToPickArray[tempValue])) ;
                //Change color of already completed numbers
                if (numbersAchieved != null && numbersAchieved.Contains(numbersToPickArray[i]))
                {
                    currentNumberToPick.GetComponent<Image>().color = new Color(209, 0, 20);
                }
            }    
        }
    }
}
