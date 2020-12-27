using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDataTable<T> : ScriptableObject
{
    [SerializeField]
    private string DataTableName;
    [SerializeField]
    private T[] DataElements;
    protected Dictionary<string, T> DataTableDictioanry = new Dictionary<string, T>();

    #region monobeahviour methods
    private void Awake()
    {
        
    }
    #endregion monobehaviour methods

    protected abstract void InitializeTable(T[] DataCollection);

    /// <summary>
    /// Returns the name of the 
    /// </summary>
    /// <returns></returns>
    public string GetTableName() { return DataTableName; }

    /// <summary>
    /// Returns a struct that contains all the valuess of our Data element
    /// </summary>
    /// <param name="RowName"></param>
    /// <param name="DataValue"></param>
    /// <returns></returns>
    public bool GetItemByRowName(string RowName, out T DataValue)
    {
        if (DataTableDictioanry.ContainsKey(RowName))
        {
            DataValue = DataTableDictioanry[RowName];
            return true;
        }
        DataValue = default(T);
        return false;
    }
}
