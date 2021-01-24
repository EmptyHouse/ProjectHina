using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHPlayerController : EHBaseController
{
    #region const variables
    public const string JUMP_COMMAND = "Jump";
    public const string DASH_COMMAND = "Dash";
    public const string ATTACK_COMMAND = "Attack";

    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";
    #endregion const variables

    private EHAttackComponent AttackComponent;

    #region monobehaivour methods
    protected override void Awake()
    {
        base.Awake();
        AttackComponent = GetComponent<EHAttackComponent>();
    }

    
    #endregion monobehaviour methods

    #region override methods
    public override void SetUpInput()
    {
        EHMovementComponent MovementComponent = GetComponent<EHMovementComponent>();
        DashComponent DashComponent = GetComponent<DashComponent>();
        EHAttackComponent AttackComponent = GetComponent<EHAttackComponent>();

        BindActionToInput(JUMP_COMMAND, ButtonInputType.Button_Pressed, MovementComponent.AttemptJump);
        BindActionToInput(JUMP_COMMAND, ButtonInputType.Button_Released, MovementComponent.EndJump);
        BindActionToInput(DASH_COMMAND, ButtonInputType.Button_Pressed, DashComponent.AttemptDash);
        BindActionToInput(ATTACK_COMMAND, ButtonInputType.Button_Pressed, Attack);
        BindActionToInput(ATTACK_COMMAND, ButtonInputType.Button_Buffer, AttackBufferEnded);

        BindActionToAxis(HORIZONTAL_AXIS, MovementComponent.SetHorizontalInput);
        BindActionToAxis(VERTICAL_AXIS, MovementComponent.SetVerticalInput);
    }
    #endregion override methods

    #region input helper methods
    private void Attack()
    {
        AttackComponent.AttemptAttack(0);
    }

    private void AttackBufferEnded()
    {
        AttackComponent.ReleaseAttack(0);
    }
    #endregion input helper methods
}
