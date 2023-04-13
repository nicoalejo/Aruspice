using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckHandler : MonoBehaviour
{
    [SerializeField] private HandHandler handHandler;
    private List<Card> deck;

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
        deck = new List<Card>();
        foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
        {
            for (int value = 1; value <= 13; value++)
            {
                deck.Add(new Card(suit, value));
            }
        }
    }

    public void ShuffleDeck()
    {
        int n = deck.Count;
        for (int i = 0; i < n - 1; i++)
        {
            int randomIndex = Random.Range(i, n);
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public void DealCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (deck.Count > 0)
            {
                Card card = deck[0];
                deck.RemoveAt(0);
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
            Card card = deck[0];
            returnCards.Add(card);
            deck.RemoveAt(0);
            
        }

        return returnCards;
    }

    public void InsertCardAtTopDeck(Card card)
    {
        deck.Insert(0, card);
    }
    public void InsertCardsAtTopDeck(List<Card> cards)
    {
        foreach (var card in cards)
        {
            deck.Insert(0, card);
        }
    }

    public void PrintDeckConsole()
    {
        foreach (var card in deck)
        {
            Debug.Log(card.ToString());    
        }
    }
}
