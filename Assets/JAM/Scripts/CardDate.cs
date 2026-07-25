using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDate : MonoBehaviour
{
    [Header("Card Data")]
    [SerializeField] private SOCards card;

    [Header("Card Content")]
    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text flavorText;

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

    public void SetCard(SOCards newCard)
    {
        card = newCard;
        if (card == null) return;

        CacheBaseColors();

        if (portrait != null) portrait.sprite = card.image;
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