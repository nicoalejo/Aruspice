using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HandHandler : MonoBehaviour
{
    [SerializeField] private CardHandler cardPrefab;
    [SerializeField] private int maxCards = 6;
    [SerializeField] private RectTransform handUIContainer;
    [SerializeField] private RectTransform ghostCardHandler;
    [SerializeField] private Canvas canvas; 
    
    private List<CardHandler> cardsInHand = new();
    
    public void AddCard(Card card)
    {
        if (cardsInHand.Count < maxCards)
        {
            CardHandler newCard = Instantiate(cardPrefab, handUIContainer);
            newCard.Initialize(card);                                                           //Initialize Card
            newCard.GetComponent<CardDragHandler>().Initialize(ghostCardHandler, handUIContainer, canvas); //Set Card Handler Drag
            newCard.GetComponent<Image>().sprite = card.CardArt;                                //Set card art
            AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.cardDealSFX);   //Play deal card sfx
            cardsInHand.Add(newCard);
        }
        else
        {
            Debug.LogWarning("Hand is full!");
        }
    }

    public void RemoveCard(CardHandler card)
    {
        cardsInHand.Remove(card);
    }

    public void Init()
    {
        ClearHand(false);
    }
    
    public void ClearHand(bool discard)
    {
        cardsInHand.Clear();
        //Moves all cards to discard pile previous destroying them from hand
        if (discard)
        {
            //TODO: move discard cards to discard pile
        }
        //Deletes all current items int hand
        foreach (Transform item in handUIContainer.transform)
        {
            Destroy(item.gameObject);
        }
    }
}
