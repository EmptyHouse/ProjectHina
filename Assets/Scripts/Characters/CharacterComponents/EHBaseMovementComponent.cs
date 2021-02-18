using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EHBaseMovementComponent : MonoBehaviour
{
    #region main variables
    private SpriteRenderer CharacterSpriteRenderer;
    private bool bIsFacingLeft;
    #endregion main variables

    #region animation controlled varaibles

    #endregion animation controlled variables

    #region input varaibles
    private Vector2 CurrentInput;
    private Vector3 PreviousInput;
    #endregion input varaibles 

    #region monobehaviour methods
    private void Awake()
    {
        CharacterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        
    }
    #endregion monobehaviour methods

    #region update methods
    protected abstract void UpdateVelocityFromInput(Vector2 CurrentInput, Vector2 PreviousInput);
    #endregion update methods

    #region input methods
    public void SetHorizontalInput(float HorizontalInput)
    {

    }
    #endregion input methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bIsFacingLeft"></param>
    public void SetSpriteRendererIsFacingLeft(bool bIsFacingLeft, bool bForceSetDirection = false)
    {
        if (this.bIsFacingLeft == bIsFacingLeft && !bForceSetDirection)
        {
            return;
        }
        this.bIsFacingLeft = bIsFacingLeft;
        CharacterSpriteRenderer.transform.localScale = Vector3.Scale(new Vector3((bIsFacingLeft ? -1 : 1), 1, 1), CharacterSpriteRenderer.transform.localScale);
    }
}
