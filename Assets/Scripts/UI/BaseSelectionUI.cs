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
    #region monobehaviour methods
    protected virtual void Awake()
    {
        SelectionNodeList = GetComponentsInChildren<BaseSelectionNode>();
        if (SelectionNodeList.Length > 0)
        {
            CurrentSelectionIndex = 0;
            SelectionNodeList[0].NodeWasSelected();
        }
    }

    private void Update()
    {
        if (!bIsAutoScrolling && Mathf.Abs(UIPlayerController.GetVerticalInput()) > JOYSTICK_THRESHOLD)
        {
            StartCoroutine(ScrollCoroutine(UIPlayerController.GetVerticalInput()));
        }
    }
    #endregion monobehaviour methods

    public virtual void OpenSelectionWindow(bool ResetToDefaultSelection = false)
    {
        this.gameObject.SetActive(true);
        if (ResetToDefaultSelection)
        {
            if (SelectionNodeList.Length > 0)
            {
                SelectSelectionNode(0);
            }
        }
    }

    public virtual void CloseSelectionWindow()
    {
        this.gameObject.SetActive(false);
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
            if (Sign * UIPlayerController.GetVerticalInput() < JOYSTICK_THRESHOLD)
            {
                bIsAutoScrolling = false;
                yield break;
            }
        }

        while (Sign * UIPlayerController.GetVerticalInput() > JOYSTICK_THRESHOLD)
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
}
