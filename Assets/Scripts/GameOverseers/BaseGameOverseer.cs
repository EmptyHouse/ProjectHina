﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

/// <summary>
/// Overseer for all gameplay related events. All overseers related to gameplay should have to derive from this class. Make sure it
/// is generic enough for all gameplay logic
/// </summary>
public class BaseGameOverseer : MonoBehaviour
{
    #region static variables
    private static BaseGameOverseer instance;
    public static BaseGameOverseer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<BaseGameOverseer>();
            }
            return instance;
        }
    }
    #endregion static variables
    /// <summary>
    /// Reference to our Physics Manager object
    /// </summary>
    public EHPhysicsManager2D PhysicsManager { get; } = new EHPhysicsManager2D();
    /// <summary>
    /// Reference to our Hitbox Manager object
    /// </summary>
    public EHHitboxManager HitboxManager { get; } = new EHHitboxManager();
    /// <summary>
    /// Contains a collection of datatables that can be used in our game. 
    /// </summary>
    public DataTableManager DataTableManager { get; } = new DataTableManager();
    /// <summary>
    /// Contains logic for managing certain global effects in our game such as CameraShake and Freeze time as these types of effects should not be managed by 
    /// individual actors
    /// </summary>
    public EHGlobalEffectManager GlobalEffectManager { get; } = new EHGlobalEffectManager();
    /// <summary>
    /// The currently loaded room that our character is in
    /// </summary>
    public RoomActor CurrentlyLoadedRoom { get; set; }
    /// <summary>
    /// The Associated GameHUD
    /// </summary>
    public EHGameHUD GameHUD { get; set; }

    #region game component references
    /// <summary>
    /// Our main camera component in the game scene. The camera component that is currently following our character
    /// </summary>
    public MainCameraFollow MainGameCamera { get; set; }

    /// <summary>
    /// The associated player controller in our game. This will also have a reference to the player's gameplay character
    /// </summary>
    public EHPlayerController PlayerController { get; set; }

    [SerializeField]
    private EHUtilities.SceneField DefaultScene;
    #endregion game component references

    #region monobehaviour methods
    protected virtual void Awake()
    {
        instance = this;
        GameHUD = (EHGameHUD)EHHUD.Instance;
        Application.targetFrameRate = 60;
    }

    protected virtual void Start()
    {
        if (CurrentlyLoadedRoom == null && !GameObject.FindObjectOfType<RoomActor>())
        {
            SceneManager.LoadScene(DefaultScene.SceneName, LoadSceneMode.Additive);
            Debug.LogError("No Environment scene was loaded. Loading Default Scene...");
        }
    }

    protected virtual void Update()
    {
        GlobalEffectManager.Tick(EHTime.DeltaTime);
    }

    protected virtual void LateUpdate()
    {
        PhysicsManager.Tick(EHTime.DeltaTime);
        HitboxManager.Tick(EHTime.DeltaTime);
    }
    #endregion monobehaviour methods

    #region room functions
    /// <summary>
    /// Function should be called from the awake function of room's actor
    /// </summary>
    /// <param name="RoomThatWasLoaded"></param>
    public void OnRoomWasLoaded(RoomActor RoomThatWasLoaded)
    {
        if (CurrentlyLoadedRoom == RoomThatWasLoaded)
        {
            return;
        }
        CurrentlyLoadedRoom = RoomThatWasLoaded;
        if (MainGameCamera)
        {
            MainGameCamera.OnRoomLoaded(CurrentlyLoadedRoom);
        }
    }
    #endregion room functions

    #region saving/loading
    // The path that our game save data will be saved
    private string SAVE_GAME_PATH { get { return Application.persistentDataPath + "/SaveGame.dat"; } }
    private string BACKUP_SAVE_PATH { get { return Application.persistentDataPath + "backup_SaveGame.dat"; } }

    public UnityAction OnSaveCompleted;
    private bool GameCurrentlySaving;

    /// <summary>
    /// Call this to save our game in its current state
    /// </summary>
    public void SaveGame()
    {
        if (GameCurrentlySaving)
        {
            return;
        }
        GameCurrentlySaving = true;
        SaveGameData SaveData = new SaveGameData();
        SaveData.TimeOfSave = DateTime.Now;

        Thread SaveGameThread = new Thread(()=>Threaded_SaveGame(SaveData, SAVE_GAME_PATH));
        SaveGameThread.Start();
    }

    /// <summary>
    /// This is where we will asynchronously save our game data 
    /// </summary>
    /// <param name="GameDataToSave"></param>
    public void Threaded_SaveGame(SaveGameData GameDataToSave, string DataPath)
    {
        try
        {
            BinaryFormatter BFormatter = new BinaryFormatter();
            FileStream FStream = new FileStream(DataPath, FileMode.Create);
            BFormatter.Serialize(FStream, GameDataToSave);
            FStream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        GameCurrentlySaving = false;
        OnSaveCompleted?.Invoke();
    }

    /// <summary>
    /// Loads an instance of the game to the last play session that was saved
    /// </summary>
    public bool LoadGame(string DataPathToLoad, out SaveGameData LoadedGameData)
    {
        LoadedGameData = null;
        if (!File.Exists(DataPathToLoad))
        {
            Debug.LogWarning("There was no save file found ");
            return false;
        }

        try
        {
            BinaryFormatter BFormatter = new BinaryFormatter();
            FileStream FStream = new FileStream(DataPathToLoad, FileMode.Open);
            LoadedGameData = (SaveGameData)BFormatter.Deserialize(FStream);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    [System.Serializable]
    public class SaveGameData
    {
        private const int SAVE_VERSION = 0;
        private int SaveGameVersion;
        // This will indicate the slot that we are saving our game to.
        public byte SaveGameSlot;
        public ushort LevelID;
        // 
        public byte CheckPointID;
        // The time of the last time we saved our game
        public DateTime TimeOfSave;
        // The total time that we have played our game
        public float TimePlayingGame;
        // The number of times our character has died
        public int NumberOfDeaths;

        public SaveGameData()
        {
            SaveGameVersion = SAVE_VERSION;
        }

        /// <summary>
        /// Returns whether or not our save game file is up to date
        /// </summary>
        /// <returns></returns>
        public bool IsSaveOutOfDate()
        {
            return SaveGameVersion == SAVE_VERSION;
        }
    }
    #endregion saving/loading

}
