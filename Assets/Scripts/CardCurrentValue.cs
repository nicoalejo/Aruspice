using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardCurrentValue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentValueTextUI;

    public void UpdateCurrentValue(int newValue)
    {
        currentValueTextUI.text = newValue.ToString();
    }
}
