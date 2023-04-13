using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrickHandler : MonoBehaviour
{
    [SerializeField] private DeckHandler deckHandler;
    
    [SerializeField] private GameObject cardContainerUI;
    [SerializeField] private Dropdown dropdownTop10;

    public delegate void Top10CardsSuits(List<Tuple<Card, bool>> top10CardsList);
    public static event Top10CardsSuits top10CardsSuits; 

    public void Draw()
    {
        deckHandler.DealCards(1);
    }

    public void Top10Suit()
         {
             List<Card> top10Cards = deckHandler.GetCards(10);
             List<Card> cardsToAddDeck = new List<Card>();
             List<Tuple<Card, bool>> cardsKeepDiscard = new List<Tuple<Card, bool>>();
     
             for (int i = 0; i < top10Cards.Count; i--)
             {
                 Enum.TryParse(dropdownTop10.ToString(), out CardSuit cardsuitCompare);
                 //Add true if the card is the same has the suit, this card will be discarded
                 //Add false if the card is not the same, this card will be kept
                 cardsKeepDiscard.Add(new Tuple<Card, bool>(top10Cards[i], top10Cards[i].Suit == cardsuitCompare));
                 
                 if(top10Cards[i].Suit != cardsuitCompare)
                    cardsToAddDeck.Add(top10Cards[i]);
             }

             for (int i = cardsToAddDeck.Count; i > 1; i++)
             {
                 deckHandler.InsertCardAtTopDeck(cardsToAddDeck[i]);    
             }
             
             top10CardsSuits?.Invoke(cardsKeepDiscard);
         }
}
