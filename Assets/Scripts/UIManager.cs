using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CardHandler cardPrefab;
    [SerializeField] private GameObject cardContainerUI;
    [SerializeField] private GameObject trickPanelUI;
    [SerializeField] private GameObject trickTop10SuitUI;
    [SerializeField] private GameObject buttonTrickTop10SuitUI;
    [SerializeField] private GameObject buttonTrickTop10ValueUI;
    [SerializeField] private Dropdown dropDownSuitValue;

    private void OnEnable()
    {
        TrickHandler.top10CardsSuits += PopulateCardsPanelUI;
    }

    private void OnDisable()
    {
        TrickHandler.top10CardsSuits -= PopulateCardsPanelUI;
    }
    
    //Activates the UI for the Trick Top 10 Suit
    public void Top10TrickSuitUI()
    {
        ActivatePanelCards();
        dropDownSuitValue.gameObject.SetActive(true);
        
        PopulateDropdownSuit();
        buttonTrickTop10SuitUI.SetActive(true);
    }
    
    //Activates the UI for the Trick Top 10 Value
    public void Top10TrickValueUI()
    {
        ActivatePanelCards();
        dropDownSuitValue.gameObject.SetActive(true);
        
        PopulateDropdownValue();
        buttonTrickTop10ValueUI.SetActive(true);
    }

    private void PopulateDropdownValue()
    {
        dropDownSuitValue.ClearOptions();
        List<string> dropdownValues = new List<string>();
        for (int i = 1; i <= DeckHandler.DeckMaxValue; i++)
        {
            dropdownValues.Add(i.ToString());
        }
        dropDownSuitValue.AddOptions(dropdownValues);
    }

    private void PopulateDropdownSuit()
    {
        dropDownSuitValue.ClearOptions();
        List<string> enumNames = new List<string>(Enum.GetNames(typeof(CardSuit)));
        dropDownSuitValue.AddOptions(enumNames);
    }
    private void ActivatePanelCards()
    {
        dropDownSuitValue.gameObject.SetActive(false);
        buttonTrickTop10SuitUI.SetActive(false);
        buttonTrickTop10ValueUI.SetActive(false);
        trickPanelUI.SetActive(true);
        trickTop10SuitUI.SetActive(true);
        ClearCardContainer();
    }

    private void ClearCardContainer()
    {
        //Deletes all items in the card container
        foreach (Transform item in cardContainerUI.transform)
        {
            Destroy(item.gameObject);
        }
    }

    public void CloseTrickPanel()
    {
        trickPanelUI.SetActive(false);
    }

    public void PopulateCardsPanelUI(List<Tuple<Card, bool>> top10CardsList)
    {
        DeactivateTrickPanelButtonsUI();
        
        foreach (var tuple in top10CardsList)
        {
            CardHandler cardTop10 = Instantiate(cardPrefab, cardContainerUI.transform);
            cardTop10.SetCanvas(GetComponentInParent<Canvas>());
            cardTop10.Initialize(tuple.Item1);
            Image cardImage = cardTop10.GetComponent<Image>();
            
            if (tuple.Item2)
            {
                cardImage.color = Color.red;
            }
        }
    }

    public void DeactivateTrickPanelButtonsUI()
    {
        trickTop10SuitUI.SetActive(false);
    }
}
