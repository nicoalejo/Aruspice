using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TiltCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Rotation Parameters")]
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float rotationAmplitude = 5f;
    [SerializeField] private float mouseTiltIntensity = 10f;
    [SerializeField] private float tiltSpeed = 20f;
    
    [Header("3D Movement Parameters")]
    [SerializeField] private float hoverZOffset = 20f;
    [SerializeField] private float followSpeed = 30f;
    
    private float savedIndex;
    private Quaternion initialRotation;
    private bool isHovering = false;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 initialPosition;
    private Vector3 targetRotation;

    void Start()
    {
        savedIndex = Random.Range(0f, 100f);
        initialRotation = transform.rotation;
        initialPosition = transform.localPosition;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        HandleRotation();
        Handle3DMovement();
    }

    private void HandleRotation()
    {
        Vector3 rotation;
        
        if (isHovering && rectTransform != null)
        {
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, 
                Input.mousePosition, 
                canvas?.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas?.worldCamera, 
                out localMousePosition
            );
            
            // Normalize the offset based on card size
            Vector2 normalizedOffset = new Vector2(
                localMousePosition.x / (rectTransform.rect.width * 0.5f),
                localMousePosition.y / (rectTransform.rect.height * 0.5f)
            );
            
            // Clamp to prevent extreme values
            normalizedOffset.x = Mathf.Clamp(normalizedOffset.x, -1f, 1f);
            normalizedOffset.y = Mathf.Clamp(normalizedOffset.y, -1f, 1f);
            
            // Apply tilt (card tilts away from mouse)
            float tiltX = normalizedOffset.y * -1f;
            float tiltY = normalizedOffset.x;
            
            // Reduced auto-tilt when hovering (like in reference)
            float sine = Mathf.Sin(Time.time * rotationSpeed + savedIndex) * 0.2f;
            float cosine = Mathf.Cos(Time.time * rotationSpeed + savedIndex) * 0.2f;
            
            rotation = new Vector3(
                tiltX * mouseTiltIntensity + (sine * rotationAmplitude),
                0f,
                tiltY * mouseTiltIntensity + (cosine * rotationAmplitude)
            );
        }
        else
        {
            float sine = Mathf.Sin(Time.time * rotationSpeed + savedIndex);
            float cosine = Mathf.Cos(Time.time * rotationSpeed + savedIndex);
            
            rotation = new Vector3(
                sine * rotationAmplitude,
                0f,
                cosine * rotationAmplitude
            );
        }
        
        targetRotation = rotation;
        
        // Smooth rotation interpolation
        float lerpX = Mathf.LerpAngle(transform.eulerAngles.x, initialRotation.eulerAngles.x + targetRotation.x, tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(transform.eulerAngles.y, initialRotation.eulerAngles.y + targetRotation.y, tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(transform.eulerAngles.z, initialRotation.eulerAngles.z + targetRotation.z, tiltSpeed * Time.deltaTime);
        
        transform.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    private void Handle3DMovement()
    {
        Vector3 targetPosition = initialPosition;
        
        if (isHovering)
        {
            // Move card up on Y axis when hovering for 3D effect
            targetPosition += Vector3.up * hoverZOffset;
        }
        
        // Smooth position interpolation
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, followSpeed * Time.deltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
