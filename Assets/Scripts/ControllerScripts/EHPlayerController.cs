using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHPlayerController : EHBaseController
{
    #region const variables
    private const string JUMP_COMMAND = "Jump";
    private const string HORIZONTAL_AXIS = "Horizontal";
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

        BindActionToInput(JUMP_COMMAND, true, MovementComponent.BeginJump);
        BindActionToInput(JUMP_COMMAND, false, MovementComponent.EndJump);

        BindActionToAxis(HORIZONTAL_AXIS, MovementComponent.SetHorizontalInput);
    }
    #endregion override methods
}
