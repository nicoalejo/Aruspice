using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrickHandler : MonoBehaviour
{
    public delegate void Top10CardsSuits(List<Tuple<Card, bool>> top10CardsList);
    public static event Top10CardsSuits top10CardsSuits;
    
    public delegate void OnDiscardShowDeck(List<Card> deck);
    public static event OnDiscardShowDeck onDiscardshowDeck;
    
    public delegate void OnTop5(List<Card> top5CardList);
    public static event OnTop5 onTop5;
    public delegate int OnActionTaken(int actionsSpent);
    public static event OnActionTaken onActionTaken;

    public static Action onDiscard4; 
    
    [SerializeField] private DeckHandler deckHandler;
    [SerializeField] private Dropdown dropdownTop10;
    
    [Header("Actions taken per each trick")]
    [SerializeField] private int drawAction = 1;
    [SerializeField] private int top10SuitValueAction = 1;
    [SerializeField] private int discardAction = 2;
    [SerializeField] private int shuffleAction = 1;
    [SerializeField] private int lookTop5Action = 1;
    
    [Header("Trick Buttons")]
    //[SerializeField] private Button drawButton;
    [SerializeField] private Button top10SuitButton;
    [SerializeField] private Button top10ValueButton;
    [SerializeField] private Button discardButton;
    [SerializeField] private Button shuffleButton;
    [SerializeField] private Button lookTop5Button;
    
    private List<Card> discard4List = new List<Card>();
    private List<Tuple<Button, int>> trickButtonsActionsList = new List<Tuple<Button, int>>();

    private void Start()
    {
        //trickButtonsActionsList.Add(new Tuple<Button, int>(drawButton,drawAction));
        trickButtonsActionsList.Add(new Tuple<Button, int>(top10SuitButton,top10SuitValueAction));
        trickButtonsActionsList.Add(new Tuple<Button, int>(top10ValueButton,top10SuitValueAction));
        trickButtonsActionsList.Add(new Tuple<Button, int>(discardButton,discardAction));
        trickButtonsActionsList.Add(new Tuple<Button, int>(shuffleButton,shuffleAction));
        trickButtonsActionsList.Add(new Tuple<Button, int>(lookTop5Button,lookTop5Action));
    }

    private void OnEnable()
    {
        DiscardCardHandler.onDiscardCardSelected += DiscardCardSelected;
    }

    private void OnDisable()
    {
        DiscardCardHandler.onDiscardCardSelected -= DiscardCardSelected;
    }

    public void Draw()
    {
        deckHandler.DealCards(1);
        HasEnoughActionForTrick(onActionTaken?.Invoke(drawAction));
    }
    
     //Executes Top 10 Suit and Value Tricks
     //Called directly through the buttons using unity events
     public void Top10SuitValue(Button buttonPressed)
     {
         List<Card> top10Cards = deckHandler.GetCards(10, true);
         List<Card> cardsToAddDeck = new List<Card>();
         List<Tuple<Card, bool>> cardsKeepDiscard = new List<Tuple<Card, bool>>();
         
         PlayBtnSFX();

         if (buttonPressed.CompareTag("TrickTop10Suit"))
         {
             CardSuit.TryParse(dropdownTop10.options[dropdownTop10.value].text, true, out CardSuit cardsuitCompare);
             for (int i = 0; i < top10Cards.Count; i++)
             {
                 //Add true if the card is the same has the suit, this card will be discarded
                 //Add false if the card is not the same, this card will be kept
                 bool isCardToDiscard = top10Cards[i].Suit == cardsuitCompare;
                 cardsKeepDiscard.Add(new Tuple<Card, bool>(top10Cards[i], isCardToDiscard));
             
                 if(!isCardToDiscard)
                     cardsToAddDeck.Add(top10Cards[i]);
             }
         }
         else if (buttonPressed.CompareTag("TrickTop10Value"))
         {
             int.TryParse(dropdownTop10.options[dropdownTop10.value].text, out int valueToDiscard); 
             
             for (int i = 0; i < top10Cards.Count; i++)
             {
                 //If card value is less than the chosen value it will be discarded
                 bool isCardToDiscard = top10Cards[i].Value <= valueToDiscard;
                 cardsKeepDiscard.Add(new Tuple<Card, bool>(top10Cards[i], isCardToDiscard));
             
                 if(!isCardToDiscard)
                     cardsToAddDeck.Add(top10Cards[i]);
             }
         }
         
         for (int i = cardsToAddDeck.Count-1; i >= 0; i--)
         {
             deckHandler.InsertCardAtTopDeck(cardsToAddDeck[i]);    
         }

         top10CardsSuits?.Invoke(cardsKeepDiscard);
         HasEnoughActionForTrick(onActionTaken?.Invoke(top10SuitValueAction));
     }

    public void Discard4ShowDeck()
    {
        PlayBtnSFX();
        onDiscardshowDeck?.Invoke(deckHandler.Deck);
    }

    private void DiscardCardSelected(Card cardSelected, bool isSelected)
    {
        if (isSelected)
        {
            discard4List.Add(cardSelected);    
        }
        else
        {
            discard4List.Remove(cardSelected);
        }
    }
    
    public void Discard4()
    {
        deckHandler.RemoveCards(discard4List);
        deckHandler.ShuffleDeck();
        PlayShuffleBtnSFX();
        Debug.Log("Deck Shuffled");
        DiscardCardHandler.cardsSelected = 0;
        onDiscard4?.Invoke();
        HasEnoughActionForTrick(onActionTaken?.Invoke(discardAction));
    }

    public void ShuffleDeck()
    {
        PlayShuffleBtnSFX();
        deckHandler.ShuffleDeck();
        HasEnoughActionForTrick(onActionTaken?.Invoke(shuffleAction));
        Debug.Log("Deck Shuffled");
    }

    public void ShowTop5Cards()
    {
        onTop5?.Invoke(deckHandler.GetCards(5, false));
        HasEnoughActionForTrick(onActionTaken?.Invoke(lookTop5Action));
    }

    public void HasEnoughActionForTrick(int? actionsLeft)
    {
        //If deck has 0 cards it deactivates all buttons
        if (deckHandler.Deck.Count == 0)
        {
            foreach (Tuple<Button,int> buttonsActions in trickButtonsActionsList)
            {
                buttonsActions.Item1.interactable = false;
            }
        }
        else
        {
            //Checks if the button's action cost is less than the actions left
            foreach (Tuple<Button,int> buttonsActions in trickButtonsActionsList)
            {
                if (buttonsActions.Item2 > actionsLeft)
                {
                    buttonsActions.Item1.interactable = false;
                }
                else
                {
                    buttonsActions.Item1.interactable = true;
                }
            }    
        }
        
        
    }
    
    private void PlayShuffleBtnSFX()
    {
        AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.cardShuffleSFX);
    }
    
    private void PlayBtnSFX()
    {
        AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.btnMainMenuSFX);
    }
    
}
