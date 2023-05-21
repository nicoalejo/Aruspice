using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsHandler : MonoBehaviour
{
    private Controles inputActions; // This is the class that was generated

    public static Action onEventLeftMouse;

    private void Awake()
    {
        inputActions = new Controles();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
        inputActions.Gameplay.LeftClick.performed += HandleLeftClick;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.LeftClick.performed -= HandleLeftClick;
        inputActions.Gameplay.Disable();
    }

    private void HandleLeftClick(InputAction.CallbackContext context)
    {
        onEventLeftMouse?.Invoke();
    }
}