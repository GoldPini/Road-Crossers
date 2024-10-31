using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    private GameInput playerInputActions;

    public enum KeyOptions
    {
        MoveForward, MoveBackward, MoveLeft, MoveRight, Run, Pause
    }

    private void Awake()
    {
        Instance = this;
        playerInputActions = new GameInput();
        playerInputActions.CharacterControl.Enable();
        playerInputActions.CharacterControl.Pause.performed += Pause_performed;

        Debug.Log(GetBindingText(KeyOptions.MoveForward));
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        playerInputActions.CharacterControl.Pause.performed -= Pause_performed;
        playerInputActions.Dispose();
    }

    public string GetBindingText(KeyOptions option)
    {
        switch (option)
        {
            case KeyOptions.MoveForward:
                return playerInputActions.CharacterControl.Move.bindings[0].ToString(); 
                break;
        }
        return "";
    }
}
