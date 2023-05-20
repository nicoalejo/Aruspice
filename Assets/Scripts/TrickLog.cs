using UnityEngine;
using UnityEngine.EventSystems;

public class TrickLog : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UIManager uiManager;
    
    private string titleInfo = "Truco\n";
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
        uiManager.FillLogUI(titleInfo, descriptionInfo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiManager.ClearLogUI();
    }
}
