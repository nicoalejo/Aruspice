using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Transform ghostCardContainer;
    private Transform handZoneUI;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void Initialize(Transform _ghostCard, Transform _handZoneUI)
    {
        ghostCardContainer = _ghostCard;
        handZoneUI = _handZoneUI;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        ghostCardContainer.transform.position = transform.position;
        transform.SetParent(ghostCardContainer);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ghostCardContainer.position = eventData.position;
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(handZoneUI);
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag.TryGetComponent(out AltarDropZone droppedCard))
        {
            
        }
    }
    // public void OnPointerEnter(PointerEventData eventData)
    // {
    //     if (transform.parent.GetComponent<HandHandler>().cardsInHand.Contains(this))
    //     {
    //         transform.DOScale(1.2f, 0.25f); // Scale up the card
    //     }
    // }
    //
    // public void OnPointerExit(PointerEventData eventData)
    // {
    //     transform.DOScale(1f, 0.25f); // Scale down the card
    // }
}
