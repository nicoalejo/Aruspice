using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TextHandler : MonoBehaviour
{
    public delegate void OnTextComplete(bool isTextIntro);
    public static OnTextComplete onTextComplete;

    [SerializeField] private TextMeshProUGUI textUI;
    [TextArea]
    [SerializeField] private string introText;
    [SerializeField] private int letterPerSecond;
    [SerializeField] private Image[] imagesToShow;

    private bool isTextComplete = false;
    private String currentText = "";
    private Coroutine textCoroutine;
    private bool isTextIntro = true;
    
    
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
        isTextIntro = true;
        currentText = introText;
        textCoroutine = StartCoroutine(TypeDialog(currentText));
        yield return textCoroutine;
        yield return new WaitForSeconds(4f);
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

    public IEnumerator ShowImages()
    {
        yield return null;
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
            onTextComplete?.Invoke(isTextIntro);
        }
    }
    
    public IEnumerator ShowTextInGO(string textToShow)
    {
        isTextIntro = false;
        currentText = textToShow;
        textCoroutine = StartCoroutine(TypeDialog(currentText));;
        yield return textCoroutine;
        yield return new WaitForSeconds(4f);
    }
    
}