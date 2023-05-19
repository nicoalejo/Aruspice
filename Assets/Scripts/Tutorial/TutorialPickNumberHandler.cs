using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialPickNumberHandler : MonoBehaviour
{
    [SerializeField] private int[] numbersToPickArray = new int [9];
    [SerializeField] private GameObject btnNumberToPick;

    public static Action<int> onNumberPicked;
    
    void Start()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < numbersToPickArray.Length; i++)
        {
             GameObject currentNumberToPick = Instantiate(btnNumberToPick, transform);
             currentNumberToPick.GetComponentInChildren<TextMeshProUGUI>().text = numbersToPickArray[i].ToString();
             if (i != 0)
             {
                 currentNumberToPick.GetComponent<Button>().interactable = false;
             }
             else
             { 
                 currentNumberToPick.GetComponent<Button>().onClick.AddListener(() => onNumberPicked?.Invoke(numbersToPickArray[i]));
             }
        }
        
       
    }


}
