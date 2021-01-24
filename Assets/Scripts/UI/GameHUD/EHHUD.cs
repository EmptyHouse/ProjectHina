using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIPlayerController))]
public class EHHUD : MonoBehaviour
{
    private static EHHUD instance;
    public static EHHUD Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<EHHUD>();
            }
            return instance;
        }
    }

    public UIPlayerController UIController { get; private set; }

    #region monobehaviour methods
    protected virtual void Awake()
    {
        instance = this;
        UIController = GetComponent<UIPlayerController>();
    }
    #endregion monobehaviour methods
}
