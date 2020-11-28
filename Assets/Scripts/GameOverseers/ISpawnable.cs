using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnable
{
    /// <summary>
    /// Called right after our object has been activated
    /// </summary>
    void OnSpawn();

    /// <summary>
    /// Called Right before our object is deactivated
    /// </summary>
    void OnDespawn();
    /// <summary>
    /// Returns the attached gameobject component
    /// </summary>
    /// <returns></returns>
    GameObject GetGameObject();
}
