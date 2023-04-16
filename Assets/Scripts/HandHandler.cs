using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandHandler : MonoBehaviour
{
    [SerializeField] private CardHandler cardPrefab;
    [SerializeField] private int maxCards = 6;
    [SerializeField] private RectTransform handUIContainer;
    [SerializeField] private Transform[] dropZones;
    [SerializeField] private RectTransform ghostCardHandler;
    [SerializeField] private Canvas canvas; 
    
    [HideInInspector]
    public List<CardHandler> cardsInHand = new List<CardHandler>();
    
    public void AddCard(Card card)
    {
        if (cardsInHand.Count < maxCards)
        {
            CardHandler newCard = Instantiate(cardPrefab, handUIContainer);
            newCard.Initialize(card);
            newCard.gameObject.GetComponent<CardDragHandler>().Initialize(ghostCardHandler, handUIContainer, canvas);
            cardsInHand.Add(newCard);
        }
        else
        {
            Debug.LogWarning("Hand is full!");
        }
    }

    public void Init()
    {
        ClearHand(false);
    }
    
    public void ClearHand(bool discard)
    {
        //Moves all cards to discard pile previous destroying them from hand
        if (discard)
        {
            
        }
        //Deletes all current items int hand
        foreach (Transform item in handUIContainer.transform)
        {
            Destroy(item.gameObject);
        }
    }
}
