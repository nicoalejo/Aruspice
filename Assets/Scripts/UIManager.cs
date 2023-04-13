using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CardHandler cardPrefab;
    [SerializeField] private GameObject cardContainerUI;
    [SerializeField] private GameObject trickPanelUI;
    [SerializeField] private GameObject trickTop10UI;
    [SerializeField] private Dropdown dropDownSuit;
    [SerializeField] private Dropdown dropDownValue;

    private void OnEnable()
    {
        TrickHandler.top10CardsSuits += PopulateCardsPanelUI;
    }

    private void OnDisable()
    {
        TrickHandler.top10CardsSuits -= PopulateCardsPanelUI;
    }

    public void Top10TrickSuitUI()
    {
        ActivatePanelTop10();
        trickTop10UI.SetActive(true);
        PopulateDropownSuit();
    }

    private void PopulateDropownSuit()
    {
        dropDownSuit.ClearOptions();
        List<string> enumNames = new List<string>(Enum.GetNames(typeof(CardSuit)));
        dropDownSuit.AddOptions(enumNames);
    }
    private void ActivatePanelTop10()
    {
        trickPanelUI.SetActive(true);
        trickTop10UI.SetActive(false);
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

    private void CloseTrickPanel()
    {
        trickPanelUI.SetActive(false);
    }

    public void PopulateCardsPanelUI(List<Tuple<Card, bool>> top10CardsList)
    {
        foreach (var tuple in top10CardsList)
        {
            if (tuple.Item2)
            {
                CardHandler newCard = Instantiate(cardPrefab, cardContainerUI.transform);
                newCard.SetCanvas(GetComponentInParent<Canvas>());
                newCard.Initialize(tuple.Item1);
                
            }
             
        }
    }
}
