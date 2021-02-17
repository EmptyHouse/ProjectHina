using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement component that handles movement of the character that it is attached to. In particular this handles flight logic for our character
/// </summary>
public class EHFlightMovementComponent : MonoBehaviour
{
    #region main variables
    public float Acceleration = 15f;
    public float MaxVelocity = 5f;
    private bool bIsFacingLeft = false;

    private EHPhysics2D Physics2D;
    private Vector2 CurrentInput;
    private SpriteRenderer CharacterSpriteRenderer;
    private Vector2 PreviousInput;

    #endregion main variables

    #region monobehaviour methods
    private void Awake()
    {
        Physics2D = GetComponent<EHPhysics2D>();
        CharacterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        CurrentInput = Vector2.zero;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            SetVerticalInput(-1);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            SetVerticalInput(1);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            SetHorizontalInput(1);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            SetHorizontalInput(-1);
        }

        UpdateVelocityFromInput();
        PreviousInput = CurrentInput;
    }
    #endregion monobehaviour methods

    protected virtual void UpdateVelocityFromInput()
    {
        Vector2 FlightDirection = CurrentInput.normalized;
        Vector2 GoalVelcoity = FlightDirection * MaxVelocity;

        Physics2D.Velocity = Vector2.MoveTowards(Physics2D.Velocity, GoalVelcoity, EHTime.DeltaTime * Acceleration);
    }

    public void SetHorizontalInput(float HorizontalInput)
    {
        CurrentInput.x = HorizontalInput;
        if (HorizontalInput < 0 && !bIsFacingLeft)
        {
            SetIsFacingLeft(true);
        }
        else if (HorizontalInput > 0 && bIsFacingLeft)
        {
            SetIsFacingLeft(false);
        }

    }

    public void SetVerticalInput(float VerticalInput)
    {
        CurrentInput.y = VerticalInput;
    }

    public void SetIsFacingLeft(bool bIsFacingLeft)
    {
        if (bIsFacingLeft == this.bIsFacingLeft)
        {
            return;
        }
        this.bIsFacingLeft = bIsFacingLeft;
        CharacterSpriteRenderer.transform.localScale = Vector3.Scale(this.transform.localScale, new Vector3((this.bIsFacingLeft ? -1 : 1), 1, 1));
    }
}
