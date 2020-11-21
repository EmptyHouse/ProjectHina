using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles logic for our selectable button UI element
/// </summary>
public class ButtonSelectionNode : BaseSelectionNode
{
    public UnityAction OnButtonPressedAction;
    public UnityAction OnButtonClickedAction;
    public UnityAction OnButtonReleasedAction;

    private bool ButtonIsHeld = false;

    #region override methods
    protected override void UpdateSelectedNode()
    {
        if (Input.GetKeyDown(SUBMIT_BUTTON))
        {
            OnButtonPressedAction?.Invoke();
            ButtonIsHeld = true;
        }
        if (Input.GetKeyUp(SUBMIT_BUTTON))
        {
            OnButtonReleasedAction?.Invoke();
            if (ButtonIsHeld)
            {
                OnButtonClickedAction?.Invoke();
            }
            ButtonIsHeld = false;
        }
    }

    public override void NodeWasSelected()
    {
        base.NodeWasSelected();
    }

    public override void NodeWasDeselected()
    {
        base.NodeWasDeselected();
        ButtonIsHeld = false;
    }
    #endregion override methods


}
