using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private RectTransform handZoneUI;
    [SerializeField] private RectTransform altarZoneUI;
    [SerializeField] private HandHandler handHandler;

    private CardDragHandler currentCardToDropAltar;
    private List<Card> cardsInAltar = new List<Card>();
    private int currentAltarValue = 0;

    private Dictionary<CardSuit, CardSuit> cardMultiplicationDictionary = new Dictionary<CardSuit, CardSuit>();
    private Dictionary<CardSuit, CardSuit> cardSubtractDictionary = new Dictionary<CardSuit, CardSuit>();

    private void Start()
    {
        InitMultiplicationDictionary();
        InitSubtractDictionary();
    }
    
    //Define multiplication interaction between suits
    private void InitMultiplicationDictionary()
    {
        cardMultiplicationDictionary.Add(CardSuit.Puertas, CardSuit.Lamassu);
        cardMultiplicationDictionary.Add(CardSuit.Buho, CardSuit.Puertas);
        cardMultiplicationDictionary.Add(CardSuit.Frutas, CardSuit.Buho);
        cardMultiplicationDictionary.Add(CardSuit.Lamassu, CardSuit.Frutas);
    }
    
    //Define subtraction interaction between suits
    private void InitSubtractDictionary()
    {
        cardSubtractDictionary.Add(CardSuit.Puertas, CardSuit.Frutas);
        cardSubtractDictionary.Add(CardSuit.Frutas, CardSuit.Puertas);
        cardSubtractDictionary.Add(CardSuit.Buho, CardSuit.Lamassu);
        cardSubtractDictionary.Add(CardSuit.Lamassu, CardSuit.Buho);
    }

    private void OnEnable()
    {
        AltarDropZone.onCardDropOnAltar += CardDropOnAltar;
    }

    private void OnDisable()
    {
        AltarDropZone.onCardDropOnAltar -= CardDropOnAltar;
    }

    private void CardDropOnAltar(CardDragHandler cardDragHandler)
    {
        uiManager.ActivateConfirmationPanel(true);
        cardDragHandler.GetComponent<RectTransform>().SetParent(altarZoneUI);
        currentCardToDropAltar = cardDragHandler;
    }
    
    public void CancelDropAltar()
    {
        currentCardToDropAltar.GetComponent<RectTransform>().SetParent(handZoneUI);
        uiManager.ActivateConfirmationPanel(false);
    }

    public void AcceptDropAltar()
    {
        CardHandler currentCardHandler = currentCardToDropAltar.GetComponent<CardHandler>();
        handHandler.RemoveCard(currentCardHandler);     //Remove from hand
        cardsInAltar.Add(currentCardHandler.CardData);  //Add to altar
        Destroy(currentCardToDropAltar);                //Destroy script for dragging
        uiManager.ActivateConfirmationPanel(false);     //Deactivate panel to confirm
        CalculateAltarValue();                          //Recalculate current altar value
        uiManager.UpdateAltarValue(currentAltarValue);  //Update UI for altar value
    }

    private void CalculateAltarValue()
    {
        List<Card> tempCalculationList = new List<Card>();
        
        //Add first card
        tempCalculationList.Add(new Card(cardsInAltar[0].Suit, cardsInAltar[0].Value));
        
        //First we multiply
        for (int i = 1; i < cardsInAltar.Count; i++)
        {
            Card tempCardHolder = new Card(cardsInAltar[i].Suit, cardsInAltar[i].Value);
            cardMultiplicationDictionary.TryGetValue(tempCardHolder.Suit, out CardSuit previousCardSuit);
            if (previousCardSuit == cardsInAltar[i-1].Suit)
            {
                Debug.Log("Multiplicación entre "+tempCardHolder.Suit+ " y " + cardsInAltar[i - 1].Suit);
                tempCardHolder.Value *= 2;
            }
           
            tempCalculationList.Add(tempCardHolder);
        }

        //Then we subtract
        for (int i = 0; i < tempCalculationList.Count-1; i++)
        {
            cardSubtractDictionary.TryGetValue(tempCalculationList[i].Suit, out CardSuit nextCardSuit);
            if (nextCardSuit == tempCalculationList[i+1].Suit)
            {
                if (tempCalculationList[i].Value > 0)
                    tempCalculationList[i].Value--;
                if (tempCalculationList[i+1].Value > 0)
                    tempCalculationList[i+1].Value--;
            }
        }
        
        //Sum all values after calculations
        currentAltarValue = 0;
        
        foreach (Card tempCard in tempCalculationList)
        {
            currentAltarValue += tempCard.Value;
        }
        
        //cardsInAltar
    }
}
