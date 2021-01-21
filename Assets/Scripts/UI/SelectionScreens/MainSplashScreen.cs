using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSplashScreen : BaseSelectionUI
{

    #region input methods
    /// <summary>
    /// Starts a brand new game and puts player at the first level
    /// </summary>
    public void OnNewGamePressed()
    {
        Debug.Log("New Game Was Clicked");
    }

    /// <summary>
    /// Load game from a previous save file
    /// </summary>
    public void OnLoadGamePressed()
    {
        Debug.Log("Load Game Was Pressed");
    }

    /// <summary>
    /// Adjust various options in a new screen
    /// </summary>
    public void OnOptionsPressed()
    {
        Debug.Log("Options Button Was pressed");
    }

    /// <summary>
    /// Extis the game :(
    /// </summary>
    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
    #endregion input methods
}
