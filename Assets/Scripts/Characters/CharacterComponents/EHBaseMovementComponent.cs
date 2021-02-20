using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EHPhysics2D))]
public abstract class EHBaseMovementComponent : MonoBehaviour
{
    #region const variables
    private const string ANIM_HORIZONTAL_INPUT = "HorizontalInput";
    private const string ANIM_VERTICAL_INPUT = "VerticalInput";
    #endregion const variables

    #region main variables
    protected EHPhysics2D Physics2D;
    protected Animator CharacterAnimator;
    protected SpriteRenderer CharacterSpriteRenderer;
    protected bool bIsFacingLeft;

    #endregion main variables

    #region animation controlled varaibles
    /// <summary>
    /// Mark this true if our movement component will be controlled by our animation controller
    /// </summary>
    [HideInInspector]
    public bool bIsAnimationControlled;

    /// <summary>
    /// The goal velocity of our movmeent component when bIsAnimationControlled is set
    /// to true
    /// </summary>
    [HideInInspector]
    public Vector2 AnimatedGoalVelocity;

    /// <summary>
    /// Set this value to true if you want to disable the movement component entirely. This will be good if we want another function
    /// to control the movement of our character
    /// </summary>
    [HideInInspector]
    public bool bMovementComponentDisabled;
    #endregion animation controlled variables

    #region input varaibles
    // The currently held directional Input that our movement will consider when setting the velocity
    private Vector2 CurrentInput;
    // The previously held direction
    private Vector3 PreviousInput;
    #endregion input varaibles 

    #region monobehaviour methods
    protected virtual void Awake()
    {
        CharacterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CharacterAnimator = GetComponent<Animator>();
        Physics2D = GetComponent<EHPhysics2D>();
    }

    protected virtual void Update()
    {
        if (!bMovementComponentDisabled)
        {
            UpdateVelocityFromInput(CurrentInput, PreviousInput);
        }
        UpdateMovementTypeFromInput(CurrentInput);
        if (CharacterAnimator)
        {
            CharacterAnimator.SetFloat(ANIM_HORIZONTAL_INPUT, Mathf.Abs(CurrentInput.x));
        }
    }
    #endregion monobehaviour methods

    #region update methods
    protected abstract void UpdateVelocityFromInput(Vector2 CurrentInput, Vector2 PreviousInput);
    protected abstract void UpdateMovementTypeFromInput(Vector2 CurrentInput);
    #endregion update methods

    #region input methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMovementInput()
    {
        return CurrentInput;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector2 GetPreviousMovementInput()
    {
        return PreviousInput;
    }
    /// <summary>
    /// Sets the current input
    /// </summary>
    /// <param name="HorizontalInput"></param>
    public void SetHorizontalInput(float HorizontalInput)
    {
        CurrentInput.x = Mathf.Clamp(HorizontalInput, -1f, 1f);
        if (HorizontalInput < 0 && !bIsFacingLeft)
        {
            SetIsFacingLeft(true);
        }
        else if(HorizontalInput > 0 && bIsFacingLeft)
        {
            SetIsFacingLeft(false);
        }
    }

    /// <summary>
    /// Set our current vertical Input. This should be a value between -1 and 1. If not we will clamp the values
    /// </summary>
    /// <param name="VerticalInput"></param>
    public void SetVerticalInput(float VerticalInput)
    {
        CurrentInput.y = Mathf.Clamp(VerticalInput, -1f, 1f);
    }
    #endregion input methods

    /// <summary>
    /// Returns if our character is currently facing left
    /// </summary>
    /// <returns></returns>
    public bool GetIsFacingLeft()
    {
        return bIsFacingLeft;
    }

    /// <summary>
    /// Returns whether or not our character can change directions
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanChangeDirection()
    {
        return true;
    }


    /// <summary>
    /// Sets whether or not our character is facing left. If there is a SpriteRenderer attached, this will set the transform of the renderer
    /// negative if facing left and positive if facing right.
    /// </summary>
    /// <param name="bIsFacingLeft"></param>
    public void SetIsFacingLeft(bool bIsFacingLeft, bool bForceSetDirection = false)
    {
        if ((this.bIsFacingLeft == bIsFacingLeft || !CanChangeDirection()) && !bForceSetDirection)
        {
            return;
        }
        this.bIsFacingLeft = bIsFacingLeft;
        Vector3 NewScale = CharacterSpriteRenderer.transform.localScale;
        NewScale.x = Mathf.Abs(NewScale.x) * (bIsFacingLeft ? -1f : 1f);
        CharacterSpriteRenderer.transform.localScale = NewScale;
    }
}
