using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSelectionUI : MonoBehaviour
{
    [Tooltip("The node that will first be selected upon opening up the screen")]
    public BaseSelectionNode InitialNodeToSelect;

    private BaseSelectionNode CurrentSelectedNode;

    #region monobehaviour methods
    private void Awake()
    {

    }

    private void Update()
    {

    }
    #endregion monobehaviour methods
}
