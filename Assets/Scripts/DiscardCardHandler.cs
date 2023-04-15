using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardCardHandler : MonoBehaviour
{
    private Image cardImage;
    private bool hasBeenSelected = false;
    private CardHandler cardHandler;

    private void Start()
    {
        cardHandler = GetComponent<CardHandler>();
        cardImage = GetComponent<Image>();
    }

    //public delegate 
    
    public void SelectCard()
    {
        Debug.Log("This Card has been Clicked");
        if (hasBeenSelected)
        {
            cardImage.color = Color.blue;
            hasBeenSelected = !hasBeenSelected;
        }
        else
        {
            cardImage.color = Color.red;
            hasBeenSelected = !hasBeenSelected;
        }
    }
}
