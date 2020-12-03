using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHPlayerController : EHBaseController
{
    #region const variables
    private const string JUMP_COMMAND = "Jump";
    private const string DASH_COMMAND = "Dash";

    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";
    #endregion const variables


    #region monobehaivour methods
    protected override void Awake()
    {
        base.Awake();
    }

    
    #endregion monobehaviour methods

    #region override methods
    public override void SetUpInput()
    {
        EHMovementComponent MovementComponent = GetComponent<EHMovementComponent>();

        BindActionToInput(JUMP_COMMAND, true, MovementComponent.AttemptJump);
        BindActionToInput(JUMP_COMMAND, false, MovementComponent.EndJump);
        BindActionToInput(DASH_COMMAND, true, MovementComponent.AttemptDash);

        BindActionToAxis(HORIZONTAL_AXIS, MovementComponent.SetHorizontalInput);
        BindActionToAxis(VERTICAL_AXIS, MovementComponent.SetVerticalInput);
    }
    #endregion override methods
}
