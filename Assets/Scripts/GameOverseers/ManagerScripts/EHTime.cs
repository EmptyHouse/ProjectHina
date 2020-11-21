using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EHTime
{
    #region enums
    public enum TimeGroup
    {
        NONE,
        PLAYER,
        ENEMY,
    }
    #endregion enums

    private static Dictionary<TimeGroup, float> TimeGroupTimeScaleDictionary = new Dictionary<TimeGroup, float>();
    public static float DELTA_TIME
    {
        get
        {
            return Time.deltaTime;
        }
    }

    static EHTime()
    {
        TimeGroupTimeScaleDictionary.Add(EHTime.TimeGroup.NONE, 1);
        TimeGroupTimeScaleDictionary.Add(EHTime.TimeGroup.PLAYER, 1);
        TimeGroupTimeScaleDictionary.Add(EHTime.TimeGroup.ENEMY, 1);
    }
    
    /// <summary>
    /// Returns the delta time for the group that is passed in
    /// </summary>
    /// <param name="Group"></param>
    /// <returns></returns>
    public static float DeltaTime(TimeGroup Group = TimeGroup.NONE)
    {
        if (TimeGroupTimeScaleDictionary.ContainsKey(Group))
        {
            return DELTA_TIME * TimeGroupTimeScaleDictionary[Group];
        }
        Debug.LogWarning("There is no group assigned to our dictionary");
        return DELTA_TIME;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Group"></param>
    /// <param name="TimeScale"></param>
    public static void SetTimeScaleForGroup(TimeGroup Group, float TimeScale)
    {
        if (TimeGroupTimeScaleDictionary.ContainsKey(Group))
        {
            TimeGroupTimeScaleDictionary[Group] = TimeScale;
            return;
        }
        Debug.LogWarning("There is no group assigned to our Time Manager");
    }

}
