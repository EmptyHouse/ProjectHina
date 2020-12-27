using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDataTable", menuName = "ScriptableObjects/AttackTables", order = 1)]
/// <summary>
/// This data table would best be used for various characters that have melee attacks. Attack Data will be associated with the animation clip that is currently playing
/// </summary>
public class AttackDataTable : BaseDataTable<FAttackDataNode>
{
    protected override void InitializeTable(FAttackDataNode[] DataCollection)
    {
        foreach (FAttackDataNode AttackNode in DataCollection)
        {
            DataTableDictioanry.Add(AttackNode.AttackDataAnimationClip.name, AttackNode);
        }
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
    public FAttackData[] AttackData;
}
