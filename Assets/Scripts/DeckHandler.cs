using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckHandler : MonoBehaviour
{
    public const int DeckMaxValue = 13;
    
    [SerializeField] private HandHandler handHandler;
    public List<Card> Deck { get; private set; }


    private void Awake()
    {
        handHandler.Init();
        
        InitializeDeck();
        ShuffleDeck();
    }

    private void InitializeDeck()
    {
        Deck = new List<Card>();
        foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
        {
            String suitString = suit.ToString();
            String pathCardArt = suitString + "/" + suitString[0];
            
            for (int value = 1; value <= DeckMaxValue; value++)
            {
                Card newCard = new Card(suit, value);
                newCard.CardArt = Resources.Load<Sprite>(pathCardArt + $"{value}");
                Deck.Add(newCard);
            }
        }
    }

    public void ShuffleDeck()
    {
        int n = Deck.Count;
        for (int i = 0; i < n - 1; i++)
        {
            int randomIndex = Random.Range(i, n);
            (Deck[i], Deck[randomIndex]) = (Deck[randomIndex], Deck[i]);
        }
    }
    
    //Deal a X number of cards to hand
    public void DealCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (Deck.Count > 0)
            {
                Card card = Deck[0];
                Deck.RemoveAt(0);
                handHandler.AddCard(card);
            }
            else
            {
                Debug.LogWarning("No more cards in the deck!");
                break;
            }
        }
    }
    
    //Return a number of cards, removing them from the deck 
    public List<Card> GetCards(int numCards, bool removeFromDeck)
    {
        if (numCards < 1)
        {
            Debug.LogWarning("Asking for < 1 cards");
            return null;
        }

        List<Card> returnCards = new List<Card>();
        numCards = Math.Min(numCards, Deck.Count);  //Gets the min between the cards requested and the current cards in deck

        if (removeFromDeck)
        {
            for (int i = 0; i < numCards; i++)
            {
                Card card = Deck[0];
                returnCards.Add(card);
                Deck.RemoveAt(0);
            }    
        }
        else
        {
            for (int i = 0; i < numCards; i++)
            {
                returnCards.Add(Deck[i]);
            }
        }
        

        return returnCards;
    }

    public void InsertCardAtTopDeck(Card card)
    {
        Deck.Insert(0, card);
    }

    public void RemoveCards(List<Card> cardsRemove)
    {
        foreach (Card cardRemove in cardsRemove)
        {
            Deck.Remove(cardRemove);
        }
    }

    public void PrintDeckConsole()
    {
        Debug.Log("The Deck order is:");
        foreach (var card in Deck)
        {
            Debug.Log(card);
        }
    }
}
