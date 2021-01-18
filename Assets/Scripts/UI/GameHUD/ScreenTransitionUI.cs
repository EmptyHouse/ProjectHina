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
        Image_BlackScreen.color = Color.clear;
        Image_BlackScreen.gameObject.SetActive(false);
    }

    /// <summary>
    /// Begin a scene transition by passing in the door that we are planning on loading from. 
    /// This will return true if the scene transition began successfully
    /// </summary>
    /// <param name="PlayerCharacter"></param>
    /// <param name="DoorToLoad"></param>
    /// <returns></returns>
    public bool StartSceneTransition(EHPlayerCharacter PlayerCharacter, DoorData DoorToLoad)
    {
        if (bIsExecutingSceneTransition)
        {
            Debug.LogWarning("Scene Transition Is Currently In Progress");
            return false;
        }
        StartCoroutine(SceneTransitionCoroutine(PlayerCharacter, DoorToLoad));
        return true;
    }

    /// <summary>
    /// Coroutine that carries our our scene transition
    /// </summary>
    /// <param name="PlayerCharacter"></param>
    /// <param name="DoorToLoad"></param>
    /// <returns></returns>
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
        bIsExecutingSceneTransition = true;
        Image_BlackScreen.color = Color.clear;
        Image_BlackScreen.gameObject.SetActive(true);

        PlayerCharacter.GetComponent<EHPlayerController>().enabled = false;
        float TimeThatHasPassed = 0;
        while (TimeThatHasPassed < TransitionTimeSeconds)
        {

            yield return null;
            TimeThatHasPassed += EHTime.RealDeltaTime;
            Image_BlackScreen.color = new Color(0, 0, 0, TimeThatHasPassed / TransitionTimeSeconds);
        }
        Image_BlackScreen.color = Color.black;

        RoomActor CurrentlyLoadedRoom = BaseGameOverseer.Instance.CurrentlyLoadedRoom;
        if (CurrentlyLoadedRoom.GetAssociatedRoomData() != DoorToLoad.GetDoorRoom())
        {
            if (CurrentlyLoadedRoom && CurrentlyLoadedRoom.GetAssociatedRoomData())
            {
                print(CurrentlyLoadedRoom.GetAssociatedRoomData().RoomScene);
                yield return SceneManager.UnloadSceneAsync(CurrentlyLoadedRoom.GetAssociatedRoomData().RoomScene);
            }
            yield return SceneManager.LoadSceneAsync(DoorToLoad.GetDoorRoom().RoomScene, LoadSceneMode.Additive);
        }
        yield return new WaitForSecondsRealtime(0.5f);

        DoorActor DoorToSpawnFrom = BaseGameOverseer.Instance.CurrentlyLoadedRoom.GetRoomDoorTriggerFromDoorData(DoorToLoad);
        Vector3 CharacterSpawnPosition = Vector3.zero;
        if (DoorToSpawnFrom == null)
        {
            Debug.LogError("There was no associated door actor to spawn our character");
        }
        else
        {
            CharacterSpawnPosition = DoorToSpawnFrom.GetSpawnPosition();
        }


        PlayerCharacter.SpawnCharacterToPosition(CharacterSpawnPosition);
        BaseGameOverseer.Instance.MainGameCamera.FocusCameraImmediate();
        PlayerCharacter.GetComponent<EHPlayerController>().enabled = true;
        TimeThatHasPassed = 0;
        bIsExecutingSceneTransition = false;
        while (TimeThatHasPassed < TransitionTimeSeconds && !bIsExecutingSceneTransition)
        {
            yield return null;
            TimeThatHasPassed += EHTime.RealDeltaTime;
            Image_BlackScreen.color = new Color(0, 0, 0, 1 - (TimeThatHasPassed / TransitionTimeSeconds));
        }

        Image_BlackScreen.color = Color.clear;
        
        
    }
}
