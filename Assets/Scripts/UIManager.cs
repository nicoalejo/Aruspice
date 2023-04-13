using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject cardContainerUI;
    [SerializeField] private GameObject trickPanelUI;
    [SerializeField] private GameObject trickTop10UI;
    [SerializeField] private Dropdown dropDownSuit;
    [SerializeField] private Dropdown dropDownValue;

    private void PopulateDropdownSuit()
    {
        trickPanelUI.SetActive(true);
        trickTop10UI.SetActive(true);
        ClearCardContainer();
        
        dropDownSuit.ClearOptions();
        List<string> enumNames = new List<string>(Enum.GetNames(typeof(CardSuit)));
        dropDownSuit.AddOptions(enumNames);
    }

    private void ClearCardContainer()
    {
        //Deletes all items in the card container
        foreach (Transform item in cardContainerUI.transform)
        {
            Destroy(item.gameObject);
        }
    }

    private void CloseTrickPanel()
    {
        trickPanelUI.SetActive(false);
    }
}
