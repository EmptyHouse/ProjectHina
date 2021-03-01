using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSplashScreen : BaseSelectionUI
{
    [SerializeField]
    private EHUtilities.SceneField DefaultSceneToLoad;
    [SerializeField]
    private Image FadeOutImage;
    [SerializeField]
    private float TimeToFadeOutImage = 2f;
    private bool bIsLoadingGame = false;

    #region input methods
    /// <summary>
    /// Starts a brand new game and puts player at the first level
    /// </summary>
    public void OnNewGamePressed()
    {
        this.enabled = false;
        StartCoroutine(LoadLevelCoroutine(DefaultSceneToLoad));
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
        Debug.Log("Closing Application...");
        Application.Quit();
    }
    #endregion input methods

    private IEnumerator LoadLevelCoroutine(EHUtilities.SceneField SceneToLoad)
    {
        bIsLoadingGame = true;
        float TimeThatHasPassed = 0;
        Color CurrentColor = FadeOutImage.color;
        while (TimeThatHasPassed < TimeToFadeOutImage)
        {
            TimeThatHasPassed += EHTime.DeltaTime;
            yield return null;
            CurrentColor.a = TimeThatHasPassed / TimeToFadeOutImage;
            FadeOutImage.color = CurrentColor;
        }
        CurrentColor.a = 1;
        FadeOutImage.color = CurrentColor;
        SceneManager.LoadScene(SceneToLoad.SceneName);
    }
}
