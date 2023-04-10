using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHandler : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int maxCards = 10;
    [SerializeField] private GameObject handUIContainer;
    [SerializeField] private Transform[] dropZones;
    
    [HideInInspector]
    public List<CardHandler> cardsInHand = new List<CardHandler>();
    
    public void AddCard(Card card)
    {
        if (cardsInHand.Count <= maxCards)
        {
            GameObject newCard = Instantiate(cardPrefab, handUIContainer.transform);
            CardHandler cardHandler = newCard.GetComponent<CardHandler>();
            cardHandler.SetCanvas(GetComponentInParent<Canvas>());
            cardHandler.Initialize(card);
            cardsInHand.Add(cardHandler);
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
