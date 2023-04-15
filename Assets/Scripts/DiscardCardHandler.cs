using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardCardHandler : MonoBehaviour
{
    public delegate void OnDiscardCardSelected(Card cardSelected, bool isSelected);

    public static event OnDiscardCardSelected onDiscardCardSelected;
    
    public delegate void OnDiscardCard(int countDiscardSelected);

    public static event OnDiscardCard onDiscardCard;
    
    [HideInInspector]
    public static int cardsSelected = 0;
    
    private Image cardImage;
    private bool hasBeenSelected = false;
    private CardHandler cardHandler;
    
    private void Start()
    {
        cardHandler = GetComponent<CardHandler>();
        cardImage = GetComponent<Image>();
    }

    public void SelectCard()
    {
        if (!hasBeenSelected && cardsSelected < 4)
        {
            cardImage.color = Color.red;
            hasBeenSelected = !hasBeenSelected;
            cardsSelected++;
            onDiscardCardSelected?.Invoke(cardHandler.CardData, hasBeenSelected);
        }
        else if(hasBeenSelected)
        {
            cardImage.color = Color.blue;
            hasBeenSelected = !hasBeenSelected;
            cardsSelected--;
            onDiscardCardSelected?.Invoke(cardHandler.CardData, hasBeenSelected);
        }
    }
}
