using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EHDebug
{
    #region const variables
    private const string FRAME_COUNTER_ON = "FrameCounter";
    private const string DEBUG_UI_SCREEN_ON = "DebugScreenOn";
    #endregion const variables


    public static EHDebugUIScreen DebugUIScreenInstance;




    /// <summary>
    /// Toggles whether or not to disable the entire debug screen. This will disable all components that our debug screen contains
    /// </summary>
    /// <param name="DebugScreenOn"></param>
    public static void ToggleDebugScreen(bool DebugScreenOn)
    {

    }

    /// <summary>
    /// Add a debug message that will be displayed our on our debug screen temporarily
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="TimeToDisplay"></param>
    /// <param name="DebugTextColor"></param>
    public static void DebugScreenMessage(string Message, float TimeToDisplay = 0, Color? DebugTextColor = null)
    {
        if (DebugUIScreenInstance)
        {
            DebugUIScreenInstance.DebugAddMessage(Message, TimeToDisplay, DebugTextColor ?? Color.red);
        }
    }

    /// <summary>
    /// Toggles whether or not we will display our frame counter in the Debug screen
    /// </summary>
    /// <param name="FrameCounterOn"></param>
    public static void ToggleFrameCounter(bool FrameCounterOn)
    {
        if (DebugUIScreenInstance)
        {

        }
    }
    
    public static void RayTraceDrawLine(EHRayTraceParams RayParams)
    {
        RayTraceDrawLine(RayParams, Color.red);
    }

    /// <summary>
    /// Draws a debug line 
    /// </summary>
    /// <param name="RayParams"></param>
    public static void RayTraceDrawLine(EHRayTraceParams RayParams, Color LineColor)
    {
        Debug.DrawLine(RayParams.RayOrigin, RayParams.RayOrigin + RayParams.RayDirection * RayParams.RayLength, LineColor);
    }
}
