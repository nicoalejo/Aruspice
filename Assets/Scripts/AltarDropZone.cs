using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AltarDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent(out CardDragHandler cardDragHandler))
        {
            cardDragHandler.GetComponent<RectTransform>().SetParent(transform);
            Destroy(cardDragHandler);
        }
    }
}
