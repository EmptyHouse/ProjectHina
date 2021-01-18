using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
    /// 
    /// </summary>
    public DataTableManager DataTableManager { get; } = new DataTableManager();

    public RoomActor CurrentlyLoadedRoom { get; set; }

    public EHGameHUD GameHUD { get; set; }

    #region game component references
    public MainCameraFollow MainGameCamera { get; set; }
    #endregion game component references
    #region monobehaviour methods
    protected virtual void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    protected virtual void LateUpdate()
    {
        PhysicsManager.Tick(EHTime.DeltaTime);
        HitboxManager.Tick(EHTime.DeltaTime);
    }
    #endregion monobehaviour methods

    #region saving/loading
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
        public ushort LevelID;
        public byte CheckPointID;
        public DateTime TimeOfSave;
        public float TimePlayingGame;
        public int NumberOfDeaths;

        public SaveGameData()
        {
            SaveGameVersion = SAVE_VERSION;
        }

        public bool IsSaveOutOfDate()
        {
            return SaveGameVersion == SAVE_VERSION;
        }
    }
    #endregion saving/loading

}
