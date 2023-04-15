using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CardHandler cardPrefab;
    [SerializeField] private CardHandler cardPrefabDiscard;
    [SerializeField] private GameObject cardContainerUI;
    [SerializeField] private GameObject trickPanelUI;
    [SerializeField] private GameObject trickTop10SuitUI;
    [SerializeField] private GameObject buttonTrickTop10SuitUI;
    [SerializeField] private GameObject buttonTrickTop10ValueUI;
    [SerializeField] private GameObject buttonTrickDiscard4ShowDeckUI;
    [SerializeField] private GameObject buttonTrickDiscard4UI;
    [SerializeField] private GameObject closePanelButtonUI;
    [SerializeField] private Dropdown dropDownSuitValue;

    private void OnEnable()
    {
        TrickHandler.top10CardsSuits += PopulateCardsPanelUI;
        TrickHandler.discardDeck += PopulateCardContainerDiscard4;
    }

    private void OnDisable()
    {
        TrickHandler.top10CardsSuits -= PopulateCardsPanelUI;
        TrickHandler.discardDeck -= PopulateCardContainerDiscard4;
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
    
    //Activates the UI for the Trick Discard 4
    public void Discard4()
    {
        ActivatePanelCards();
        buttonTrickDiscard4ShowDeckUI.SetActive(true);
    }

    private void ActivatePanelCards()
    {
        dropDownSuitValue.gameObject.SetActive(false);
        buttonTrickTop10SuitUI.SetActive(false);
        buttonTrickTop10ValueUI.SetActive(false);
        buttonTrickDiscard4UI.SetActive(false);
        trickPanelUI.SetActive(true);
        trickTop10SuitUI.SetActive(true);
        ClearCardContainer();
    }
    private List<GameObject> PopulateCardContainerDiscard4(List<Card> deck)
    {
        buttonTrickDiscard4ShowDeckUI.SetActive(false);
        buttonTrickDiscard4UI.SetActive(true);

        List<GameObject> showingDeck = new List<GameObject>();
        
        foreach (Card cardInDeck in deck)
        {
            CardHandler cardInContainer = Instantiate(cardPrefabDiscard, cardContainerUI.transform);
            cardInContainer.SetCanvas(GetComponentInParent<Canvas>());
            cardInContainer.Initialize(cardInDeck);
            showingDeck.Add(cardInContainer.gameObject);
        }

        return showingDeck;
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

            if (tuple.Item2)
            {
                Image cardImage = cardTop10.GetComponent<Image>();
                cardImage.color = Color.red;
            }
        }
    }

    public void DeactivateTrickPanelButtonsUI()
    {
        trickTop10SuitUI.SetActive(false);
    }
    
    
}
