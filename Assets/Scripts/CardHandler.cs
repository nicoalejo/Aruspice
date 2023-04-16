using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class CardHandler : MonoBehaviour //, IPointerEnterHandler, IPointerExitHandler
{
    
    //TODO: Remove when img is added instead of text of prototype
    [SerializeField] private TextMeshProUGUI suitUI;
    [SerializeField] private TextMeshProUGUI valueUI;
    public Card CardData { get; private set; }

    

    public void Initialize(Card card)
    {
        CardData = card;
        // Update the card's appearance based on the suit and value (e.g., change the sprite or text)
        //TODO: Remove when img is added instead of text of prototype
        suitUI.text = CardData.Suit.ToString();
        valueUI.text = CardData.Value.ToString();
    }

}
