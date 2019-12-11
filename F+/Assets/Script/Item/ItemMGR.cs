using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMGR : MonoBehaviour
{
    [SerializeField]
    private ItemObject itemPrefab;

    private List<ItemObject> items = new List<ItemObject>();

    // Start is called before the first frame update
    void Start()
    {
        this.CreateItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// マップの部屋のどこかにランダムで4～7個のアイテムを配置する
    /// </summary>
    private void CreateItems()
    {
        int cnt = Random.Range(4, 7);
        for (int i = 0; i < cnt; ++i)
        {
            ItemObject item = Instantiate(itemPrefab);
            item.point = MapGenerator.instance.RandomPointInRoom();
            item.transform.position = MapData.GridToWorld(item.point);

            // ここでアイテムIDを設定する
            item.ItemID = Random.Range(0, DataBase.instance.GetItemTableCount()- 1);

            items.Add(item);
            item.transform.parent = this.transform;
            MapData.instance.SetMapObject(item.point, MapData.MapObjType.item, item.ItemID);
        }
    }

    /// <summary>
    /// マップの指定の場所にアイテムを配置する
    /// </summary>
    public void CreateItem(Point point)
    {
        ItemObject item = Instantiate(itemPrefab);
        item.point = point;
        item.transform.position = MapData.GridToWorld(item.point);

        items.Add(item);
    }

    public void UpdateMapObject()
    {
        foreach(var itr in items)
        {
            MapData.instance.SetMapObject(itr.point, MapData.MapObjType.item, itr.ItemID);
        }
    }

    public void DestroyItem(Point point)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {// 逆順ループ
            if (point == items[i].point)
            {
                Destroy(items[i].gameObject);
                items.RemoveAt(i);
                return;
            }
        }
    }


    /// <summary>
    /// アイテムの効果を発揮させる
    /// </summary>
    /// <param name="itemID">アイテムID</param>
    /// <param name="actor">効果を反映させたいキャラクター</param>
    public Actor.Parameter UseItem(int itemID, Actor.Parameter actorParam)
    {
        ItemTableEntity item = DataBase.instance.GetItemTable(itemID);
        Actor.Parameter param = actorParam;
        param.hp += item.HP;
        param.basicAtk += item.Atk;
        param.hunger += item.Hunger;

        if(param.hp > param.maxHp)
        {
            param.hp = param.maxHp;
        }
        if(param.hunger > param.maxHunger)
        {
            param.hunger = param.maxHunger;
        }

        actorParam = param;
        return actorParam;
    }

    /// <summary>
    /// 武器を装備させる
    /// </summary>
    /// <param name="itemID">アイテムID</param>
    /// <param name="actor">武器を装備させたいキャラクター</param>
    public Actor.Parameter EquipWeapon(int itemID, Actor.Parameter actorParam)
    {
        ItemTableEntity item = DataBase.instance.GetItemTable(itemID);
        Actor.Parameter param = actorParam;
        param.weaponId = itemID;

        actorParam = param;
        return actorParam;
    }

    /// <summary>
    /// 盾を装備させる
    /// </summary>
    /// <param name="itemID">アイテムID</param>
    /// <param name="actor">盾を装備させたいキャラクター</param>
    public Actor.Parameter EquipShield(int itemID, Actor.Parameter actorParam)
    {
        ItemTableEntity item = DataBase.instance.GetItemTable(itemID);
        Actor.Parameter param = actorParam;
        param.shieldId = itemID;

        actorParam = param;
        return actorParam;
    }

    /// <summary>
    /// 装備を解除する
    /// </summary>
    /// <param name="itemID">アイテムID</param>
    /// <param name="actor">盾を装備させたいキャラクター</param>
    public Actor.Parameter RemoveEquip(Actor.Parameter actorParam, UI_Inventory.EquipType type)
    {
        Actor.Parameter param = actorParam;
        if(type == UI_Inventory.EquipType.weapon)
        {
            param.weaponId = Actor.Parameter.notEquipValue;
        }
        else
        {
            param.shieldId = Actor.Parameter.notEquipValue;
        }
        
        actorParam = param;
        return actorParam;
    }

    #region singleton

    static ItemMGR _instance;

    public static ItemMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(ItemMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use ItemMGR in the scene hierarchy.");
                    _instance = (ItemMGR)previous;
                }
                else
                {
                    var go = new GameObject("ItemMGR");
                    _instance = go.AddComponent<ItemMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
