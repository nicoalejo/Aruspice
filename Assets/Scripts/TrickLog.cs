using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrickLog : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UIManager uiManager;
    
    private string titleInfo = "Truco\n";
    private string explanationInfo = "Explicación del funcionamiento\n";
    [SerializeField] private int actionCost;
    [SerializeField] private string descriptionInfo = "";
     
    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();

        descriptionInfo = "Costo Acciones: " + actionCost + "\n"
                          + descriptionInfo;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        uiManager.FillLogUI(titleInfo, explanationInfo, descriptionInfo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiManager.ClearLogUI();
    }
}
