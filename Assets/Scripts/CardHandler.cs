using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Card CardData { get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(Card card)
    {
        CardData = card;
        // Update the card's appearance based on the suit and value (e.g., change the sprite or text)
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.parent.GetComponent<HandHandler>().cardsInHand.Contains(this))
        {
            rectTransform.DOScale(1.2f, 0.25f); // Scale up the card
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOScale(1f, 0.25f); // Scale down the card
    }

    public void SetCanvas(Canvas _canvas)
    {
        canvas = _canvas;
    }
}
