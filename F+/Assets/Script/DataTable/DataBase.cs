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

    public EnemyTableEntity GetEnemyTableEntity(int id)
    {
        return table_Enemy.Table[id];
    }
    public WeaponTableEntity GetWeaponTableEntity(int id)
    {
        return table_weapon.Table[id];
    }
    public ShieldTableEntity GetShiledTableEntity(int id)
    {
        return table_shield.Table[id];
    }
    public LevelTableEntity GetLevelTableEntity(int id)
    {
        return table_Level.Table[id];
    }
    public ItemTableEntity GetItemTableEntity(int id)
    {
        return table_Item.Table[id];
    }

    public List<EnemyTableEntity> GetEnemyTable()
    {
        return table_Enemy.Table;
    }
    public List<WeaponTableEntity> GetWeaponTable()
    {
        return table_weapon.Table;
    }
    public List<ShieldTableEntity> GetShiledTable()
    {
        return table_shield.Table;
    }
    public List<LevelTableEntity> GetLevelTable()
    {
        return table_Level.Table;
    }
    public List<ItemTableEntity> GetItemTable()
    {
        return table_Item.Table;
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
