using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
/// <summary>
/// Animation updator script manually updates our animation controllers. This will allow us to make animations frame dependant if the need comes.
/// We can also cap delta times this way, so that we do not have situations where the deltatime is greater than our max allowed delta time
/// </summary>
public class AnimationUpdater : MonoBehaviour
{
    private Animator Anim;

    #region monobehaviour methods
    private void Awake()
    {
        Anim = GetComponent<Animator>();
        Anim.enabled = false;
    }

    private void OnEnable()
    {
        Anim.enabled = false;
        StartCoroutine(AnimationCoroutine());
    }

    private void OnDisable()
    {
        Anim.enabled = true;
    }
    #endregion monobehaviour methods

    private IEnumerator AnimationCoroutine()
    {
        while (true)
        {
            Anim.Update(EHTime.DeltaTime);
            yield return null;
        }
    }
}
