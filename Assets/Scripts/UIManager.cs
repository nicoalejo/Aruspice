using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Cards Prefabs")]
    [SerializeField] private CardHandler cardPrefab;        //Prefab with drag integrated for hand zone
    [SerializeField] private CardHandler cardPrefabNoDrag;  //Prefab with no drag for all the tricks
    [SerializeField] private CardHandler cardPrefabDiscard; //Prefab without drag script to show on tricks
    
    [Header("Actions and Rounds")]
    [SerializeField] private TextMeshProUGUI actionsLeftTextUI; //Text for actions left this round 
    [SerializeField] private TextMeshProUGUI currentRoundTextUI;//Text for current round
    
    [Header("Front UI")]
    [SerializeField] private GameObject cardContainerUI;            //Container for cards in hand
    [SerializeField] private GameObject cardAltarContainerUI;       //Container for cards in altar
    [SerializeField] private TextMeshProUGUI altarValueTextUI;       //Altar value text
    [SerializeField] private TextMeshProUGUI expectedValueUI;
    [SerializeField] private TextMeshProUGUI deckCardsLeft;
    [SerializeField] private GameObject panelSelectNumberAltarUI;   //Panel for selecting number for altar at the begging of the game
    [SerializeField] private GameObject panelInstructionsUI;        //Panel for instructions
    [SerializeField] private GameObject panelPauseUI;               //Panel for pause
    
    [Header("Tricks")]
    [SerializeField] private GameObject trickPanelUI;
    [SerializeField] private GameObject trickTop10SuitUI;
    [SerializeField] private GameObject buttonTrickTop10SuitUI;
    [SerializeField] private GameObject buttonTrickTop10ValueUI;
    [SerializeField] private GameObject buttonTrickDiscard4ShowDeckUI;
    [SerializeField] private GameObject buttonTrickDiscard4UI;
    [SerializeField] private GameObject closePanelButtonUI;
    [SerializeField] private GameObject confirmationPanelUI;
    [SerializeField] private Dropdown dropDownSuitValue;
    
    [Header("Game Over Panel")]
    [SerializeField] private GameObject winPanelUI;
    [SerializeField] private GameObject winImagesPanelUI;
    [SerializeField] private GameObject losePanelUI;
    [SerializeField] private GameObject textGameObjectUI;
    [SerializeField] private TextHandler textHandler;

    [Header("Log Panel")]
    [SerializeField] private TextMeshProUGUI titleLogUI;
    [SerializeField] private TextMeshProUGUI descriptionLogUI;
    
    private void Start()
    {
        ClearAltarContainer();
        altarValueTextUI.text = "" + 0;
    }

    private void OnEnable()
    {
        TrickHandler.top10CardsSuits += PopulateCardsPanelUI;
        TrickHandler.onDiscardshowDeck += PopulateCardContainerDiscard4;
        TrickHandler.onDiscard4 += CloseTrickPanel;
        DiscardCardHandler.onDiscardCard += ActivateDeactivateTrick4SelectButton;
        TrickHandler.onTop5 += PopulateCardsContainerTop5;
        TextHandler.onTextComplete += StartActivateWinPanel;
        ControlsHandler.onEventEscape += EscapePause;
    }

    private void OnDisable()
    {
        TrickHandler.top10CardsSuits -= PopulateCardsPanelUI;
        TrickHandler.onDiscardshowDeck -= PopulateCardContainerDiscard4;
        TrickHandler.onDiscard4 -= CloseTrickPanel;
        DiscardCardHandler.onDiscardCard -= ActivateDeactivateTrick4SelectButton;
        TrickHandler.onTop5 -= PopulateCardsContainerTop5;
        TextHandler.onTextComplete -= StartActivateWinPanel;
        ControlsHandler.onEventEscape -= EscapePause;
    }
    
    private void EscapePause()
    {
        panelPauseUI.SetActive(!panelPauseUI.activeSelf);
    }

    public void OpenInstructions()
    {
        panelInstructionsUI.SetActive(true);
    }

    public void CloseInstructions()
    {
        panelInstructionsUI.SetActive(false);
    }
    
    public void ClearLogUI()
    {
        titleLogUI.text = "";
        descriptionLogUI.text = "";
    }

    public void FillLogUI(string title, string description)
    {
        titleLogUI.text = title;
        descriptionLogUI.text = description;
    }

    #region Trick Discard 4
    //Activates the UI for the Trick Discard 4
    public void Discard4()
    {
        PlayTrickBtnSFX();
        
        ActivatePanelCards();
        buttonTrickDiscard4ShowDeckUI.SetActive(true);
    }
    //Populates the container with the whole Deck when using the discard 4 Trick
    private void PopulateCardContainerDiscard4(List<Card> deck)
    {
        buttonTrickDiscard4ShowDeckUI.SetActive(false);
        buttonTrickDiscard4UI.SetActive(true);
        buttonTrickDiscard4UI.GetComponent<Button>().interactable = false;
        closePanelButtonUI.SetActive(false);

        foreach (Card cardInDeck in deck)
        {
            CardHandler cardInContainer = Instantiate(cardPrefabDiscard, cardContainerUI.transform);
            cardInContainer.Initialize(cardInDeck);
            cardInContainer.GetComponent<Image>().sprite = cardInDeck.CardArt;
        }
    }
    
    //Activates/Deactivates the button for the discard 4 trick if the selected cards are > 0 or less than 1
    private void ActivateDeactivateTrick4SelectButton(int totalCardsSelected)
    {
        buttonTrickDiscard4UI.GetComponent<Button>().interactable = totalCardsSelected > 0;
    }

    #endregion

    #region Trick Top 10 Suit and Value

    //Activates the UI for the Trick Top 10 Suit
    public void Top10TrickSuitUI()
    {
        PlayTrickBtnSFX();
        
        ActivatePanelCards();
        dropDownSuitValue.gameObject.SetActive(true);
        
        PopulateDropdownSuit();
        buttonTrickTop10SuitUI.SetActive(true);
    }
    
    //Activates the UI for the Trick Top 10 Value
    public void Top10TrickValueUI()
    {
        PlayTrickBtnSFX();
        
        ActivatePanelCards();
        dropDownSuitValue.gameObject.SetActive(true);
        
        PopulateDropdownValue();
        buttonTrickTop10ValueUI.SetActive(true);
    }
    
    //Populates dropdown with values from 1 to 13, taking the DeckMaxValue
    private void PopulateDropdownValue()
    {
        dropDownSuitValue.ClearOptions();
        List<string> dropdownValues = new List<string>();
        for (int i = 1; i <= DeckHandler.DeckMaxValue; i++)
        {
            dropdownValues.Add(i.ToString());
        }
        dropDownSuitValue.AddOptions(dropdownValues);
    }

    private void PopulateDropdownSuit()
    {
        dropDownSuitValue.ClearOptions();
        List<string> enumNames = new List<string>(Enum.GetNames(typeof(CardSuit)));
        dropDownSuitValue.AddOptions(enumNames);
    }
    
    public void PopulateCardsPanelUI(List<Tuple<Card, bool>> top10CardsList)
    {
        DeactivateTrickPanelButtonsUI();
        
        foreach (var tuple in top10CardsList)
        {
            CardHandler cardTop10 = Instantiate(cardPrefabNoDrag, cardContainerUI.transform);
            cardTop10.Initialize(tuple.Item1);
            cardTop10.GetComponent<Image>().sprite = tuple.Item1.CardArt;

            if (tuple.Item2)
            {
                Image cardImage = cardTop10.GetComponent<Image>();
                cardImage.color = Color.red;
            }
        }
    }
    #endregion

    #region General Functions

    public void HideShowPanelSelectAltarNumber(bool show)
    {
        panelSelectNumberAltarUI.SetActive(show);
    }
    private void ClearAltarContainer()
    {
        //Deletes all items in the card container
        foreach (Transform item in cardAltarContainerUI.transform)
        {
            Destroy(item.gameObject);
        }
    }
    private void ClearCardContainer()
    {
        //Deletes all items in the card container
        foreach (Transform item in cardContainerUI.transform)
        {
            Destroy(item.gameObject);
        }
    }

    public void CloseTrickPanel()
    {
        PlayTrickBtnSFX();
        trickPanelUI.SetActive(false);
    }
    
    public void DeactivateTrickPanelButtonsUI()
    {
        trickTop10SuitUI.SetActive(false);
    }
    
    private void ActivatePanelCards()
    {
        dropDownSuitValue.gameObject.SetActive(false);
        buttonTrickTop10SuitUI.SetActive(false);
        buttonTrickTop10ValueUI.SetActive(false);
        buttonTrickDiscard4UI.SetActive(false);
        buttonTrickDiscard4ShowDeckUI.SetActive(false);
        trickPanelUI.SetActive(true);
        trickTop10SuitUI.SetActive(true);
        closePanelButtonUI.SetActive(true);
        ClearCardContainer();
    }
    
    public void UpdateRoundValue(int newValue)
    {
        currentRoundTextUI.text = "" + newValue;
    }

    public void UpdateActionsValue(int newValue)
    {
        actionsLeftTextUI.text = "" + newValue;
    }

    public void UpdateExpectedValue(int newValue)
    {
        expectedValueUI.text = "" + newValue;
    }

    public void ActivateWinPanel(int winValue)
    {
        StartCoroutine(ActivateWinPanelCoroutine(winValue));
    }

    private IEnumerator ActivateWinPanelCoroutine(int winValue)
    {
        ActivateTextGameObject(true);
        yield return StartCoroutine(textHandler.ShowTextInGO(TextWinManager.instance.GetTextById(winValue)));
        
        ActivateWinPanelWithImages(false);
    }
    
    private void StartActivateWinPanel(bool isTextIntro)
    {
        if (isTextIntro)
        {
            StartCoroutine(DelayAndActivate(0.5f, isTextIntro));    
        }
        else
        {
            ActivateWinPanelWithImages(isTextIntro);    
        }
    }

    private IEnumerator DelayAndActivate(float timeToDelay, bool isTextIntro)
    {
        yield return new WaitForSeconds(timeToDelay);
        ActivateWinPanelWithImages(isTextIntro);
    }
    
    private void ActivateWinPanelWithImages(bool isTextIntro)
    {
        ActivateTextGameObject(false);
        if (!isTextIntro)
        {
            winPanelUI.SetActive(true);
            ActivateImagesWinPanel();    
        }
    }

    private void ActivateImagesWinPanel()
    {
        List<int> achievedNumbers = SaveManager.instance.GetCompletedNumbers();
        foreach (Transform imageInWinPanel in winImagesPanelUI.GetComponentInChildren<Transform>())
        {
            //Parse the name of the image to int
            int.TryParse(imageInWinPanel.name, out int nameNumberImage);
            if (achievedNumbers.Contains(nameNumberImage))
            {
                imageInWinPanel.gameObject.SetActive(true);
            }
        }
        
    }

    
    
    public void ActivateLosePanel()
    {
        losePanelUI.SetActive(true);
    }

    public void UpdateDeckCardsLeftValue(int cardsLeft)
    {
        deckCardsLeft.text = cardsLeft.ToString();
    } 
    
    public void ActivateTextGameObject(bool isActive)
    {
        textGameObjectUI.SetActive(isActive);
    }

    #endregion

    #region Trick Look Top 5
    private void PopulateCardsContainerTop5(List<Card> top5Cardlist)
    {
        foreach (Card card in top5Cardlist)
        {
            CardHandler cardTop5 = Instantiate(cardPrefabNoDrag, cardContainerUI.transform);
            cardTop5.Initialize(card);
            cardTop5.GetComponent<Image>().sprite = card.CardArt;

        }
    }

    public void TrickTop5UI()
    {
        PlayTrickBtnSFX();
        ActivatePanelCards();
    }

    #endregion

    #region Altar

    public void ActivateConfirmationPanel(bool activate)
    {
        confirmationPanelUI.SetActive(activate);
    }

    public void UpdateAltarValue(int altarValue)
    {
        altarValueTextUI.text = "" + altarValue;
    }

    #endregion

    private void PlayTrickBtnSFX()
    {
        AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.btnMainMenuSFX);
    }
    


}
