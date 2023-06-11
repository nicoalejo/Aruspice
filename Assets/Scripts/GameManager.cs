using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scenes")] 
    [SerializeField] private string mainMenuScene;
    [SerializeField] private string currentScene;

    [Header("Round Values")] 
    [SerializeField] private int numberOfRounds = 5;
    [SerializeField] private int numberOfActionPerRound = 3;
    [SerializeField] private int cardsToDealEachRound = 3;
    [SerializeField] private int expectedValue = 50;

    [Header("UI")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private RectTransform handZoneUI;
    [SerializeField] private RectTransform altarZoneUI;
    [SerializeField] private HandHandler handHandler;
    [SerializeField] private GameObject currentValueGO;
    
    [Header("Handlers")]
    [SerializeField] private TrickHandler trickHandler;
    [SerializeField] private DeckHandler deckHandler;
    [SerializeField] private TextHandler textHandler;

    private CardDragHandler currentCardToDropAltar;
    private List<Card> cardsInAltar = new ();
    private List<CardLogCalculation> cardsInAltarWithHandler = new();
    private int currentAltarValue = 0;
    private int currentRound = 1;
    private int actionsLeftThisRound;
    private bool isWin = false;
    private List<int> completedNumbers = new();

    private Dictionary<CardSuit, CardSuit> cardMultiplicationDictionary = new ();
    private Dictionary<CardSuit, CardSuit> cardSubtractDictionary = new ();
    
    private void Start()
    {
        InitMultiplicationDictionary();
        InitSubtractDictionary();
        
        AudioManager.instance.StartOnMainPlay(AudioManager.Gamesound.level01MainTheme);//Init the music
        
        actionsLeftThisRound = numberOfActionPerRound;
        uiManager.UpdateActionsValue(actionsLeftThisRound);
        uiManager.UpdateRoundValue(currentRound);

        deckHandler.DealCards(cardsToDealEachRound); //Deal cards at the start of game
        
        uiManager.UpdateDeckCardsLeftValue(deckHandler.Deck.Count); //Update deck cards left after deal the cards
        uiManager.ClearLogUI(); //Cleans the Log text
        
        completedNumbers = SaveManager.instance.GetCompletedNumbers();
        if (completedNumbers.Count == 0) //Shows intro text if the player has not completed any number
        {
            StartCoroutine(textHandler.Intro());    
        }
        else
        {
            uiManager.ActivateTextGameObject(false);
        }
        
    }
    
    //Define multiplication interaction between suits
    private void InitMultiplicationDictionary()
    {
        cardMultiplicationDictionary.Add(CardSuit.Puertas, CardSuit.Lamassu);
        cardMultiplicationDictionary.Add(CardSuit.Buho, CardSuit.Puertas);
        cardMultiplicationDictionary.Add(CardSuit.Frutas, CardSuit.Buho);
        cardMultiplicationDictionary.Add(CardSuit.Lamassu, CardSuit.Frutas);
    }
    
    //Define subtraction interaction between suits
    private void InitSubtractDictionary()
    {
        cardSubtractDictionary.Add(CardSuit.Puertas, CardSuit.Frutas);
        cardSubtractDictionary.Add(CardSuit.Frutas, CardSuit.Puertas);
        cardSubtractDictionary.Add(CardSuit.Buho, CardSuit.Lamassu);
        cardSubtractDictionary.Add(CardSuit.Lamassu, CardSuit.Buho);
    }

    private void OnEnable()
    {
        AltarDropZone.onCardDropOnAltar += CardDropOnAltar;
        TrickHandler.onActionTaken += ActionTaken;
        PickNumberHandler.onNumberPicked += NumberExpectedSelected;
    }

    private void OnDisable()
    {
        AltarDropZone.onCardDropOnAltar -= CardDropOnAltar;
        TrickHandler.onActionTaken -= ActionTaken;
        PickNumberHandler.onNumberPicked -= NumberExpectedSelected;
    }

    //Sets the current number to expect in the altar
    private void NumberExpectedSelected(int expectedNumber)
    {
        expectedValue = expectedNumber;                 //Set expected number in GameManager
        uiManager.UpdateExpectedValue(expectedValue);   //Update value in UI
        uiManager.HideShowPanelSelectAltarNumber(false); //Hide panel to choose numbers
    }

    //Activates the confirmation panel to drop card on altar and sets the currentCardToDropAltar
    private void CardDropOnAltar(CardDragHandler cardDragHandler)
    {
        uiManager.ActivateConfirmationPanel(true);
        cardDragHandler.GetComponent<RectTransform>().SetParent(altarZoneUI);
        currentCardToDropAltar = cardDragHandler;
    }
    
    public void CancelDropAltar()
    {
        currentCardToDropAltar.GetComponent<RectTransform>().SetParent(handZoneUI);
        uiManager.ActivateConfirmationPanel(false);
        AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.btnMainMenuSFX);
    }
    
    //If the player accepts the card dropped in the altar, sets that card as child of the altar GO
    public void AcceptDropAltar()
    {
        AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.cardAddAltar);  //SFX when adding card to altar
        CardHandler currentCardHandler = currentCardToDropAltar.GetComponent<CardHandler>();
        handHandler.RemoveCard(currentCardHandler);     //Remove from hand card list
        cardsInAltar.Add(currentCardHandler.CardData);  //Add to altar card list
        
        CreateCurrentValueGo(currentCardHandler);       //Creates new CurrentValueGO and adds it to the current card

        cardsInAltarWithHandler.Add(currentCardHandler.AddComponent<CardLogCalculation>()); //Adds the component to show the value calculation in Log
        Destroy(currentCardToDropAltar);                //Destroy script for dragging
        uiManager.ActivateConfirmationPanel(false);     //Deactivate panel to confirm
        CalculateAltarValue();                          //Recalculate current altar value
        uiManager.UpdateAltarValue(currentAltarValue);  //Update UI for altar value

        CheckWin();                    //Check if the expected value is reached
    }
    
    //Creates new object to show the current value of the card, and sets it's position
    private void CreateCurrentValueGo(CardHandler currentCardHandler)
    {
        Instantiate(currentValueGO, currentCardHandler.transform);
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(currentScene);
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    private void CheckWin()
    {
        if (currentAltarValue == expectedValue)
        {
            Debug.Log("You Won");
            isWin = true;
            HandleGameOver(isWin);
        }
        else
        {
            StartNewRound();    
        }
    }

    //Actions for new round
    private void StartNewRound()
    {
        currentRound++;
        if (currentRound > numberOfRounds || deckHandler.Deck.Count == 0)
        {
            Debug.Log("You Lost");
            isWin = false;
            HandleGameOver(isWin);
        }
        else
        {
            actionsLeftThisRound = numberOfActionPerRound;              //Sets actions back to max
            uiManager.UpdateRoundValue(currentRound);                   //Update UI current round
            uiManager.UpdateActionsValue(actionsLeftThisRound);         //Updates Action UI value
            handHandler.ClearHand(true);                          //Remove all cards in hand
            deckHandler.DealCards(cardsToDealEachRound);                //Deals 3 cards for the new round
            trickHandler.HasEnoughActionForTrick(actionsLeftThisRound); //Reactivates all tricks
            
            uiManager.UpdateDeckCardsLeftValue(deckHandler.Deck.Count); //Updates UI for deck cards left
        }
    }

    private void HandleGameOver(bool win)
    {
        if (win)
        {
            SaveManager.instance.Save(expectedValue);
            uiManager.ActivateWinPanel(expectedValue);
        }
        else
        {
            uiManager.ActivateLosePanel();
        }
    } 

    private void CalculateAltarValue()
    {
        List<Card> tempCalculationList = new List<Card>();
        
        //Add first card
        tempCalculationList.Add(new Card(cardsInAltar[0].Suit, cardsInAltar[0].Value));
        cardsInAltarWithHandler[0].SetInitialValueLog(cardsInAltar[0].Suit, cardsInAltar[0].Value);     //Sets the initial log value 
      
        //First we multiply
        for (int i = 1; i < cardsInAltar.Count; i++)
        {
            Card tempCardHolder = new Card(cardsInAltar[i].Suit, cardsInAltar[i].Value);
            cardsInAltarWithHandler[i].SetInitialValueLog(cardsInAltar[i].Suit, cardsInAltar[i].Value); //Init for the log
            cardMultiplicationDictionary.TryGetValue(tempCardHolder.Suit, out CardSuit previousCardSuit);
            if (previousCardSuit == cardsInAltar[i-1].Suit)
            {
                //Debug.Log("Multiplicación entre "+tempCardHolder.Suit+ " y " + cardsInAltar[i - 1].Suit);
                tempCardHolder.Value *= 2;
                cardsInAltarWithHandler[i].SetMultiplyLogInfo(cardsInAltar[i-1].Suit, tempCardHolder.Value); //Sets multiplier for log
            }
            tempCalculationList.Add(tempCardHolder);
        }

        //Then we subtract
        for (int i = 0; i < tempCalculationList.Count-1; i++)
        {
            cardSubtractDictionary.TryGetValue(tempCalculationList[i].Suit, out CardSuit nextCardSuit);
            if (nextCardSuit == tempCalculationList[i+1].Suit)
            {
                if (tempCalculationList[i].Value > 0)
                {
                    tempCalculationList[i].Value--;
                    cardsInAltarWithHandler[i].SetSubtractLogInfo(tempCalculationList[i+1].Suit, tempCalculationList[i].Value, false);    //Sets subtract for log
                }
                if (tempCalculationList[i+1].Value > 0)
                {
                    tempCalculationList[i+1].Value--;
                    cardsInAltarWithHandler[i+1].SetSubtractLogInfo(tempCalculationList[i].Suit, tempCalculationList[i+1].Value, true); //Sets subtract of next card for log
                }
            }
        }
        
        //Sum all values after calculations
        currentAltarValue = 0;

        for (int i = 0; i < tempCalculationList.Count; i++)
        {
            cardsInAltarWithHandler[i].GetComponentInChildren<CardCurrentValue>().UpdateCurrentValue(tempCalculationList[i].Value); //Sets the current value in the CardCurrentValue component of the card
            currentAltarValue += tempCalculationList[i].Value;
        }
    }

    //Calculates how many actions are taken based on the actions passed as parameter 
    //This is linked to a delegate that is invoked everytime a trick is performed
    //Also Updates the actions left and the cards left in deck.
    private int ActionTaken(int numberOfActionsTaken)
    {
        actionsLeftThisRound -= numberOfActionsTaken;
        uiManager.UpdateActionsValue(actionsLeftThisRound);
        uiManager.UpdateDeckCardsLeftValue(deckHandler.Deck.Count);
        return actionsLeftThisRound;
    }
}
