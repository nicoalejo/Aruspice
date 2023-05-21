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
    private void Start()
    {
        StartCoroutine(Intro());
    }

    //Shows the intro screen and then load the first level
    IEnumerator Intro()
    {
        if (shouldShow)
        {
            yield return TypeDialog(introText);
            yield return new WaitForSeconds(4f);
         
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
        StopCoroutine(Intro());
        introPanelUI.SetActive(false);
    }
    
}