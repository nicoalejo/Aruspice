using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerJAM : MonoBehaviour
{
    public const int StatCount = 4;
    public const int MinStat = 0;
    public const int MaxStat = 20;
    public const int StartingStat = 10;

    public static GameManagerJAM Instance { get; private set; }

    [Header("Card Spawning")]
    // Leave off while the GameFlowHandler drives the intro, on to skip straight to the game.
    [SerializeField] private bool startOnSceneLoad = false;
    [SerializeField] private CardDate cardPrefab;
    [SerializeField] private Transform cardParent;

    // Pool of characters and events. Cards are never used up: every draw takes
    // from the whole pool, minus the one on screen. Any size works.
    [Header("Card Pool (equal chance, never runs out)")]
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
    // The Image inside LosePanelUI that shows the art of the bad ending.
    [SerializeField] private Image gameOverImage;
    // The Image inside VictoryPanelUI that shows the art of the good ending.
    [SerializeField] private Image victoryImage;

    [Header("End Game Text")]
    // Names shown in the lose text, in stat1..stat4 order.
    [SerializeField]
    private string[] statNames = { "Créditos", "Tiempo", "Puntaje Social", "Estabilidad" };
    // One bad ending per stat and per direction, in the same order as statNames.
    [SerializeField]
    private StatEnding[] statEndings = new StatEnding[StatCount];
    // Used when the matching bad ending above was left empty.
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

    [Header("End Game Art")]
    // Used when the stat ending that closed the run has no art of its own.
    [SerializeField] private Sprite defaultLoseImage;
    // Shown when the run is won by surviving every card, so there is no character
    // to take the ending art from.
    [SerializeField] private Sprite winBySurvivalImage;

    [Header("Audio")]
    // The ring warns the player when a stat is close to ending the run.
    // It sounds below the low mark or above the high one.
    [SerializeField] private int lowStatWarning = 3;
    [SerializeField] private int highStatWarning = 18;

    // Raised every time the stats change, so the UI can refresh itself.
    public event Action OnStatsChanged;
    // Raised when any stat reaches 0 or 100.
    public event Action<int> OnStatOutOfBounds;
    // Raised once the run is over. True when the player won.
    public event Action<bool> OnGameOver;

    private readonly int[] stats = new int[StatCount];
    // How many dates the player has had with each character so far.
    private readonly Dictionary<SOCards, int> dateCounts = new Dictionary<SOCards, int>();
    // Reused by the draw so picking a card does not allocate every time.
    private readonly List<int> candidateIndices = new List<int>();
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

        PlaySound(accepted ? AudioManager.Gamesound.choose : AudioManager.Gamesound.discard);

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

        // Only warns while the run goes on: an ended run already has its own sound.
        PlayStatWarning();
        DrawNextCard();
    }

    // Rings when any stat is close to one of the limits.
    private void PlayStatWarning()
    {
        for (int i = 0; i < StatCount; i++)
        {
            if (stats[i] >= lowStatWarning && stats[i] <= highStatWarning) continue;

            PlaySound(AudioManager.Gamesound.ring);
            return;
        }
    }

    // Quiet when there is no AudioManager in the scene.
    private static void PlaySound(AudioManager.Gamesound sound)
    {
        if (AudioManager.instance == null) return;
        AudioManager.instance.PlayOnShotByDictionary(sound);
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

        PlaySound(won ? AudioManager.Gamesound.win : AudioManager.Gamesound.failure);

        if (won)
        {
            ShowEnding(victoryText, victoryImage, BuildVictoryMessage(dateWinner), GetVictoryImage(dateWinner));
            if (victoryPanel != null) victoryPanel.SetActive(true);
        }
        else
        {
            ShowEnding(gameOverText, gameOverImage, BuildLoseMessage(failedStat), GetLoseImage(failedStat));
            if (losePanel != null) losePanel.SetActive(true);
        }

        OnGameOver?.Invoke(won);
    }

    // Fills in the text and the art of an ending panel. Both references are
    // optional, so a panel can show only one of the two.
    private static void ShowEnding(TMP_Text label, Image image, string text, Sprite sprite)
    {
        if (label != null) label.text = text;
        if (image == null) return;

        image.sprite = sprite;
        // Turned off rather than left showing an empty box when no art was assigned.
        image.enabled = sprite != null;
    }

    private string BuildVictoryMessage(SOCards dateWinner)
    {
        if (dateWinner == null) return winBySurvivalMessage;

        string message = string.IsNullOrWhiteSpace(dateWinner.endingText)
            ? winByDatesMessage
            : dateWinner.endingText;

        return Format(message, dateWinner.DisplayName);
    }

    // The art of the character the run was won with, or the survival art when the
    // player won without settling on anyone.
    private Sprite GetVictoryImage(SOCards dateWinner)
    {
        if (dateWinner == null) return winBySurvivalImage;

        Sprite ending = dateWinner.EndingImage;
        return ending != null ? ending : winBySurvivalImage;
    }

    private string BuildLoseMessage(int failedStat)
    {
        if (failedStat < 0) return string.Empty;

        string statName = failedStat < statNames.Length ? statNames[failedStat] : $"Stat {failedStat + 1}";
        bool hitMax = HitMax(failedStat);

        StatEnding ending = GetStatEnding(failedStat);
        string message = ending != null ? (hitMax ? ending.atMax : ending.atMin) : null;
        if (string.IsNullOrWhiteSpace(message)) message = hitMax ? loseByMaxMessage : loseByMinMessage;

        return Format(message, statName);
    }

    private Sprite GetLoseImage(int failedStat)
    {
        if (failedStat < 0) return defaultLoseImage;

        StatEnding ending = GetStatEnding(failedStat);
        if (ending == null) return defaultLoseImage;

        Sprite image = HitMax(failedStat) ? ending.atMaxImage : ending.atMinImage;
        return image != null ? image : defaultLoseImage;
    }

    // The bad endings written for this stat, if there are any.
    private StatEnding GetStatEnding(int failedStat)
    {
        if (statEndings == null || failedStat < 0 || failedStat >= statEndings.Length) return null;
        return statEndings[failedStat];
    }

    // Which of the two limits ended the run.
    private bool HitMax(int failedStat)
    {
        if (failedStat < 0 || failedStat >= StatCount) return false;
        return stats[failedStat] >= MaxStat;
    }

    // The bespoke endings rarely need it, but {0} still works inside them. Text
    // with stray braces is shown as it was written instead of throwing.
    private static string Format(string message, string argument)
    {
        if (string.IsNullOrEmpty(message)) return message;

        try
        {
            return string.Format(message, argument);
        }
        catch (FormatException)
        {
            return message;
        }
    }

    #endregion

    #region Cards

    public void DrawNextCard()
    {
        // A run always opens with a person, never with an event.
        SOCards next = PickRandomCard(cardsPlayed == 0);
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

    private SOCards PickRandomCard(bool charactersOnly)
    {
        SOCards picked = PickFrom(charactersOnly);

        // Rather than showing nothing, take any card and let the designer know.
        if (picked == null && charactersOnly)
        {
            Debug.LogWarning("[GameManagerJAM] The deck has no character cards for the opening draw, using any card instead.", this);
            picked = PickFrom(false);
        }

        return picked;
    }

    // Every eligible card has the same chance, except the one currently on
    // screen, which cannot come out twice in a row.
    private SOCards PickFrom(bool charactersOnly)
    {
        candidateIndices.Clear();

        for (int i = 0; i < deck.Length; i++)
        {
            if (deck[i] == null) continue;
            if (charactersOnly && !deck[i].isCharacter) continue;
            if (i == lastCardIndex) continue;

            candidateIndices.Add(i);
        }

        // Nothing else to draw: repeat the card on screen if it still fits.
        if (candidateIndices.Count == 0)
        {
            bool lastIsUsable = lastCardIndex >= 0
                && deck[lastCardIndex] != null
                && (!charactersOnly || deck[lastCardIndex].isCharacter);

            return lastIsUsable ? deck[lastCardIndex] : null;
        }

        int index = candidateIndices[UnityEngine.Random.Range(0, candidateIndices.Count)];
        lastCardIndex = index;
        return deck[index];
    }

    #endregion
}

// The two bad endings of a single stat: one for bottoming out, one for overflowing.
// Each one can bring its own art; without it the default lose image is used.
[Serializable]
public class StatEnding
{
    [TextArea] public string atMin;
    public Sprite atMinImage;
    [TextArea] public string atMax;
    public Sprite atMaxImage;
}