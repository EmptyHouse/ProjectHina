using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EHBox2DCollider))]
public class DialogueTrigger : MonoBehaviour
{

    #region main variables
    private EHBox2DCollider DialogueBoxCollider;
    private EHPlayerCharacter PlayerCharacterReference;
    #endregion main variables

    #region monobeaviour methods
    private void Awake()
    {
        DialogueBoxCollider = GetComponent<EHBox2DCollider>();
        if (!DialogueBoxCollider) 
        {
            Debug.LogWarning("No Box Collider trigger was found for our dialogue trigger component");
            return;
        }
        DialogueBoxCollider.OnTrigger2DEnter += OnPlayerEnterDialogueTrigger;
        DialogueBoxCollider.OnTrigger2DExit += OnPlayerExitDialogueTrigger;
    }
    #endregion monobehaviour methods
    private void OnActionButtonPressed()
    {

    }

    /// <summary>
    /// Called whenever a trigger has entered our trigger object
    /// </summary>
    /// <param name="HitData"></param>
    private void OnPlayerEnterDialogueTrigger(FTriggerData HitData)
    {
        EHPlayerCharacter PlayerCharacter = HitData.OtherCollider.GetComponent<EHPlayerCharacter>();
        if (PlayerCharacter != null)
        {
            PlayerCharacterReference = PlayerCharacter;
            PlayerCharacterReference.PlayerController.BindActionToInput(EHPlayerController.JUMP_COMMAND, EHBaseController.ButtonInputType.Button_Pressed, OnActionButtonPressed);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="HitData"></param>
    private void OnPlayerExitDialogueTrigger(FTriggerData HitData)
    {
        EHPlayerCharacter PlayerCharacter = HitData.OtherCollider.GetComponent<EHPlayerCharacter>();
        if (PlayerCharacter != null)
        {
            PlayerCharacterReference = null;
            PlayerCharacterReference.PlayerController.UnbindActionToInput(EHPlayerController.JUMP_COMMAND, EHBaseController.ButtonInputType.Button_Pressed, OnActionButtonPressed);
        }
    }
}
