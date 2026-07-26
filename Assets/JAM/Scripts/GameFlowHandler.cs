using TMPro;
using UnityEngine;

// Drives the opening of the run: start panel -> intro text -> game.
public class GameFlowHandler : MonoBehaviour
{
    private enum Phase { Start, Intro, Game }

    [Header("Start Panel")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private TMP_Text startText;
    [SerializeField] private string startMessage = "Da click para empezar";

    [Header("Intro Text")]
    // The ShowText panel and the TextHandler that types the intro.
    [SerializeField] private GameObject showTextPanel;
    [SerializeField] private TextHandler textHandler;

    [Header("Game")]
    // The Game object inside the canvas, holding the card and the stat bars.
    [SerializeField] private GameObject gameRoot;
    [SerializeField] private GameManagerJAM gameManager;

    private Phase phase = Phase.Start;

    void Awake()
    {
        if (startText != null)
        {
            startText.text = startMessage;
            startText.gameObject.SetActive(true);
        }

        if (startPanel != null) startPanel.SetActive(true);
        if (showTextPanel != null) showTextPanel.SetActive(false);
        if (gameRoot != null) gameRoot.SetActive(false);
    }

    void Start()
    {
        // Started here and not in Awake: the AudioManager builds its sources in
        // its own Awake, and the order between the two is not guaranteed.
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StartOnMainPlay(AudioManager.Gamesound.introTheme);
        }

        // Both steps are advanced by clicking, and the clicks come from here.
        if (FindObjectOfType<ControlsHandler>() == null)
        {
            Debug.LogError("[GameFlowHandler] No ControlsHandler in the scene, so no click will ever arrive and the intro cannot be passed.", this);
        }
    }

    private void OnEnable()
    {
        ControlsHandler.onEventLeftMouse += HandleClick;
        TextHandler.onTextComplete += HandleTextComplete;
    }

    private void OnDisable()
    {
        ControlsHandler.onEventLeftMouse -= HandleClick;
        TextHandler.onTextComplete -= HandleTextComplete;
    }

    private void HandleClick()
    {
        // Once the intro is running the clicks belong to the TextHandler.
        if (phase != Phase.Start) return;
        BeginIntro();
    }

    private void BeginIntro()
    {
        phase = Phase.Intro;

        if (startPanel != null) startPanel.SetActive(false);
        if (showTextPanel != null) showTextPanel.SetActive(true);

        if (textHandler != null)
        {
            // Started on the TextHandler itself: it stops its own coroutine when
            // the player clicks to skip the typing.
            textHandler.StartCoroutine(textHandler.Intro());
        }
        else
        {
            // Nothing to read, go straight to the game.
            BeginGame();
        }
    }

    private void HandleTextComplete(bool isTextIntro)
    {
        if (phase != Phase.Intro) return;
        BeginGame();
    }

    private void BeginGame()
    {
        phase = Phase.Game;

        // The intro song gives way to the one that plays for the whole run, with
        // the ambience looping underneath it.
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StartOnMainPlay(AudioManager.Gamesound.gameTheme);
            AudioManager.instance.StartAmbience();
        }

        if (showTextPanel != null) showTextPanel.SetActive(false);
        // Activated before the manager starts, so the stat bars can measure themselves.
        if (gameRoot != null) gameRoot.SetActive(true);

        if (gameManager != null) gameManager.StartGame();
        else Debug.LogError("[GameFlowHandler] No GameManagerJAM assigned, the game cannot start.", this);
    }
}