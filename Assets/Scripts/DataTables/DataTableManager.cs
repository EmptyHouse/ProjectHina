using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTableManager
{
    private Dictionary<string, AttackDataTable> AttackDataTableDictionary = new Dictionary<string, AttackDataTable>();

    /// <summary>
    /// Add an attack table that we will reference
    /// </summary>
    /// <param name="AttackTable"></param>
    public void AddAttackDataTable(AttackDataTable AttackTable)
    {
        if (AttackTable == null)
        {
            Debug.LogWarning("Null Attack Table was passed in");
            return;
        }
        if (!AttackDataTableDictionary.ContainsKey(AttackTable.name))
        {
            AttackDataTableDictionary.Add(AttackTable.name, GameObject.Instantiate<AttackDataTable>(AttackTable));
        }
    }

    /// <summary>
    /// Returns the associated data table if it has been added to the data manager
    /// </summary>
    /// <param name="AttackTable"></param>
    /// <returns></returns>
    public AttackDataTable GetAttackDataTable(AttackDataTable AttackTable)
    {
        if (AttackDataTableDictionary.ContainsKey(AttackTable.name))
        {
            return AttackDataTableDictionary[AttackTable.name];
        }
        return null;
    }

    /// <summary>
    /// Returns the attack instance of the data table associated with this 
    /// </summary>
    /// <param name="AttackTable"></param>
    /// <param name="AttackHash"></param>
    /// <param name="AttackData"></param>
    /// <param name="MultiHitIndex"></param>
    /// <returns></returns>
    public bool GetAttackDataFromAttackDataTable(AttackDataTable AttackTable, int AttackHash, out FAttackData AttackData, int MultiHitIndex = 0)
    {
        AttackDataTable InstancedAttackTable = GetAttackDataTable(AttackTable);
        if (InstancedAttackTable == null)
        {
            AttackData = default;
            return false;
        }

        return InstancedAttackTable.GetAttackDataFromAnimationClipHash(AttackHash, out AttackData, MultiHitIndex);
    }
}
