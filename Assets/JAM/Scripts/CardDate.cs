using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardDate : MonoBehaviour
{
    private SOCards card;

    [Header("Card Content")]
    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text flavorText;

    [Header("Choice Buttons")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;

    [Header("Stat Icons (same order as stat1..stat4)")]
    [SerializeField] private Image[] acceptedIcons = new Image[4];
    [SerializeField] private Image[] rejectedIcons = new Image[4];

    [Header("Tint Colors")]
    [SerializeField] private Color positiveColor = new Color(0.24f, 0.85f, 0.31f);
    [SerializeField] private Color negativeColor = new Color(0.90f, 0.24f, 0.24f);

    // Colors the icons had in the prefab, restored whenever a stat change is 0.
    private Color[] acceptedBaseColors;
    private Color[] rejectedBaseColors;

    void Awake()
    {
        CacheBaseColors();
    }

    private void CacheBaseColors()
    {
        if (acceptedBaseColors != null) return;
        acceptedBaseColors = CacheColors(acceptedIcons);
        rejectedBaseColors = CacheColors(rejectedIcons);
    }

    void Start()
    {
        if (card != null) SetCard(card);
    }

    // Called by the GameManager once, right after the card is instantiated.
    // The buttons live inside the prefab, so they cannot reference the manager
    // from the Inspector and have to be hooked up here instead.
    public void BindChoiceButtons(UnityAction onAccept, UnityAction onReject)
    {
        if (acceptButton != null)
        {
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(onAccept);
        }
        else Debug.LogWarning("[CardDate] Accept button (BtnOK) is not assigned.", this);

        if (rejectButton != null)
        {
            rejectButton.onClick.RemoveAllListeners();
            rejectButton.onClick.AddListener(onReject);
        }
        else Debug.LogWarning("[CardDate] Reject button (BtnReject) is not assigned.", this);
    }

    // Blocks input while the card is animating out / the next one is coming in.
    public void SetButtonsInteractable(bool interactable)
    {
        if (acceptButton != null) acceptButton.interactable = interactable;
        if (rejectButton != null) rejectButton.interactable = interactable;
    }

    public void SetCard(SOCards newCard)
    {
        card = newCard;
        if (card == null) return;

        CacheBaseColors();

        if (portrait != null) portrait.sprite = card.image;
        if (characterName != null) characterName.text = card.DisplayName;
        if (flavorText != null) flavorText.text = card.flavorText;

        TintIcons(acceptedIcons, acceptedBaseColors, card.accepted);
        TintIcons(rejectedIcons, rejectedBaseColors, card.rejected);
    }

    private Color[] CacheColors(Image[] icons)
    {
        var colors = new Color[icons.Length];
        for (int i = 0; i < icons.Length; i++)
        {
            colors[i] = icons[i] != null ? icons[i].color : Color.white;
        }
        return colors;
    }

    private void TintIcons(Image[] icons, Color[] baseColors, CardChoiceStats stats)
    {
        int[] values = stats.ToArray();

        for (int i = 0; i < icons.Length && i < values.Length; i++)
        {
            if (icons[i] == null) continue;

            if (values[i] > 0) icons[i].color = positiveColor;
            else if (values[i] < 0) icons[i].color = negativeColor;
            else icons[i].color = baseColors[i];
        }
    }
}