using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHGameHUD : MonoBehaviour
{
    [SerializeField]
    private ScreenTransitionUI ScreenTransition = null;

    #region monobehaviour methods
    private void Start()
    {
        BaseGameOverseer.Instance.GameHUD = this;
    }
    #endregion monobehaviour methods

    #region get methods
    public ScreenTransitionUI GetScreenTransition() { return ScreenTransition; }
    #endregion get methods
}
