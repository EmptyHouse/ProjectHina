using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Handles logic for our selectable button UI element
/// </summary>
public class ButtonSelectionNode : BaseSelectionNode
{
    public enum EButtonActionType
    {
        CLICK,
        PRESS,
        RELEASE,
    }
    [Header("UI Elements")]
    [SerializeField]
    private Image ButtonBGImage;
    [SerializeField]
    private Text ButtonText;

    [Header("Button Events")]
    [Tooltip("Action that will be executed when our button is pressed down and then released")]
    public UnityEvent OnButtonClickedAction;
    [Tooltip("Action that will be executed when our button is pressed down")]
    public UnityEvent OnButtonPressedAction;
    [Tooltip("Action that will be executed when our button is released")]
    public UnityEvent OnButtonReleasedAction;

    [Header("Button Properties")]
    public Color SelectedColor;
    private Color DefaultColor;
    private bool ButtonIsHeld = false;
    #region monobehaviour methods
    protected virtual void Awake()
    {
        if (ButtonBGImage)
        {
            DefaultColor = ButtonBGImage.color;
        }
    }
    #endregion monobehaviour methods

    #region override methods
    protected override void UpdateSelectedNode()
    {
    }

    public void BindActionToButton(EButtonActionType ActionType, UnityAction ButtonAction)
    {
        switch (ActionType)
        {
            case EButtonActionType.CLICK:
                OnButtonClickedAction.AddListener(ButtonAction);
                return;
            case EButtonActionType.PRESS:
                OnButtonPressedAction.AddListener(ButtonAction);
                return;
            case EButtonActionType.RELEASE:
                OnButtonReleasedAction.AddListener(ButtonAction);
                return;
        }
        Debug.LogWarning("Invalid Action Type...");
    }

    public void UnbindActionFromButton(EButtonActionType ActionType, UnityAction ButtonAction)
    {
        switch (ActionType)
        {
            case EButtonActionType.CLICK:
                OnButtonClickedAction.RemoveListener(ButtonAction);
                return;
            case EButtonActionType.PRESS:
                OnButtonPressedAction.RemoveListener(ButtonAction);
                return;
            case EButtonActionType.RELEASE:
                OnButtonReleasedAction.RemoveListener(ButtonAction);
                return;
        }
    }

    public override void NodeWasSelected()
    {
        base.NodeWasSelected();
        ButtonIsHeld = false;
        ButtonBGImage.color = SelectedColor;
        UIPlayerController UIController = EHHUD.Instance.UIController;
        if (UIController != null)
        {
            UIController.BindActionToInput(UIPlayerController.SUBMIT, EHBasePlayerController.ButtonInputType.Button_Pressed, OnSubmitPressed);
            UIController.BindActionToInput(UIPlayerController.SUBMIT, EHBasePlayerController.ButtonInputType.Button_Released, OnSubmitReleased);
        }
    }

    public override void NodeWasDeselected()
    {
        base.NodeWasDeselected();
        ButtonIsHeld = false;
        ButtonBGImage.color = DefaultColor;
        UIPlayerController UIController = EHHUD.Instance.UIController;
        if (UIController)
        {
            UIController.UnbindActionToInput(UIPlayerController.SUBMIT, EHBasePlayerController.ButtonInputType.Button_Pressed, OnSubmitPressed);
            UIController.UnbindActionToInput(UIPlayerController.SUBMIT, EHBasePlayerController.ButtonInputType.Button_Released, OnSubmitReleased);
        }
    }
    #endregion override methods

    /// <summary>
    /// 
    /// </summary>
    private void OnSubmitPressed()
    {
        OnButtonPressedAction?.Invoke();
        ButtonIsHeld = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnSubmitReleased()
    {
        OnButtonReleasedAction?.Invoke();
        if (ButtonIsHeld)
        {
            OnButtonClickedAction?.Invoke();
        }
        ButtonIsHeld = false;
    }
}
