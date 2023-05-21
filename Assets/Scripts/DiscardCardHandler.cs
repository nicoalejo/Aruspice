using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardCardHandler : MonoBehaviour
{
    //Delegates that sends the card selected and if it was selected or diselected
    public delegate void OnDiscardCardSelected(Card cardSelected, bool isSelected);

    public static event OnDiscardCardSelected onDiscardCardSelected;
    
    //Delegate that sends how many cards are currently selected 
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
            AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.cardSelectSFX);
            cardImage.color = Color.red;
            hasBeenSelected = !hasBeenSelected;
            cardsSelected++;
            onDiscardCardSelected?.Invoke(cardHandler.CardData, hasBeenSelected);
            onDiscardCard?.Invoke(cardsSelected);
        }
        else if(hasBeenSelected)
        {
            AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.cardSelectSFX);
            cardImage.color = Color.white;
            hasBeenSelected = !hasBeenSelected;
            cardsSelected--;
            onDiscardCardSelected?.Invoke(cardHandler.CardData, hasBeenSelected);
            onDiscardCard?.Invoke(cardsSelected);
        }
    }
}
