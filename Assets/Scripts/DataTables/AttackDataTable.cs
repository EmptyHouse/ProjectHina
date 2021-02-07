using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDataTable", menuName = "ScriptableObjects/AttackTables", order = 1)]
/// <summary>
/// This data table would best be used for various characters that have melee attacks. Attack Data will be associated with the animation clip that is currently playing
/// </summary>
public class AttackDataTable : BaseDataTable<FAttackDataNode>
{
    private Dictionary<int, string> AnimationHashToRowName = new Dictionary<int, string>();

    protected override void InitializeTable(FAttackDataNode[] DataCollection)
    {
        foreach (FAttackDataNode AttackNode in DataCollection)
        {
            DataTableDictioanry.Add(AttackNode.AttackDataAnimationClip.name, AttackNode);
            AnimationHashToRowName.Add(Animator.StringToHash(AttackNode.AttackDataAnimationClip.name), AttackNode.AttackDataAnimationClip.name);
        }
    }

    /// <summary>
    /// Retutns the Attack data that is associasted with the animation clip hash
    /// </summary>
    /// <param name="AnimationClipHash"></param>
    /// <returns></returns>
    public bool GetAttackDataFromAnimationClipHash(int AnimationClipHash, out FAttackData AttackData)
    {
        if (!AnimationHashToRowName.ContainsKey(AnimationClipHash))
        {
            Debug.LogWarning("The Animation clip hash that was passed in has not been setup. ANIMATION HASH: " + AnimationClipHash);
            AttackData = default;
            return false;
        }
        string AttackDataName = AnimationHashToRowName[AnimationClipHash];
        FAttackDataNode AttackNode = DataTableDictioanry[AttackDataName];
        AttackData = DataTableDictioanry[AttackDataName].AttackData;
        return true;
    }
}

/// <summary>
/// 
/// </summary>
[System.Serializable]
public struct FAttackDataNode
{
    public string AttackName;
    public AnimationClip AttackDataAnimationClip;
    public FAttackData AttackData;
}
