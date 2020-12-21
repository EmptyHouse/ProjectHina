using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static time class that will be used to help with anything time related in our game
/// </summary>
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
    /// <summary>
    /// The scale that will be applied to our delta time. This should slow down or speed up gameplay based on the scale. Keep in mind this is a global scale. 
    /// You will need to use TimeScale found in gameplay characters to affect characters individually
    /// </summary>
    public static float TimeScale { get; private set; }

    /// <summary>
    /// The float time that indicates when our game was first launched
    /// </summary>
    public static float TimeOfGameLaunch { get; private set; }
    /// <summary>
    /// Time that indicates when our play session started. Everytime a game session is exited, we should remember to save the time that has
    /// passed for record. This will be reset every time a new game session is started.
    /// </summary>
    public static float TimeOfGameplayStart;

    static EHTime()
    {
        TimeGroupTimeScaleDictionary.Add(EHTime.TimeGroup.NONE, 1);
        TimeGroupTimeScaleDictionary.Add(EHTime.TimeGroup.PLAYER, 1);
        TimeGroupTimeScaleDictionary.Add(EHTime.TimeGroup.ENEMY, 1);
        TimeScale = 1;
        TimeOfGameLaunch = Time.unscaledTime;//Keep track of when the game has been launched. You may want to do some debugging on this to ensure that it is launched at start

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
    /// Sets the time scale for a specific time group
    /// NOTE: Not entirely sure if we will use thsi system yet. Remove if we do not
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
