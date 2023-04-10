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

    private void ShuffleDeck()
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

    private void DealCards(int count)
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
}
