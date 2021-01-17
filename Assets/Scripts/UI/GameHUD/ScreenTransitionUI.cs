using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenTransitionUI : MonoBehaviour
{
    [SerializeField]
    private float TransitionTimeSeconds = 1f;
    [Header("UI Components")]
    [SerializeField]
    private Image Image_BlackScreen;
    private bool bIsExecutingSceneTransition = false;


    private void Awake()
    {
        Image_BlackScreen.color = new Color(0, 0, 0, 0);
    }

    public void StartSceneTransition(EHPlayerCharacter PlayerCharacter, DoorData DoorToLoad)
    {
        if (bIsExecutingSceneTransition)
        {
            Debug.LogWarning("Scene Transition Is Currently In Progress");
        }
        StartCoroutine(SceneTransitionCoroutine(PlayerCharacter, DoorToLoad));
    }

    private IEnumerator SceneTransitionCoroutine(EHPlayerCharacter PlayerCharacter, DoorData DoorToLoad)
    {
        if (!PlayerCharacter)
        {
            Debug.LogWarning("The player that was passed in was null");
            yield break;
        }
        if (!DoorToLoad)
        {
            Debug.LogWarning("The Door to load was null");
            yield break;
        }
        PlayerCharacter.GetComponent<EHPlayerController>().enabled = false;
        float TimeThatHasPassed = 0;
        while (TimeThatHasPassed < TransitionTimeSeconds)
        {

            yield return null;
            TimeThatHasPassed += EHTime.RealDeltaTime;
            Image_BlackScreen.color = new Color(0, 0, 0, TimeThatHasPassed / TransitionTimeSeconds);
        }

        RoomActor CurrentlyLoadedRoom = BaseGameOverseer.Instance.CurrentlyLoadedRoom;
        if (CurrentlyLoadedRoom && CurrentlyLoadedRoom.GetAssociatedRoomData())
        {
            SceneManager.UnloadSceneAsync(CurrentlyLoadedRoom.GetAssociatedRoomData().RoomScene);
        }
        AsyncOperation SceneOperation = SceneManager.LoadSceneAsync(DoorToLoad.GetDoorRoom().RoomScene, LoadSceneMode.Additive);
        while (!SceneOperation.isDone)
        {
            yield return null;
        }

        //PlayerCharacter.transform.position = DoorToLoad.
        TimeThatHasPassed = 0;
        while (TimeThatHasPassed < TransitionTimeSeconds)
        {
            yield return null;
            TimeThatHasPassed += EHTime.RealDeltaTime;
            Image_BlackScreen.color = new Color(0, 0, 0, 1 - (TimeThatHasPassed / TransitionTimeSeconds));
        }
        PlayerCharacter.GetComponent<EHPlayerController>().enabled = true;
    }
}
