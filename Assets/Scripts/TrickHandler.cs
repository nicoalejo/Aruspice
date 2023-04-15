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
    
    //Executes Top 10 Suit and Value Tricks
    //Called directly through the buttons using unity events
    public void Top10SuitValue(Button buttonPressed)
         {
             List<Card> top10Cards = deckHandler.GetCards(10);
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
}
