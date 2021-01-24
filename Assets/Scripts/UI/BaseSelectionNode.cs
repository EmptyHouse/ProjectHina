using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSelectionNode : MonoBehaviour
{
    #region enums
    public enum SelectionDirection
    {
        NORTH,
        SOUTH,
        EAST,
        WEST,
    }
    #endregion enums

    private bool IsSelectionNodeActive = false;

    #region monobehaviour methods
    private void Update()
    {
        if (IsSelectionNodeActive)
        {
            UpdateSelectedNode();
        }
    }
    #endregion monobehavoiur methods
    /// <summary>
    /// Any logic that needs to occur while our node is selected should be handled here
    /// </summary>
    protected abstract void UpdateSelectedNode();


    public virtual void NodeWasSelected()
    {
        IsSelectionNodeActive = true;
    }

    public virtual void NodeWasDeselected()
    {
        IsSelectionNodeActive = false;
    }
}
