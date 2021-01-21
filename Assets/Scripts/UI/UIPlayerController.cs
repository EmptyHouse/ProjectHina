using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIPlayerController
{
    public static float JOYSTICK_DEADZONE_THRESHOLD = .65f;

    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";

    #region input methods
    public static bool GetSubmitButtonDown()
    {
        foreach (KeyCode SubmitKeycode in GetAllSubmitKeyCodes()) { if (Input.GetKeyDown(SubmitKeycode)) return true; }
        return false;
    }

    public static bool GetSubmitButtonHeld()
    {
        foreach (KeyCode SubmitKeycode in GetAllSubmitKeyCodes()) { if (Input.GetKeyUp(SubmitKeycode)) return true; }
        return false;
    }

    public static bool GetSubmitButtonUp()
    {
        foreach (KeyCode SubmitKeycode in GetAllSubmitKeyCodes()) { if (Input.GetKey(SubmitKeycode)) return true; }
        return false;
    }

    public static bool GetCancelButtonDown()
    {
        foreach (KeyCode CancelKeyCode in GetAllCancelKeyCodes()) { if (Input.GetKeyDown(CancelKeyCode)) return true; }
        return false;
    }

    public static bool GetCancelButtonUp()
    {
        foreach (KeyCode CancelKeyCode in GetAllCancelKeyCodes()) { if (Input.GetKeyUp(CancelKeyCode)) return true; }
        return false;
    }

    public static bool GetCancelButtonHeld()
    {
        foreach (KeyCode CancelKeyCode in GetAllCancelKeyCodes()) { if (Input.GetKey(CancelKeyCode)) return true; }
        return false;
    }

    public static float GetHorizontalInput()
    {
        return Input.GetAxisRaw(HORIZONTAL_AXIS);
    }

    public static float GetVerticalInput()
    {
        return Input.GetAxisRaw(VERTICAL_AXIS);
    }
    #endregion input methods

    #region get keycode methods
    private enum ButtonID
    {
        SUBMIT,
        CANCEL,
    }
    private static Dictionary<ButtonID, KeyCode[]> KeyboardInputs = new Dictionary<ButtonID, KeyCode[]>()
    {
        { ButtonID.SUBMIT, new KeyCode[] {KeyCode.Return, KeyCode.Space } },
        {ButtonID.CANCEL, new KeyCode[] {KeyCode.Escape, KeyCode.Backspace } },
    };

    /// <summary>
    /// Returns the appropriate submit keycodes based on the peripheral that is being used
    /// </summary>
    /// <returns></returns>
    private static KeyCode[] GetAllSubmitKeyCodes()
    {
        return KeyboardInputs[ButtonID.SUBMIT];
    }

    private static KeyCode[] GetAllCancelKeyCodes()
    {
        return KeyboardInputs[ButtonID.CANCEL];
    }
    #endregion get keycode methods
}
