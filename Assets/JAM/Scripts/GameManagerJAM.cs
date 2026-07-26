using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerJAM : MonoBehaviour
{
    public const int StatCount = 4;
    public const int MinStat = 0;
    public const int MaxStat = 100;
    public const int StartingStat = 50;

    public static GameManagerJAM Instance { get; private set; }

    [Header("Card Spawning")]
    // Leave off while the GameFlowHandler drives the intro, on to skip straight to the game.
    [SerializeField] private bool startOnSceneLoad = false;
    [SerializeField] private CardDate cardPrefab;
    [SerializeField] private Transform cardParent;

    [Header("Deck (10 cards, all with the same probability)")]
    [SerializeField] private SOCards[] deck = new SOCards[10];

    [Header("UI")]
    [SerializeField] private StatsBarUI statsBar;

    [Header("End Game")]
    // Cards that have to be survived to win the run.
    [SerializeField] private int cardsToWin = 10;
    // Dates with the same character that win the run on their own.
    [SerializeField] private int datesToWin = 7;
    // When on, only accepted cards count as a date with that character.
    [SerializeField] private bool onlyAcceptedCardsAreDates = true;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject victoryPanel;
    // The GameOverText inside LosePanelUI.
    [SerializeField] private TMP_Text gameOverText;
    // The GameOverText inside VictoryPanelUI.
    [SerializeField] private TMP_Text victoryText;

    [Header("End Game Text")]
    // Names shown in the lose text, in stat1..stat4 order.
    [SerializeField]
    private string[] statNames = { "Créditos", "Tiempo", "Puntaje Social", "Estabilidad" };
    // {0} is replaced with the name of the stat that ended the run.
    [SerializeField]
    private string loseByMinMessage = "{0} llegó a cero. ¡El altar te ha abandonado!";
    [SerializeField]
    private string loseByMaxMessage = "{0} se desbordó. ¡El altar te ha consumido!";
    // {0} is replaced with the name of the character you dated the most.
    [SerializeField]
    private string winByDatesMessage = "¡{0} es tu media naranja! El altar bendice su unión.";
    [SerializeField]
    private string winBySurvivalMessage = "¡Sobreviviste a todas las citas! El altar queda satisfecho.";

    // Raised every time the stats change, so the UI can refresh itself.
    public event Action OnStatsChanged;
    // Raised when any stat reaches 0 or 100.
    public event Action<int> OnStatOutOfBounds;
    // Raised once the run is over. True when the player won.
    public event Action<bool> OnGameOver;

    private readonly int[] stats = new int[StatCount];
    // How many dates the player has had with each character so far.
    private readonly Dictionary<SOCards, int> dateCounts = new Dictionary<SOCards, int>();
    private CardDate currentCard;
    private SOCards currentCardData;
    private int lastCardIndex = -1;
    private int cardsPlayed;
    private bool gameOver;

    public SOCards CurrentCardData => currentCardData;
    public int CardsPlayed => cardsPlayed;
    public bool IsGameOver => gameOver;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Safety net in case a panel was left visible in the editor.
        if (losePanel != null) losePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        ResetStats();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        // Off by default: the GameFlowHandler starts the run once the intro is over.
        if (startOnSceneLoad) StartGame();
    }

    // Begins a run from scratch. Safe to call again to restart.
    public void StartGame()
    {
        gameOver = false;
        cardsPlayed = 0;
        lastCardIndex = -1;
        dateCounts.Clear();

        if (losePanel != null) losePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        ResetStats();

        if (currentCard != null) currentCard.SetButtonsInteractable(true);
        DrawNextCard();
    }

    #region Stats

    public int GetStat(int index)
    {
        if (index < 0 || index >= StatCount) return 0;
        return stats[index];
    }

    public void ResetStats()
    {
        for (int i = 0; i < StatCount; i++) stats[i] = StartingStat;

        // Snapped, not animated: this is the starting state, not a change.
        RefreshStatsUI(false);
        OnStatsChanged?.Invoke();
    }

    private void RefreshStatsUI(bool animated)
    {
        if (statsBar != null) statsBar.SetAll(stats, animated);
    }

    // Returns the index of the first stat that went out of bounds, or -1 when
    // every stat stayed inside the limits.
    public int ApplyStats(CardChoiceStats change)
    {
        if (change == null) return -1;

        int failedStat = -1;
        int[] values = change.ToArray();

        for (int i = 0; i < StatCount && i < values.Length; i++)
        {
            // Checked before clamping, so overshooting the limit still counts as a loss.
            int raw = stats[i] + values[i];
            stats[i] = Mathf.Clamp(raw, MinStat, MaxStat);

            if (raw > MinStat && raw < MaxStat) continue;

            if (failedStat < 0) failedStat = i;
            OnStatOutOfBounds?.Invoke(i);
        }

        RefreshStatsUI(true);
        OnStatsChanged?.Invoke();

        return failedStat;
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
        if (gameOver || currentCardData == null) return;

        SOCards playedCard = currentCardData;
        int failedStat = ApplyStats(accepted ? playedCard.accepted : playedCard.rejected);
        cardsPlayed++;

        // Losing takes priority: busting a stat on the last card is still a loss.
        if (failedStat >= 0)
        {
            EndGame(false, failedStat, null);
            return;
        }

        // The more specific win is checked first, so it gets to name the character.
        if (CountDate(playedCard, accepted) >= datesToWin)
        {
            EndGame(true, -1, playedCard);
            return;
        }

        if (cardsPlayed >= cardsToWin)
        {
            EndGame(true, -1, null);
            return;
        }

        DrawNextCard();
    }

    // Returns how many dates the player has had with that character.
    // Event cards are not people, so they never add up to a date.
    private int CountDate(SOCards card, bool accepted)
    {
        if (card == null || !card.isCharacter) return 0;
        if (onlyAcceptedCardsAreDates && !accepted) return GetDateCount(card);

        dateCounts.TryGetValue(card, out int count);
        count++;
        dateCounts[card] = count;
        return count;
    }

    public int GetDateCount(SOCards card)
    {
        if (card == null) return 0;
        dateCounts.TryGetValue(card, out int count);
        return count;
    }

    #endregion

    #region End Game

    // dateWinner is the character the run was won with, or null when the player
    // won by simply surviving every card.
    private void EndGame(bool won, int failedStat, SOCards dateWinner)
    {
        if (gameOver) return;
        gameOver = true;

        if (currentCard != null) currentCard.SetButtonsInteractable(false);

        if (won)
        {
            if (victoryText != null) victoryText.text = BuildVictoryMessage(dateWinner);
            if (victoryPanel != null) victoryPanel.SetActive(true);
        }
        else
        {
            if (gameOverText != null) gameOverText.text = BuildLoseMessage(failedStat);
            if (losePanel != null) losePanel.SetActive(true);
        }

        OnGameOver?.Invoke(won);
    }

    private string BuildVictoryMessage(SOCards dateWinner)
    {
        if (dateWinner == null) return winBySurvivalMessage;
        return string.Format(winByDatesMessage, dateWinner.DisplayName);
    }

    private string BuildLoseMessage(int failedStat)
    {
        if (failedStat < 0) return string.Empty;

        string statName = failedStat < statNames.Length ? statNames[failedStat] : $"Stat {failedStat + 1}";
        bool hitMax = stats[failedStat] >= MaxStat;

        return string.Format(hitMax ? loseByMaxMessage : loseByMinMessage, statName);
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