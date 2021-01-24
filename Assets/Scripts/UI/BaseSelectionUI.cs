using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSelectionUI : MonoBehaviour
{
    protected const float JOYSTICK_THRESHOLD = .65f;
    public float AutoScrollTime = .1f;
    private int CurrentSelectionIndex;
    private BaseSelectionNode[] SelectionNodeList;
    private bool bIsAutoScrolling;

    private Vector2 JoystickInput;
    private Vector2 PreviousJoystickInput;
    #region monobehaviour methods
    protected virtual void Awake()
    {
        SelectionNodeList = GetComponentsInChildren<BaseSelectionNode>();
        if (SelectionNodeList.Length > 0)
        {
            CurrentSelectionIndex = 0;
        }
    }

    private void Update()
    {
        if (!bIsAutoScrolling && Mathf.Abs(JoystickInput.y) > JOYSTICK_THRESHOLD)
        {
            StartCoroutine(ScrollCoroutine(JoystickInput.y));
        }
    }

    private void SetVerticalInput(float VerticalInput)
    {
        PreviousJoystickInput.y = JoystickInput.y;
        JoystickInput.y = VerticalInput;
    }

    private void SetHorizontalInput(float HorizontalInput)
    {
        PreviousJoystickInput.x = JoystickInput.x;
        JoystickInput.x = HorizontalInput;
    }
    #endregion monobehaviour methods

    public virtual void OpenSelectionWindow(bool ResetToDefaultSelection = false)
    {
        this.gameObject.SetActive(true);
        if (ResetToDefaultSelection)
        {
            CurrentSelectionIndex = 0;
        }
        BindUIInput();
        SelectionNodeList[CurrentSelectionIndex].NodeWasSelected();
    }

    public virtual void CloseSelectionWindow()
    {
        this.gameObject.SetActive(false);
        UnbindUIInput();
        SelectionNodeList[CurrentSelectionIndex].NodeWasDeselected();
    }

    protected virtual void BindUIInput()
    {
        UIPlayerController UIController = EHHUD.Instance.UIController;
        if (UIController)
        {
            UIController.BindActionToAxis(UIPlayerController.HORIZONTAL_AXIS, SetHorizontalInput);
            UIController.BindActionToAxis(UIPlayerController.VERTICAL_AXIS, SetVerticalInput);
        }
    }

    protected virtual void UnbindUIInput()
    {
        UIPlayerController UIController = EHHUD.Instance.UIController;
        if (UIController)
        {
            UIController.UnbindActionToAxis(UIPlayerController.HORIZONTAL_AXIS, SetHorizontalInput);
            UIController.UnbindActionToAxis(UIPlayerController.VERTICAL_AXIS, SetVerticalInput);
        }
    }

    protected void SelectSelectionNode(int NewSelectionNodeIndex)
    {
        if (CurrentSelectionIndex != NewSelectionNodeIndex)
        {
            if (SelectionNodeList[CurrentSelectionIndex])
            {
                SelectionNodeList[CurrentSelectionIndex].NodeWasDeselected();
            }
            SelectionNodeList[NewSelectionNodeIndex].NodeWasSelected();
            CurrentSelectionIndex = NewSelectionNodeIndex;
        }
    }

    private IEnumerator ScrollCoroutine(float InitialJoystickValue)
    {
        bIsAutoScrolling = true;
        float TimeThatHasPassed = .1f;
        float Sign = Mathf.Sign(InitialJoystickValue);
        SelectSelectionNode(Mathf.Clamp((CurrentSelectionIndex - (int)Sign), 0, SelectionNodeList.Length - 1));
        while (TimeThatHasPassed < AutoScrollTime * 3)
        {
            yield return null;
            TimeThatHasPassed += EHTime.RealDeltaTime;
            if (Sign * JoystickInput.y < JOYSTICK_THRESHOLD)
            {
                bIsAutoScrolling = false;
                yield break;
            }
        }

        while (Sign * JoystickInput.y > JOYSTICK_THRESHOLD)
        {
            yield return null;
            TimeThatHasPassed += EHTime.RealDeltaTime;
            if (TimeThatHasPassed > AutoScrollTime)
            {
                SelectSelectionNode(Mathf.Clamp((CurrentSelectionIndex - (int)Sign), 0, SelectionNodeList.Length - 1));
                TimeThatHasPassed -= AutoScrollTime;
            }
        }
        bIsAutoScrolling = false;
    }

    private bool IsSelectionIndexValid(int SelectionIndex)
    {
        if (SelectionIndex < 0 || SelectionIndex >= SelectionNodeList.Length)
        {
            return false;
        }
        return true;
    }
}
