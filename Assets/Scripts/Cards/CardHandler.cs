using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class CardHandler : MonoBehaviour //, IPointerEnterHandler, IPointerExitHandler
{
    public Card CardData { get; private set; }
    
    public void Initialize(Card card)
    {
        CardData = card;
    }

}
