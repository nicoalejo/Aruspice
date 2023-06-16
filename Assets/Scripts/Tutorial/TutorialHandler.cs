using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialHandler : MonoBehaviour
{
    [Header("Intro")]
    [SerializeField] private TextMeshProUGUI textUI;
    [TextArea]
    [SerializeField] private string introText;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private int letterPerSecond;
    
    [Header("Panels")]
    [SerializeField] private TextMeshProUGUI numberActionsText;
    [SerializeField] private TextMeshProUGUI numberRoundText;
    [SerializeField] private TextMeshProUGUI numberAltarText;
    [SerializeField] private GameObject numberPanel;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject trickPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject[] trickContent;
    [SerializeField] private GameObject[] objectsNeeded;
    [SerializeField] private GameObject[] tutorialTexts = new GameObject[26];
    
    [SerializeField] private String sceneMainMenu;

    private bool isTextComplete = false;
    private String currentText = "";
    private Coroutine textCoroutine;
    private int textIndex = 0;
    private int objectIndex = 0;
    private int contentIndex = 0;

    private void Start()
    {
        StartCoroutine(Intro());
    }

    private void OnEnable()
    {
        ControlsHandler.onEventLeftMouse += ContinueAll;
    }

    private void OnDisable()
    {
        ControlsHandler.onEventLeftMouse -= ContinueAll;
    }

    //Shows the intro screen and then load the first level
    public IEnumerator Intro()
    {
        currentText = introText;
        textCoroutine = StartCoroutine(TypeDialog(currentText));
        yield return textCoroutine;
        yield return new WaitForSeconds(4f);
        introPanel.SetActive(false);
    }

    //Recieves a textUI and a string and shows the text smoothly in the textUI
    private IEnumerator TypeDialog(string line)
    {
        textUI.text = "";
        foreach (var letter in line.ToCharArray())
        {
            textUI.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
    }

    private void ContinueAll()
    {
        if (!isTextComplete)
        {
            StopCoroutine(textCoroutine);
            textUI.text = currentText;
            isTextComplete = true;
        }
        else 
        {
            introPanel.SetActive(false);
        }
    }
    
    //Activate the next text for the tutorial
    public void ActivateNextText()
    {
        tutorialTexts[textIndex].SetActive(false);
        textIndex++;
        switch (textIndex)
        {
            case 1:
                numberPanel.SetActive(false);
                break;
            case 7:
                instructionsPanel.SetActive(true);
                break;
            case 8:
                instructionsPanel.SetActive(false);
                break;
            case 10:
                trickPanel.SetActive(true);
                numberActionsText.text = "2";
                break;
            case 12:
                trickPanel.SetActive(false);
                break;
            case 13:
                activateNextContent();
                numberActionsText.text = "1";
                break;
            case 14:
                trickPanel.SetActive(false);
                break;
            case 15:
                activateNextContent();
                numberActionsText.text = "0";
                break;
            case 16:
                trickPanel.SetActive(false);
                activateNextNeededObject();
                numberActionsText.text = "3";
                numberRoundText.text = "2";
                numberAltarText.text = "6";
                break;
            case 17:
                activateNextNeededObject();
                break;
            case 18:
                activateNextContent();
                numberActionsText.text = "2";
                break;
            case 20:
                trickPanel.SetActive(false);
                break;
            case 21:
                numberActionsText.text = "3";
                numberRoundText.text = "4";
                numberAltarText.text = "26";
                break;
            case 23:
                activateNextContent();
                numberActionsText.text = "2";
                break;
            case 24:
                trickPanel.SetActive(false);
                activateNextNeededObject();
                numberActionsText.text = "3";
                numberRoundText.text = "5";
                numberAltarText.text = "40";
                break;
            case 25:
                victoryPanel.SetActive(true);
                break;
        }
        
        tutorialTexts[textIndex].SetActive(true);
    }

    private void activateNextNeededObject()
    {
         objectsNeeded[objectIndex].SetActive(false);
         objectIndex++;
         objectsNeeded[objectIndex].SetActive(true);
    }
    
    private void activateNextContent()
    {
        trickPanel.SetActive(true);
        trickContent[contentIndex].SetActive(false);
        contentIndex++;
        trickContent[contentIndex].SetActive(true);
    }
    
    //Back to main menu
    public void BackToMenu()
    {
        SceneManager.LoadScene(sceneMainMenu);
    }
}
