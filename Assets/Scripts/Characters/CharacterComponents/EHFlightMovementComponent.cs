using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement component that handles movement of the character that it is attached to. In particular this handles flight logic for our character
/// </summary>
public class EHFlightMovementComponent : EHBaseMovementComponent
{
    #region main variables
    public float Acceleration = 15f;
    public float MaxVelocity = 5f;
    #endregion main variables

    #region override methods
    protected override void UpdateVelocityFromInput(Vector2 CurrentInput, Vector2 PreviousInput)
    {
        Vector2 FlightDirection = CurrentInput.normalized;
        Vector2 GoalVelcoity = FlightDirection * MaxVelocity;

        Physics2D.Velocity = Vector2.MoveTowards(Physics2D.Velocity, GoalVelcoity, EHTime.DeltaTime * Acceleration);
    }
    #endregion override methods
}
