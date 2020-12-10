using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The base class for all controller that can be used by players
/// </summary>
public abstract class EHBaseController : MonoBehaviour
{
    [SerializeField]
    private ButtonInputNode[] ButtonInputNodes = new ButtonInputNode[1];
    [SerializeField]
    private AxisInputNode[] AxisInputNodes = new AxisInputNode[1];

    /// <summary>
    /// Dictionary reference to all of our Button actions with the string name of the action being the key
    /// </summary>
    private Dictionary<string, ButtonInputNode> ButtonInputNodeDictionary = new Dictionary<string, ButtonInputNode>();

    /// <summary>
    /// Dictionary of all axis inputs with the string name of the axis being the key
    /// </summary>
    private Dictionary<string, AxisInputNode> AxisInputDictionary = new Dictionary<string, AxisInputNode>();
    // Indicates the index of the player controlling this controller. In the case of using joysticks we can offset using this value
    public byte PlayerIndex { get; private set; }

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

    /// <summary>
    /// This method will bind a button event to a function to be invoked whenever a button is pressed or released
    /// </summary>
    /// <param name="InputName"></param>
    /// <param name="bButtonPressedInput"></param>
    /// <param name="ButtonAction"></param>
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

    /// <summary>
    /// Unbinds a function from a button event
    /// </summary>
    /// <param name="InputName"></param>
    /// <param name="bButtonPressedInput"></param>
    /// <param name="ButtonAction"></param>
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

    /// <summary>
    /// Binds event to an axis value. This function is called every frame regardless of whether the values has changed or not
    /// </summary>
    /// <param name="AxisName"></param>
    /// <param name="AxisAction"></param>
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

    /// <summary>
    /// Unbinds a function to a button event
    /// </summary>
    /// <param name="AxisName"></param>
    /// <param name="AxisAction"></param>
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

    /// <summary>
    /// This will be called upon awake. Please set up any binding in this method
    /// </summary>
    public abstract void SetUpInput();

    /// <summary>
    /// Node that contains all the information needed for our button input event
    /// </summary>
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

    /// <summary>
    /// Node that contains all the information neede for our axis input event
    /// </summary>
    [System.Serializable]
    private class AxisInputNode
    {
        public string AxisName = "";

        public UnityAction<float> AxisAction;
    }
}
