using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHSpawnPool : MonoBehaviour
{
    #region static variables
    private static EHSpawnPool instance;
    public static EHSpawnPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<EHSpawnPool>();
            }
            return instance;
        }
    }
    #endregion static variables

    public SpawnComponent[] InitialSpawnObjects = new SpawnComponent[0];
    public Transform DespawnContainerTransform;
    private Dictionary<string, Queue<ISpawnable>> SpawnPoolDictionary = new Dictionary<string, Queue<ISpawnable>>(); 

    #region monobeahviour methods
    private void Awake()
    {
        instance = this;
    }
    #endregion monobeahviour methods

    public static void Spawn(ISpawnable ObjectToSpawn)
    {
        if (instance == null)
        {
            Debug.LogError("Trying to access a null spawn pool");
        }

    }

    public static void Despawn()
    {
        if (instance == null)
        {

        }
    }

    public void DespawnAfterTime(ISpawnable ObjectToDespawn, float TimeUntilDespawn, bool bIsRealTime = false)
    {
        StartCoroutine(DespawnCoroutine(ObjectToDespawn, TimeUntilDespawn, bIsRealTime));
    }

    private void CreateObjectForPool(GameObject ObjectToCreate)
    {
        GameObject NewObject = Instantiate(ObjectToCreate);
        NewObject.gameObject.SetActive(false);
        NewObject.transform.SetParent(DespawnContainerTransform);
        NewObject.name = NewObject.name.Substring(0, NewObject.name.Length - "(Clone)".Length);

        if (!SpawnPoolDictionary.ContainsKey(NewObject.name))
        {
            SpawnPoolDictionary.Add(NewObject.name, new Queue<ISpawnable>());
        }
        // To do, be sure to add a way to add the spawn pool object to the dictionary 
    }

    private IEnumerator DespawnCoroutine(ISpawnable Spawnable, float TimeUntilDespawn, bool bIsRealTime = false)
    {
        float TimeThatHasPassed = 0;
        while ((TimeThatHasPassed += EHTime.DeltaTime) < TimeUntilDespawn)
        {
            yield return null;
        }
        Destroy(Spawnable.GetGameObject());
    }

    [System.Serializable]
    public struct SpawnComponent
    {
        public ISpawnable SpawnableObject;
        public int InitialNumberOfSpawnedObjects;
    }
}
