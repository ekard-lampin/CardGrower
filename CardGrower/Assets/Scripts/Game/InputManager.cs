using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    void Awake() { instance = this; }

    public bool GetFPress() { return Keyboard.current.fKey.wasPressedThisFrame; }

    public Vector3 GetMovementInput()
    {
        Vector3 inputValue = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) { inputValue.x++; }
        if (Keyboard.current.sKey.isPressed) { inputValue.x--; }
        if (Application.isEditor)
        {
            if (Keyboard.current.cKey.isPressed) { inputValue.y--; }
        }
        else
        {
            if (Keyboard.current.leftAltKey.isPressed) { inputValue.y--; }
        }
        if (Keyboard.current.spaceKey.isPressed) { inputValue.y++; }
        if (Keyboard.current.aKey.isPressed) { inputValue.z--; }
        if (Keyboard.current.dKey.isPressed) { inputValue.z++; }

        return inputValue;
    }

    public bool GetMouseLeftClickPress()
    {
        return Mouse.current.leftButton.wasPressedThisFrame;
    }

    public Vector2 GetMouseMovementInput()
    {
        return Mouse.current.delta.ReadValue();
    }

    public bool GetTabPress() { return Keyboard.current.tabKey.wasPressedThisFrame; }
}
