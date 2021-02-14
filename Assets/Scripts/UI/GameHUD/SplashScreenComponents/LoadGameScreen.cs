using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Splash screen UI component. Handles loading a file and beginning where the game was last saved
/// </summary>
public class LoadGameScreen : BaseSelectionUI
{

    #region monobehaviour methods
    protected override void Awake()
    {
        base.Awake(); 
    }
    #endregion monobehaviour methods

    #region input methods
    /// <summary>
    /// when we select a load file, this will begin the transition to the game that correlates to that file if it is valid
    /// </summary>
    /// <param name="LoadFileSelection"></param>
    public void OnLoadFileSelected(byte LoadFileSelection)
    {

    }

    /// <summary>
    /// Returns to the main splash screen menu
    /// </summary>
    public void OnBackButtonPressed()
    {

    }

    /// <summary>
    /// Prompts the user to delete a save file
    /// </summary>
    public void OnDeleteSaveButtonPressed()
    {

    }
    #endregion input methods
}
