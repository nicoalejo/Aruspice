using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private RectTransform handZoneUI;
    [SerializeField] private RectTransform altarZoneUI;

    private CardDragHandler currentCardToDropAltar;
    
    private void OnEnable()
    {
        AltarDropZone.onCardDropOnAltar += CardDropOnAltar;
    }

    private void OnDisable()
    {
        AltarDropZone.onCardDropOnAltar -= CardDropOnAltar;
    }

    private void CardDropOnAltar(CardDragHandler cardDragHandler)
    {
        uiManager.ActivateConfirmationPanel(true);
        cardDragHandler.GetComponent<RectTransform>().SetParent(altarZoneUI);
        currentCardToDropAltar = cardDragHandler;
    }
    
    public void CancelDropAltar()
    {
        currentCardToDropAltar.GetComponent<RectTransform>().SetParent(handZoneUI);
        uiManager.ActivateConfirmationPanel(false);
    }

    public void AcceptDropAltar()
    {
        Destroy(currentCardToDropAltar);
        uiManager.ActivateConfirmationPanel(false);
    }
}
