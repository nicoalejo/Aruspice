using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PickNumberHandler : MonoBehaviour
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
             var tempValue = i;         //Creates a temp value for i to send to the anonymous function
             currentNumberToPick.GetComponent<Button>().onClick.AddListener(() => onNumberPicked?.Invoke(numbersToPickArray[tempValue])) ;
        }
    }


}
