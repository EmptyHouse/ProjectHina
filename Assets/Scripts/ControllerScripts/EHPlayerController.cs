using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHPlayerController : EHBasePlayerController
{
    #region const variables
    public const string JUMP_COMMAND = "Jump";
    public const string DASH_COMMAND = "Dash";
    public const string ATTACK_COMMAND = "Attack";
    public const string SHOOT_COMMAND = "Shoot";

    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";
    #endregion const variables

    private EHAttackComponent AttackComponent;
    private EHMovementComponent MovementComponent;
    private DashComponent DashComponent;
    private EHPlayerCharacter PlayerCharacter;
    private WallJump CharacterWallJump;

    #region monobehaivour methods
    protected override void Awake()
    {
        base.Awake();
        BaseGameOverseer.Instance.PlayerController = this;
        PlayerCharacter = GetComponent<EHPlayerCharacter>();
    }

    
    #endregion monobehaviour methods

    #region override methods
    public override void SetUpInput()
    {
        DashComponent = GetComponent<DashComponent>();
        AttackComponent = GetComponent<EHAttackComponent>();
        MovementComponent = GetComponent<EHMovementComponent>();
        CharacterWallJump = GetComponent<WallJump>();

        BindActionToInput(JUMP_COMMAND, ButtonInputType.Button_Pressed, Jump);
        BindActionToInput(JUMP_COMMAND, ButtonInputType.Button_Buffer, JumpBufferEnded);
        BindActionToInput(JUMP_COMMAND, ButtonInputType.Button_Released, MovementComponent.EndJump);
        BindActionToInput(DASH_COMMAND, ButtonInputType.Button_Pressed, DashComponent.InputDash);
        BindActionToInput(DASH_COMMAND, ButtonInputType.Button_Buffer, DashComponent.CancelDash);
        BindActionToInput(SHOOT_COMMAND, ButtonInputType.Button_Pressed, Shoot);
        BindActionToInput(SHOOT_COMMAND, ButtonInputType.Button_Buffer, ShootBufferEnd);
        BindActionToInput(ATTACK_COMMAND, ButtonInputType.Button_Pressed, Attack);
        BindActionToInput(ATTACK_COMMAND, ButtonInputType.Button_Buffer, AttackBufferEnded);
        BindActionToInput(JUMP_COMMAND, ButtonInputType.Button_Pressed, CharacterWallJump.InputWallJump);
        BindActionToInput(JUMP_COMMAND, ButtonInputType.Button_Buffer, CharacterWallJump.InputCancelWallJump);

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

    private void Shoot()
    {
        AttackComponent.AttemptAttack(1);
    }

    private void ShootBufferEnd()
    {
        AttackComponent.ReleaseAttack(1);
    }
    private void Jump()
    {
        MovementComponent.InputJump();
    }

    private void JumpBufferEnded()
    {
        MovementComponent.ReleaseInputJump();
    }
    #endregion input helper methods

    public EHPlayerCharacter GetPlayerCharacter() { return PlayerCharacter; }
}
