using UnityEngine;
using UnityEngine.EventSystems;

public class CardLogCalculation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UIManager uiManager;
    
    private string titleInfo = "Carta en Altar\n";
    private string explanationInfo = "Explicación del valor actual\n";
    private string descriptionInfo = "";
    
    private CardSuit suit;
    private int initialValue;
    private int finalValue;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    public void SetInitialValueLog(CardSuit suit, int initialValue)
    {
        descriptionInfo = "Valor inicial: " + initialValue + "\n"
                            + "Tipo: " + suit + "\n"
                            + "Calculos: \n";
    }

    public void SetMultiplyLogInfo(CardSuit suitMultiplier, int newValue)
    {
        descriptionInfo += "Carta previa " + suitMultiplier + " (*2)\n"
                           + "Nuevo valor: " + newValue + "\n";
    }
    
    public void SetSubtractLogInfo(CardSuit suitSubtractor, int newValue, bool isPrevious)
    {
        string cardPreviousNext = isPrevious ? "previa" : "siguiente";
        descriptionInfo += "Carta " + cardPreviousNext + " tipo " + suitSubtractor + " (-1)\n"
                            + "Nuevo valor: " + newValue + "\n";
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
