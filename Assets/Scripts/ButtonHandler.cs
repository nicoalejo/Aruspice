using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Direction Settings")]
    [Tooltip("Direction of movement: 1 for right, -1 for left")]
    public float direction = 1f;

    [Header("References")]
    [Tooltip("The inner object that will rotate and move")]
    public Transform innerObject;
    
    [Tooltip("Character image that will fade on hover")]
    public Image characterImage;

    [Header("Animation Settings")]
    [Tooltip("How far the inner object moves on hover")]
    public float moveDistance = 50f;
    
    [Tooltip("Rotation amount on hover (in degrees)")]
    public float rotationAmount = 15f;
    
    [Tooltip("Speed of the smooth transition")]
    public float animationSpeed = 5f;

    [Header("Opacity Settings")]
    [Tooltip("Character image opacity when hovered (0-1)")]
    [Range(0f, 1f)]
    public float hoveredOpacity = 0.3f;
    
    [Tooltip("Character image opacity when not hovered (0-1)")]
    [Range(0f, 1f)]
    public float normalOpacity = 1f;

    [Header("Color Settings")]
    [Tooltip("Tint color applied to character image when hovered")]
    public Color hoveredTintColor = Color.white;

    private bool isHovered = false;
    private Vector2 innerObjectOriginalPosition;
    private Quaternion innerObjectOriginalRotation;
    private Color originalCharacterColor;
    private RectTransform innerRectTransform;

    void Start()
    {
        if (innerObject != null)
        {
            innerRectTransform = innerObject.GetComponent<RectTransform>();
            
            if (innerRectTransform != null)
            {
                innerObjectOriginalPosition = innerRectTransform.anchoredPosition;
            }
            
            innerObjectOriginalRotation = innerObject.localRotation;
        }

        if (characterImage != null)
        {
            originalCharacterColor = characterImage.color;
            Color tempColor = originalCharacterColor;
            tempColor.a = normalOpacity;
            characterImage.color = tempColor;
        }
    }

    void Update()
    {
        AnimateInnerObject();
        AnimateCharacterOpacity();
    }

    void AnimateInnerObject()
    {
        if (innerObject == null) return;

        Quaternion targetRotation = innerObjectOriginalRotation;

        if (innerRectTransform != null)
        {
            Vector2 targetPosition = innerObjectOriginalPosition;

            if (isHovered)
            {
                targetPosition += new Vector2(moveDistance * direction, 0);
                targetRotation = innerObjectOriginalRotation * Quaternion.Euler(0, 0, -rotationAmount * direction);
            }

            innerRectTransform.anchoredPosition = Vector2.Lerp(innerRectTransform.anchoredPosition, targetPosition, Time.deltaTime * animationSpeed);
            innerObject.localRotation = Quaternion.Lerp(innerObject.localRotation, targetRotation, Time.deltaTime * animationSpeed);
        }
    }

    void AnimateCharacterOpacity()
    {
        if (characterImage == null) return;

        float targetAlpha = isHovered ? hoveredOpacity : normalOpacity;
        Color targetColor = isHovered ? hoveredTintColor : originalCharacterColor;
        
        Color currentColor = characterImage.color;
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * animationSpeed);
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * animationSpeed);
        
        characterImage.color = currentColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}
