using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHSplashHUD : EHHUD
{
    [SerializeField]
    private BaseSelectionUI DefaultUIToOpen;

    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual void Start()
    {
        DefaultUIToOpen.OpenSelectionWindow();
    }
}
