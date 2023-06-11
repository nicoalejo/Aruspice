using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsHandler : MonoBehaviour
{
    private Controles inputActions; // This is the class that was generated

    public static Action onEventLeftMouse;
    public static Action onEventEscape;

    private void Awake()
    {
        inputActions = new Controles();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
        inputActions.Gameplay.LeftClick.performed += HandleLeftClick;
        inputActions.Gameplay.Esc.performed += HandleEscape;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.LeftClick.performed -= HandleLeftClick;
        inputActions.Gameplay.Esc.performed -= HandleEscape;
        inputActions.Gameplay.Disable();
    }

    private void HandleEscape(InputAction.CallbackContext context)
    {
        onEventEscape?.Invoke();
    }

    private void HandleLeftClick(InputAction.CallbackContext context)
    {
        onEventLeftMouse?.Invoke();
    }
}