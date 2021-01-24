using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerController : EHBaseController
{
    public static float JOYSTICK_DEADZONE_THRESHOLD = .65f;

    public const string SUBMIT = "Submit";
    public const string CANCEL = "Cancel";

    public const string HORIZONTAL_AXIS = "Horizontal";
    public const string VERTICAL_AXIS = "Vertical";

    public override void SetUpInput()
    {
        
    }
}
