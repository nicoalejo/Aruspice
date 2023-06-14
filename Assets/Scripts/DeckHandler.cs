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
    public List<Card> Deck { get; private set; } = new ();


    private void Awake()
    {
        handHandler.Init();

        InitializeDeck();
        ShuffleDeck();
    }
        

    private void InitDeckTutorial()
    {
        Card newCard = new Card(CardSuit.Buho, 13);
        newCard.CardArt = Resources.Load<Sprite>("Buho/B13");
        Deck.Add(newCard);
        
        newCard = new Card(CardSuit.Lamassu, 4);
        newCard.CardArt = Resources.Load<Sprite>("Lamassu/L4");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Buho, 6);
        newCard.CardArt = Resources.Load<Sprite>("Buho/B6");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 13);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P13");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Frutas, 5);
        newCard.CardArt = Resources.Load<Sprite>("Frutas/F5");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 11);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P11");
        Deck.Add(newCard);
        
        newCard = new Card(CardSuit.Frutas, 11);
        newCard.CardArt = Resources.Load<Sprite>("Frutas/F11");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Frutas, 6);
        newCard.CardArt = Resources.Load<Sprite>("Frutas/F6");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Frutas, 3);
        newCard.CardArt = Resources.Load<Sprite>("Frutas/F3");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 7);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P7");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Buho, 10);
        newCard.CardArt = Resources.Load<Sprite>("Buho/B10");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 1);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P1");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Lamassu, 11);
        newCard.CardArt = Resources.Load<Sprite>("Lamassu/L11");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Frutas, 8);
        newCard.CardArt = Resources.Load<Sprite>("Frutas/F8");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 12);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P12");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 6);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P6");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 5);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P5");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Buho, 4);
        newCard.CardArt = Resources.Load<Sprite>("Buho/B4");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Buho, 3);
        newCard.CardArt = Resources.Load<Sprite>("Buho/B3");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Lamassu, 9);
        newCard.CardArt = Resources.Load<Sprite>("Lamassu/L9");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 8);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P8");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Lamassu, 12);
        newCard.CardArt = Resources.Load<Sprite>("Lamassu/L12");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Frutas, 10);
        newCard.CardArt = Resources.Load<Sprite>("Frutas/F10");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Lamassu, 7);
        newCard.CardArt = Resources.Load<Sprite>("Lamassu/L7");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Buho, 2);
        newCard.CardArt = Resources.Load<Sprite>("Buho/B2");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 4);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P4");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Lamassu, 2);
        newCard.CardArt = Resources.Load<Sprite>("Lamassu/L2");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 10);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P10");
        Deck.Add(newCard);
        
        newCard = new Card(CardSuit.Lamassu, 13);
        newCard.CardArt = Resources.Load<Sprite>("Lamassu/L13");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Puertas, 9);
        newCard.CardArt = Resources.Load<Sprite>("Puertas/P9");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Buho, 11);
        newCard.CardArt = Resources.Load<Sprite>("Buho/B11");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Lamassu, 3);
        newCard.CardArt = Resources.Load<Sprite>("Lamassu/L3");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Frutas, 13);
        newCard.CardArt = Resources.Load<Sprite>("Frutas/F13");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Buho, 8);
        newCard.CardArt = Resources.Load<Sprite>("Buho/B8");
        Deck.Add(newCard);

        newCard = new Card(CardSuit.Lamassu, 5);
        newCard.CardArt = Resources.Load<Sprite>("Lamassu/L5");
        Deck.Add(newCard);
    }

    private void InitializeDeck()
    {
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
