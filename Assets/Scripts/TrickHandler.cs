using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickHandler : MonoBehaviour
{
    [SerializeField] private DeckHandler deckHandler;
    
    [SerializeField] private GameObject cardContainerUI;
    void Start()
    {
        
    }

    public void Draw()
    {
        deckHandler.DealCards(1);
    }

    public void Top10Suit()
    {
        List<Card> top10Cards = deckHandler.GetCards(10);
        
    }
}
