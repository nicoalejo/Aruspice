using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cards' Types
public enum CardSuit
{
    Lamassu,
    Buho,
    Puertas,
    Sol
}

public class Card
{ 
    public CardSuit Suit { get; private set; }
    public int Value { get; private set; }

    public Card(CardSuit suit, int value)
    {
        Suit = suit;
        Value = value;
    }

    public override string ToString()
    {
        return "Suit: " + Suit.ToString() + ". Value: " + Value.ToString();
    }
}
