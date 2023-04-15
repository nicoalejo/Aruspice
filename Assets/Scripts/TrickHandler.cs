using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrickHandler : MonoBehaviour
{
    public delegate void Top10CardsSuits(List<Tuple<Card, bool>> top10CardsList);
    public static event Top10CardsSuits top10CardsSuits;
    
    public delegate void OnDiscardShowDeck(List<Card> deck);
    public static event OnDiscardShowDeck onDiscardshowDeck;
    
    public delegate void OnTop5(List<Card> top5CardList);
    public static event OnTop5 onTop5;

    public static Action onDiscard4; 
    
    [SerializeField] private DeckHandler deckHandler;
    [SerializeField] private GameObject cardContainerUI;
    [SerializeField] private Dropdown dropdownTop10;

    private List<Card> discard4List = new List<Card>();

    private void OnEnable()
    {
        DiscardCardHandler.onDiscardCardSelected += DiscardCardSelected;
    }

    private void OnDisable()
    {
        DiscardCardHandler.onDiscardCardSelected -= DiscardCardSelected;
    }

    public void Draw()
    {
        deckHandler.DealCards(1);
    }
    
    //Executes Top 10 Suit and Value Tricks
    //Called directly through the buttons using unity events
    public void Top10SuitValue(Button buttonPressed)
         {
             List<Card> top10Cards = deckHandler.GetCards(10, true);
             List<Card> cardsToAddDeck = new List<Card>();
             List<Tuple<Card, bool>> cardsKeepDiscard = new List<Tuple<Card, bool>>();

             if (buttonPressed.CompareTag("TrickTop10Suit"))
             {
                 CardSuit.TryParse(dropdownTop10.options[dropdownTop10.value].text, true, out CardSuit cardsuitCompare);
                 for (int i = 0; i < top10Cards.Count; i++)
                 {
                     //Add true if the card is the same has the suit, this card will be discarded
                     //Add false if the card is not the same, this card will be kept
                     bool isCardToDiscard = top10Cards[i].Suit == cardsuitCompare;
                     cardsKeepDiscard.Add(new Tuple<Card, bool>(top10Cards[i], isCardToDiscard));
                 
                     if(!isCardToDiscard)
                         cardsToAddDeck.Add(top10Cards[i]);
                 }
             }
             else if (buttonPressed.CompareTag("TrickTop10Value"))
             {
                 int.TryParse(dropdownTop10.options[dropdownTop10.value].text, out int valueToDiscard); 
                 
                 for (int i = 0; i < top10Cards.Count; i++)
                 {
                     //If card value is less than the chosen value it will be discarded
                     bool isCardToDiscard = top10Cards[i].Value <= valueToDiscard;
                     cardsKeepDiscard.Add(new Tuple<Card, bool>(top10Cards[i], isCardToDiscard));
                 
                     if(!isCardToDiscard)
                         cardsToAddDeck.Add(top10Cards[i]);
                 }
             }
             
             for (int i = cardsToAddDeck.Count-1; i >= 0; i--)
             {
                 deckHandler.InsertCardAtTopDeck(cardsToAddDeck[i]);    
             }

             top10CardsSuits?.Invoke(cardsKeepDiscard);
         }

    public void Discard4ShowDeck()
    {
        onDiscardshowDeck?.Invoke(deckHandler.Deck);
    }

    private void DiscardCardSelected(Card cardSelected, bool isSelected)
    {
        if (isSelected)
        {
            discard4List.Add(cardSelected);    
        }
        else
        {
            discard4List.Remove(cardSelected);
        }
    }
    
    public void Discard4()
    {
        deckHandler.RemoveCards(discard4List);
        DiscardCardHandler.cardsSelected = 0;
        onDiscard4?.Invoke();
    }

    public void ShuffleDeck()
    {
        deckHandler.ShuffleDeck();
        Debug.Log("Deck Shuffled");
    }

    public void ShowTop5Cards()
    {
        onTop5?.Invoke(deckHandler.GetCards(5, false));
    }
}
