using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextHandler : MonoBehaviour
{
    [SerializeField] private bool shouldShow = true;
    [SerializeField] private GameObject introPanelUI;
    [SerializeField] private TextMeshProUGUI introTextUI;
    [TextArea]
    [SerializeField] private string introText;
    [SerializeField] private int letterPerSecond;
    [SerializeField] private Image[] imagesToShow;

    private bool isTextComplete = false;
    private Coroutine introCoroutine;
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
    IEnumerator Intro()
    {
        if (shouldShow)
        {
            introCoroutine = StartCoroutine(TypeDialog(introText));
            yield return introCoroutine;
            yield return new WaitForSeconds(4f);
            //introPanelUI.SetActive(false);
        }
        else
        {
            introPanelUI.SetActive(false);
        }
    }

    //Shows text smoothly
    public IEnumerator TypeDialog(string line)
    {
        introTextUI.text = "";
        foreach (var letter in line.ToCharArray())
        {
            introTextUI.text += letter;
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
            StopCoroutine(introCoroutine);
            introTextUI.text = introText;
            isTextComplete = true;
        }
        else
        {
            introPanelUI.SetActive(false);  
        }

    }
    
}