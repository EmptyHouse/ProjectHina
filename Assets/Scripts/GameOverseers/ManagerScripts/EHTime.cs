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

    /// <summary>
    /// Max delta constant between each frame. If delta time reaches above this value, this is what will be returned instead.
    /// </summary>
    private const float MAX_DELTA_TIME = .1f;//in case we ever dip below 10 fps

    /// <summary>
    /// The time in seconds that has passed between each frame. This contains a max cap of time that has passed between each.
    /// If the frame rate dips below that, this will return .1f
    /// </summary>
    public static float DeltaTime
    {
        get
        {
            return TimeScale * Mathf.Min(Time.deltaTime, MAX_DELTA_TIME);
        }
    }

    /// <summary>
    /// Unscaled Delta Time between each frame. Like DeltaTime, this will also contain a cap on the amount
    /// of time that has passed between each frame.
    /// </summary>
    public static float RealDeltaTime
    {
        get
        {
            return Mathf.Min(Time.deltaTime, MAX_DELTA_TIME);
        }
    }

    public static float TimeScale { get; private set; }

    static EHTime()
    {
        TimeGroupTimeScaleDictionary.Add(EHTime.TimeGroup.NONE, 1);
        TimeGroupTimeScaleDictionary.Add(EHTime.TimeGroup.PLAYER, 1);
        TimeGroupTimeScaleDictionary.Add(EHTime.TimeGroup.ENEMY, 1);
        TimeScale = 1;
    }

    /// <summary>
    /// Sets the time scale of the game
    /// </summary>
    /// <param name="NewTimeScale"></param>
    public static void SetTimeScale(float NewTimeScale)
    {
        if (NewTimeScale < 0)
        {
            TimeScale = 0;
            return;
        }
        TimeScale = NewTimeScale;
    }
    
    /// <summary>
    /// Returns the delta time for the group that is passed in
    /// </summary>
    /// <param name="Group"></param>
    /// <returns></returns>
    public static float DeltaTimeGroup(TimeGroup Group = TimeGroup.NONE)
    {
        if (TimeGroupTimeScaleDictionary.ContainsKey(Group))
        {
            return DeltaTime * TimeGroupTimeScaleDictionary[Group];
        }
        Debug.LogWarning("There is no group assigned to our dictionary");
        return DeltaTime;
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
