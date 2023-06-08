using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using VoltstroStudios.UnityWebBrowser.Input;
using VoltstroStudios.UnityWebBrowser.Shared;

/// <summary>
///     Input handler using Unity's new input system
/// </summary>
[CreateAssetMenu(fileName = "UMI3D Input System Handler", menuName = "UWB/Inputs/UMI3D Input System Handler")]
public class UMI3DWebBrowserInputSystemHandler : WebBrowserInputHandler
{
    [Header("Scroll Input")] public InputAction scrollInput;

    public float scrollValue = 0.2f;

    [Header("Pointer Position")] public InputAction pointPosition;

    private readonly List<WindowsKey> keysDown = new();
    private readonly List<WindowsKey> keysUp = new();
    private string inputBuffer = string.Empty;

    private Keyboard keyboard;
    private IMECompositionMode compositionMode;

    public override float GetScroll()
    {
        //Mouse scroll wheel in the new input system is fucked, its value is either 120 or -120,
        //no in-between or -1.0 to 1.0 like the old input system. Why Unity.
        //While there are forum post talking about this, nothing is from Unity themselves about the issue.
        float scroll = scrollInput.ReadValue<Vector2>().y;
        scroll = Mathf.Clamp(scroll, -scrollValue, scrollValue);
        Debug.Log(scroll);
        return scroll;
    }

    public override Vector2 GetCursorPos()
    {
        return pointPosition.ReadValue<Vector2>();
    }

    public override WindowsKey[] GetDownKeys()
    {
        keysDown.Clear();
        foreach (KeyControl key in keyboard.allKeys)
            try
            {
                if (key.wasPressedThisFrame)
                    keysDown.Add(key.keyCode.UnityKeyToWindowKey());
            }
            catch (ArgumentOutOfRangeException)
            {
                //Safe to ignore
            }

        return keysDown.ToArray();
    }

    public override WindowsKey[] GetUpKeys()
    {
        keysUp.Clear();
        foreach (KeyControl key in keyboard.allKeys)
            try
            {
                if (key.wasReleasedThisFrame)
                    keysUp.Add(key.keyCode.UnityKeyToWindowKey());
            }
            catch (ArgumentOutOfRangeException)
            {
                //Safe to ignore
            }

        return keysUp.ToArray();
    }

    public override string GetFrameInputBuffer()
    {
        string buffer = inputBuffer;
        inputBuffer = string.Empty;
        return buffer;
    }

    public override void OnStart()
    {
        keyboard = Keyboard.current;

        //Don't bother initializing some stuff if we can't get a keyboard
        if (keyboard != null)
        {
            keyboard.onTextInput += OnTextEnter;
            inputBuffer = string.Empty;
        }

        Debug.Log("Enable");

        scrollInput.Enable();
        pointPosition.Enable();

        keysDown.Clear();
        keysUp.Clear();
    }

    private void OnTextEnter(char character)
    {
        inputBuffer += character;
    }

    public override void OnStop()
    {
        //Keyboard might actually already be destroyed by now
        if (keyboard != null)
        {
            keyboard.onTextInput -= OnTextEnter;
            keyboard = null;
        }

        Debug.Log("Disable");

        scrollInput.Disable();
        pointPosition.Disable();
    }

    public override void EnableIme(Vector2 location)
    {
        //Appears we still have to set UnityEngine.Input.imeCompositionMode?
        compositionMode = UnityEngine.Input.imeCompositionMode;
        UnityEngine.Input.imeCompositionMode = IMECompositionMode.On;

        keyboard.SetIMEEnabled(true);
        keyboard.SetIMECursorPosition(location);
    }

    public override void DisableIme()
    {
        UnityEngine.Input.imeCompositionMode = compositionMode;
        switch (compositionMode)
        {
            case IMECompositionMode.Auto:
            case IMECompositionMode.On:
                keyboard.SetIMEEnabled(true);
                break;
            case IMECompositionMode.Off:
                keyboard.SetIMEEnabled(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
