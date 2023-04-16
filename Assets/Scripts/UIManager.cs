using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CardHandler cardPrefab;
    [SerializeField] private CardHandler cardPrefabDiscard;
    [SerializeField] private GameObject cardContainerUI;            //Container for cards in hand
    [SerializeField] private GameObject cardAltarContainerUI;       //Container for cards in altar
    [SerializeField] private TextMeshProUGUI altarValueTextUI;       //Altar value text
    [SerializeField] private GameObject trickPanelUI;
    [SerializeField] private GameObject trickTop10SuitUI;
    [SerializeField] private GameObject buttonTrickTop10SuitUI;
    [SerializeField] private GameObject buttonTrickTop10ValueUI;
    [SerializeField] private GameObject buttonTrickDiscard4ShowDeckUI;
    [SerializeField] private GameObject buttonTrickDiscard4UI;
    [SerializeField] private GameObject closePanelButtonUI;
    [SerializeField] private GameObject confirmationPanelUI;
    [SerializeField] private Dropdown dropDownSuitValue;

    private void Start()
    {
        ClearAltarContainer();
        altarValueTextUI.text = "" + 0;
    }

    private void OnEnable()
    {
        TrickHandler.top10CardsSuits += PopulateCardsPanelUI;
        TrickHandler.onDiscardshowDeck += PopulateCardContainerDiscard4;
        TrickHandler.onDiscard4 += CloseTrickPanel;
        DiscardCardHandler.onDiscardCard += ActivateDeactivateTrick4SelectButton;
        TrickHandler.onTop5 += PopulateCardsContainerTop5;
    }

    private void OnDisable()
    {
        TrickHandler.top10CardsSuits -= PopulateCardsPanelUI;
        TrickHandler.onDiscardshowDeck -= PopulateCardContainerDiscard4;
        TrickHandler.onDiscard4 -= CloseTrickPanel;
        DiscardCardHandler.onDiscardCard -= ActivateDeactivateTrick4SelectButton;
        TrickHandler.onTop5 -= PopulateCardsContainerTop5;
    }

    #region Trick Discard 4
    //Activates the UI for the Trick Discard 4
    public void Discard4()
    {
        ActivatePanelCards();
        buttonTrickDiscard4ShowDeckUI.SetActive(true);
    }
    //Populates the container with the whole Deck when using the discard 4 Trick
    private void PopulateCardContainerDiscard4(List<Card> deck)
    {
        buttonTrickDiscard4ShowDeckUI.SetActive(false);
        buttonTrickDiscard4UI.SetActive(true);
        buttonTrickDiscard4UI.GetComponent<Button>().interactable = false;
        closePanelButtonUI.SetActive(false);

        foreach (Card cardInDeck in deck)
        {
            CardHandler cardInContainer = Instantiate(cardPrefabDiscard, cardContainerUI.transform);
            cardInContainer.Initialize(cardInDeck);
        }
    }
    
    private void ActivateDeactivateTrick4SelectButton(int totalCardsSelected)
    {
        buttonTrickDiscard4UI.GetComponent<Button>().interactable = totalCardsSelected > 0;
    }

    #endregion

    #region Trick Top 10 Suit and Value

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
    
    public void PopulateCardsPanelUI(List<Tuple<Card, bool>> top10CardsList)
    {
        DeactivateTrickPanelButtonsUI();
        
        foreach (var tuple in top10CardsList)
        {
            CardHandler cardTop10 = Instantiate(cardPrefab, cardContainerUI.transform);
            cardTop10.Initialize(tuple.Item1);

            if (tuple.Item2)
            {
                Image cardImage = cardTop10.GetComponent<Image>();
                cardImage.color = Color.red;
            }
        }
    }
    #endregion

    #region General Functions

    private void ClearAltarContainer()
    {
        //Deletes all items in the card container
        foreach (Transform item in cardAltarContainerUI.transform)
        {
            Destroy(item.gameObject);
        }
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
    
    public void DeactivateTrickPanelButtonsUI()
    {
        trickTop10SuitUI.SetActive(false);
    }
    
    private void ActivatePanelCards()
    {
        dropDownSuitValue.gameObject.SetActive(false);
        buttonTrickTop10SuitUI.SetActive(false);
        buttonTrickTop10ValueUI.SetActive(false);
        buttonTrickDiscard4UI.SetActive(false);
        trickPanelUI.SetActive(true);
        trickTop10SuitUI.SetActive(true);
        closePanelButtonUI.SetActive(true);
        ClearCardContainer();
    }
    
    

    #endregion

    #region Trick Look Top 5
    private void PopulateCardsContainerTop5(List<Card> top5Cardlist)
    {
        foreach (Card card in top5Cardlist)
        {
            CardHandler cardTop5 = Instantiate(cardPrefab, cardContainerUI.transform);
            cardTop5.Initialize(card);
        }
    }

    public void TrickTop5UI()
    {
        ActivatePanelCards();
    }

    #endregion

    #region Altar

    public void ActivateConfirmationPanel(bool activate)
    {
        confirmationPanelUI.SetActive(activate);
    }

    public void UpdateAltarValue(int altarValue)
    {
        altarValueTextUI.text = "" + altarValue;
    }

    #endregion
}
