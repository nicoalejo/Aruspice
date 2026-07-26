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
    [SerializeField] private GameObject[] acceptedIcons = new GameObject[4];
    [SerializeField] private GameObject[] rejectedIcons = new GameObject[4];

    [Header("Arrow Prefabs")]
    [SerializeField] private GameObject upArrowPrefab;
    [SerializeField] private GameObject downArrowPrefab;

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

        AddArrows(acceptedIcons, card.accepted);
        AddArrows(rejectedIcons, card.rejected);
    }

    private Color[] CacheColors(GameObject[] icons)
    {
        var colors = new Color[icons.Length];
        for (int i = 0; i < icons.Length; i++)
        {
            if (icons[i] != null)
            {
                var image = icons[i].GetComponent<Image>();
                colors[i] = image != null ? image.color : Color.white;
            }
            else
            {
                colors[i] = Color.white;
            }
        }
        return colors;
    }

    private void TintIcons(GameObject[] icons, Color[] baseColors, CardChoiceStats stats)
    {
        int[] values = stats.ToArray();

        for (int i = 0; i < icons.Length && i < values.Length; i++)
        {
            if (icons[i] == null) continue;

            Color targetColor;
            if (values[i] > 0) targetColor = positiveColor;
            else if (values[i] < 0) targetColor = negativeColor;
            else targetColor = baseColors[i];

            // Tint all Image components except arrow instances
            var images = icons[i].GetComponentsInChildren<Image>();
            foreach (var image in images)
            {
                // Skip arrow instances
                if (image.transform.name == "Arrow_Instance") continue;
                
                image.color = targetColor;
            }
        }
    }

    private void AddArrows(GameObject[] icons, CardChoiceStats stats)
    {
        int[] values = stats.ToArray();

        for (int i = 0; i < icons.Length && i < values.Length; i++)
        {
            if (icons[i] == null) continue;

            ClearArrows(icons[i]);

            if (values[i] > 0)
            {
                if (upArrowPrefab != null)
                {
                    GameObject arrow = Instantiate(upArrowPrefab, icons[i].transform);
                    arrow.name = "Arrow_Instance";
                }
            }
            else if (values[i] < 0)
            {
                if (downArrowPrefab != null)
                {
                    GameObject arrow = Instantiate(downArrowPrefab, icons[i].transform);
                    arrow.name = "Arrow_Instance";
                }
            }
        }
    }

    private void ClearArrows(GameObject iconParent)
    {
        for (int i = iconParent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = iconParent.transform.GetChild(i);
            if (child.name == "Arrow_Instance")
            {
                Destroy(child.gameObject);
            }
        }
    }
}