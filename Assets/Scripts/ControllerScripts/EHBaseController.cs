using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EHBaseController : MonoBehaviour
{
    [SerializeField]
    private ButtonInputNode[] ButtonInputNodes = new ButtonInputNode[1];
    [SerializeField]
    private AxisInputNode[] AxisInputNodes = new AxisInputNode[1];

    private Dictionary<string, ButtonInputNode> ButtonInputNodeDictionary = new Dictionary<string, ButtonInputNode>();
    private Dictionary<string, AxisInputNode> AxisInputDictionary = new Dictionary<string, AxisInputNode>();

    #region monobehaviour methods
    protected virtual void Awake()
    {
        foreach (ButtonInputNode ButtonNode in ButtonInputNodes)
        {
            ButtonInputNodeDictionary.Add(ButtonNode.InputName, ButtonNode);
        }

        foreach (AxisInputNode AxisNode in AxisInputNodes)
        {
            AxisInputDictionary.Add(AxisNode.AxisName, AxisNode);
        }
        SetUpInput();
    }

    protected virtual void OnDestroy()
    {
        foreach (ButtonInputNode ButtonNode in ButtonInputNodes)
        {
            ButtonNode.OnButtonPressed = null;
            ButtonNode.OnButtonReleased = null;
        }

        foreach (AxisInputNode AxisNode in AxisInputNodes)
        {
            AxisNode.AxisAction = null;
        }
    }

    protected virtual void Update()
    {
        foreach (ButtonInputNode ButtonNode in ButtonInputNodes)
        {
            if (ButtonNode.ButtonDown)
            {
                ButtonNode.OnButtonPressed?.Invoke();
            }
            if (ButtonNode.ButtonReleased)
            {
                ButtonNode?.OnButtonReleased?.Invoke();
            }
        }

        foreach (AxisInputNode AxisNode in AxisInputNodes)
        {
            AxisNode.AxisAction?.Invoke(Input.GetAxisRaw(AxisNode.AxisName));
        }
    }
    #endregion monobehaviour methods

    public void BindActionToInput(string InputName, bool bButtonPressedInput, UnityAction ButtonAction)
    {
        if (InputName == null || !ButtonInputNodeDictionary.ContainsKey(InputName))
        {
            Debug.LogWarning("Invalid Button Name: " + InputName);
            return;
        }
        ButtonInputNode ButtonInput = ButtonInputNodeDictionary[InputName];
        if (bButtonPressedInput)
        {
            ButtonInput.OnButtonPressed += ButtonAction;
        }
        else
        {
            ButtonInput.OnButtonReleased += ButtonAction;
        }
    }

    public void UnbindActionToInput(string InputName, bool bButtonPressedInput, UnityAction ButtonAction)
    {
        if (InputName == null || !ButtonInputNodeDictionary.ContainsKey(InputName))
        {
            Debug.LogWarning("Invalid Button Name: " + InputName);
            return;
        }
        ButtonInputNode ButtonInput = ButtonInputNodeDictionary[InputName];
        if (bButtonPressedInput)
        {
            ButtonInput.OnButtonPressed -= ButtonAction;
        }
        else
        {
            ButtonInput.OnButtonReleased -= ButtonAction;
        }
    }

    public void BindActionToAxis(string AxisName, UnityAction<float> AxisAction)
    {
        if (AxisName == null || !AxisInputDictionary.ContainsKey(AxisName))
        {
            Debug.LogWarning("Invalid Axis Name: " + AxisName);
            return;
        }
        AxisInputNode AxisNode = AxisInputDictionary[AxisName];
        AxisNode.AxisAction += AxisAction;
    }

    public void UnbindActionToAxis(string AxisName, UnityAction<float> AxisAction)
    {
        if (AxisName == null || !AxisInputDictionary.ContainsKey(AxisName))
        {
            Debug.LogWarning("Invalid Axis Name: " + AxisName);
            return;
        }
        AxisInputNode AxisNode = AxisInputDictionary[AxisName];
        AxisNode.AxisAction -= AxisAction;
    }

    public abstract void SetUpInput();

    [System.Serializable]
    private class ButtonInputNode
    {
        public string InputName = "";

        public KeyCode MainKey = KeyCode.None;
        public KeyCode AlternateKey = KeyCode.None;

        public UnityAction OnButtonPressed;
        public UnityAction OnButtonReleased;

        public bool ButtonDown
        {
            get
            {
                return Input.GetKeyDown(MainKey) || Input.GetKeyDown(AlternateKey);
            }
        }

        public bool ButtonReleased
        {
            get
            {
                return Input.GetKeyUp(MainKey) || Input.GetKeyUp(AlternateKey);
            }
        }
    }

    [System.Serializable]
    private class AxisInputNode
    {
        public string AxisName = "";

        public UnityAction<float> AxisAction;
    }
}
