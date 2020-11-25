using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region main variables
    public bool IsTimeSlowed;
    #endregion main variables

    public EHPhysicsManager2D PhysicsManager { get; private set; }
    public EHHitboxManager HitboxManager { get; private set; }

    #region monobehaviour methods
    protected virtual void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;

        HitboxManager = new EHHitboxManager();
        PhysicsManager = new EHPhysicsManager2D();
    }

    protected virtual void LateUpdate()
    {


        HitboxManager.Tick(EHTime.DELTA_TIME);
        PhysicsManager.Tick(EHTime.DELTA_TIME);
    }
    #endregion monobehaviour methods

}
