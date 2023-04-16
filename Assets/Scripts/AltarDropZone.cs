using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AltarDropZone : MonoBehaviour, IDropHandler
{
    public delegate void OnCardDropOnAltar(CardDragHandler cardDragHandler);

    public static OnCardDropOnAltar onCardDropOnAltar;
        
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent(out CardDragHandler cardDragHandler))
        {
            onCardDropOnAltar?.Invoke(cardDragHandler);
        }
    }          
           
}
