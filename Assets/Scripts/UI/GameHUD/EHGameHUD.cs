using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIPlayerController))]
public class EHGameHUD : EHHUD
{
    [SerializeField]
    private ScreenTransitionUI ScreenTransition = null;

    #region get methods
    public ScreenTransitionUI GetScreenTransition() { return ScreenTransition; }
    #endregion get methods
}
