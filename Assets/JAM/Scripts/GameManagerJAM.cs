using System;
using UnityEngine;

public class GameManagerJAM : MonoBehaviour
{
    public const int StatCount = 4;
    public const int MinStat = 0;
    public const int MaxStat = 100;
    public const int StartingStat = 50;

    public static GameManagerJAM Instance { get; private set; }

    [Header("Card Spawning")]
    [SerializeField] private CardDate cardPrefab;
    [SerializeField] private Transform cardParent;

    [Header("Deck (10 cards, all with the same probability)")]
    [SerializeField] private SOCards[] deck = new SOCards[10];

    // Raised every time the stats change, so the UI can refresh itself.
    public event Action OnStatsChanged;
    // Raised when any stat reaches 0 or 100.
    public event Action<int> OnStatOutOfBounds;

    private readonly int[] stats = new int[StatCount];
    private CardDate currentCard;
    private SOCards currentCardData;
    private int lastCardIndex = -1;

    public SOCards CurrentCardData => currentCardData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ResetStats();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        DrawNextCard();
    }

    #region Stats
    
    //TODO: Change so it proceeds to game ending when the stats are out of bounds
    public int GetStat(int index)
    {
        if (index < 0 || index >= StatCount) return 0;
        return stats[index];
    }

    public void ResetStats()
    {
        for (int i = 0; i < StatCount; i++) stats[i] = StartingStat;
        OnStatsChanged?.Invoke();
    }

    public void ApplyStats(CardChoiceStats change)
    {
        if (change == null) return;

        int[] values = change.ToArray();
        for (int i = 0; i < StatCount && i < values.Length; i++)
        {
            //TODO: change the Clamp so when it reaches or surpass 0 or 100 it activates endgame screen, 
            stats[i] = Mathf.Clamp(stats[i] + values[i], MinStat, MaxStat);
        }

        OnStatsChanged?.Invoke();
        
        for (int i = 0; i < StatCount; i++)
        {
            if (stats[i] <= MinStat || stats[i] >= MaxStat) OnStatOutOfBounds?.Invoke(i);
        }
    }

    #endregion

    #region Choices

    // Hook these to the accept / reject buttons of the card.
    public void AcceptCard()
    {
        ResolveCard(true);
    }

    public void RejectCard()
    {
        ResolveCard(false);
    }

    private void ResolveCard(bool accepted)
    {
        if (currentCardData == null) return;

        ApplyStats(accepted ? currentCardData.accepted : currentCardData.rejected);
        DrawNextCard();
    }

    #endregion

    #region Cards

    public void DrawNextCard()
    {
        SOCards next = PickRandomCard();
        if (next == null)
        {
            Debug.LogWarning("[GameManagerJAM] The deck has no valid cards assigned.", this);
            return;
        }

        currentCardData = next;
        SpawnCardIfNeeded();
        if (currentCard != null) currentCard.SetCard(currentCardData);
    }

    private void SpawnCardIfNeeded()
    {
        if (currentCard != null) return;

        if (cardPrefab == null)
        {
            Debug.LogError("[GameManagerJAM] No card prefab assigned.", this);
            return;
        }

        Transform parent = cardParent != null ? cardParent : transform;
        currentCard = Instantiate(cardPrefab, parent);
        currentCard.BindChoiceButtons(AcceptCard, RejectCard);
    }

    // Every card has the same chance, except the one currently on screen,
    // which cannot come out twice in a row.
    private SOCards PickRandomCard()
    {
        int valid = 0;
        for (int i = 0; i < deck.Length; i++)
        {
            if (deck[i] != null) valid++;
        }
        if (valid == 0) return null;

        // With a single usable card there is nothing else to draw, so repeat it.
        bool avoidLast = valid > 1 && lastCardIndex >= 0;

        int candidates = avoidLast ? valid - 1 : valid;
        int pick = UnityEngine.Random.Range(0, candidates);

        for (int i = 0; i < deck.Length; i++)
        {
            if (deck[i] == null) continue;
            if (avoidLast && i == lastCardIndex) continue;

            if (pick == 0)
            {
                lastCardIndex = i;
                return deck[i];
            }
            pick--;
        }

        return null;
    }

    #endregion
}