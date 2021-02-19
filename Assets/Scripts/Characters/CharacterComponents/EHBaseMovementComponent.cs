using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EHBaseMovementComponent : MonoBehaviour
{
    #region const variables
    private const string ANIM_HORIZONTAL_INPUT = "HorizontalInput";
    private const string ANIM_VERTICAL_INPUT = "VerticalInput";
    #endregion const variables

    #region main variables
    private SpriteRenderer CharacterSpriteRenderer;
    private bool bIsFacingLeft;
    protected EHPhysics2D Physics2D;
    protected Animator CharacterAnimator;
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
    public Vector2 AnimationGoalVelocity;

    /// <summary>
    /// Set this value to true if you want to disable the movement component entirely. This will be good if we want another function
    /// to control the movement of our character
    /// </summary>
    [HideInInspector]
    public bool bMovementComponentDisabled;
    #endregion animation controlled variables

    #region input varaibles
    private Vector2 CurrentInput;
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
        UpdateVelocityFromInput(CurrentInput, PreviousInput);
    }
    #endregion monobehaviour methods

    #region update methods
    protected abstract void UpdateVelocityFromInput(Vector2 CurrentInput, Vector2 PreviousInput);
    #endregion update methods

    #region input methods
    /// <summary>
    /// Sets the current input
    /// </summary>
    /// <param name="HorizontalInput"></param>
    public void SetHorizontalInput(float HorizontalInput)
    {
        CurrentInput.x = Mathf.Clamp(HorizontalInput, -1, 1);
    }

    /// <summary>
    /// Set our current vertical Input. This should be a value between -1 and 1. If not we will clamp the values
    /// </summary>
    /// <param name="VerticalInput"></param>
    public void SetVerticalInput(float VerticalInput)
    {
        CurrentInput.y = Mathf.Clamp(VerticalInput, -1, 1);
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
    public virtual bool CanChangeDirection()
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
        if (this.bIsFacingLeft == bIsFacingLeft && !bForceSetDirection)
        {
            return;
        }
        this.bIsFacingLeft = bIsFacingLeft;
        CharacterSpriteRenderer.transform.localScale = Vector3.Scale(new Vector3((bIsFacingLeft ? -1 : 1), 1, 1), CharacterSpriteRenderer.transform.localScale);
    }
}
