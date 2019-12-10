using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    // =--------- テーブル ---------= //
    [SerializeField]
    private EnemyTable table_Enemy;

    [SerializeField]
    private WeaponTable table_weapon;

    [SerializeField]
    private ShieldTable table_shield;

    [SerializeField]
    private LevelTable table_Level;

    [SerializeField]
    private ItemTable table_Item;

    // =--------- ゲッター ---------= //

    public EnemyTableEntity GetEnemyTable(int id)
    {
        return table_Enemy.Table[id];
    }
    public WeaponTableEntity GetWeaponTable(int id)
    {
        return table_weapon.Table[id];
    }
    public ShieldTableEntity GetShiledTable(int id)
    {
        return table_shield.Table[id];
    }
    public LevelTableEntity GetLevelTable(int id)
    {
        return table_Level.Table[id];
    }
    public ItemTableEntity GetItemTable(int id)
    {
        return table_Item.Table[id];
    }

    public int GetEnemyTableCount()
    {
        return table_Enemy.Table.Count;
    }
    public int GetWeaponTableCount()
    {
        return table_weapon.Table.Count;
    }
    public int GetShiledTableCount()
    {
        return table_shield.Table.Count;
    }
    public int GetLevelTableCount()
    {
        return table_Level.Table.Count;
    }
    public int GetItemTableCount()
    {
        return table_Item.Table.Count;
    }
    #region singleton

    static DataBase _instance;

    public static DataBase instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(DataBase));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use DataBase in the scene hierarchy.");
                    _instance = (DataBase)previous;
                }
                else
                {
                    var go = new GameObject("DataBase");
                    _instance = go.AddComponent<DataBase>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
