using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform ghostCardContainer;
    private RectTransform handZoneUI;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void Initialize(RectTransform _ghostCard, RectTransform _handZoneUI, Canvas _canvas)
    {
        ghostCardContainer = _ghostCard;
        handZoneUI = _handZoneUI;
        canvas = _canvas;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        ghostCardContainer.anchoredPosition = rectTransform.anchoredPosition;
        rectTransform.SetParent(ghostCardContainer);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ghostCardContainer.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        rectTransform.SetParent(handZoneUI);
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
