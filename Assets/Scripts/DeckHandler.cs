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


    private void Start()
    {
        handHandler.Init();
        
        InitializeDeck();
        ShuffleDeck();
        PrintDeckConsole();
        DealCards(3);
    }

    private void InitializeDeck()
    {
        Deck = new List<Card>();
        foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
        {
            for (int value = 1; value <= DeckMaxValue; value++)
            {
                Deck.Add(new Card(suit, value));
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

    public List<Card> GetCards(int numCards)
    {
        if (numCards < 1)
        {
            Debug.LogWarning("Asking for < 1 cards");
            return null;
        }

        List<Card> returnCards = new List<Card>();

        for (int i = 0; i < numCards; i++)
        {
            Card card = Deck[0];
            returnCards.Add(card);
            Deck.RemoveAt(0);
            
        }

        return returnCards;
    }

    public void InsertCardAtTopDeck(Card card)
    {
        Deck.Insert(0, card);
    }
    public void InsertCardsAtTopDeck(List<Card> cards)
    {
        foreach (var card in cards)
        {
            Deck.Insert(0, card);
        }
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
