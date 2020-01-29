using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMGR : MonoBehaviour
{
    [SerializeField]
    private ItemObject itemPrefab;

    private List<ItemObject> items = new List<ItemObject>();
    public List<ItemObject> Items
    {
        get { return items; }
    }

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
            item.Point = MapGenerator.instance.RandomPointInRoom();
            item.transform.position = MapData.GridToWorld(item.Point);

            // ここでアイテムIDを設定する
            item.ItemID = Random.Range(0, DataBase.instance.GetItemTableCount()- 1);

            UI_MGR.instance.Ui_Map.CreateMapItem(item.Point);

            items.Add(item);
            item.transform.parent = this.transform;
            MapData.instance.SetMapObject(item.Point, MapData.MapObjType.item, item.ItemID);
        }
    }

    /// <summary>
    /// マップの指定の場所にアイテムを配置する
    /// </summary>
    public ItemObject CreateItem(Point point, int itemID)
    {
        ItemObject item = Instantiate(itemPrefab);
        item.Point = point;
        item.transform.position = MapData.GridToWorld(item.Point);
        item.ItemID = itemID;

        MapData.instance.SetMapObject(point, MapData.MapObjType.item, itemID);

        UI_MGR.instance.Ui_Map.CreateMapItem(item.Point);

        items.Add(item);
        return item;
    }

    public void UpdateMapObject()
    {
        foreach(var itr in items)
        {
            MapData.instance.SetMapObject(itr.Point, MapData.MapObjType.item, itr.ItemID);
        }
    }

    public void DestroyItem(Point point)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {// 逆順ループ
            if (point == items[i].Point)
            {
                MapData.instance.ResetMapObject(items[i].Point);
                Destroy(items[i].gameObject);
                items.RemoveAt(i);
                return;
            }
        }
        UI_MGR.instance.Ui_Map.RemoveMapItem();
    }


    /// <summary>
    /// アイテムの効果を発揮させる
    /// </summary>
    /// <param name="itemID">アイテムID</param>
    /// <param name="actor">効果を反映させたいキャラクター</param>
    public void UseItem(int itemID, Actor actor)
    {
        ItemTableEntity item = DataBase.instance.GetItemTableEntity(itemID);
        actor.AddHP(item.HP, true);
        actor.AddAtk(item.Atk);
        actor.AddHunger(item.Hunger);
    }

    /// <summary>
    /// 武器を装備させる
    /// </summary>
    /// <param name="itemID">アイテムID</param>
    /// <param name="actor">武器を装備させたいキャラクター</param>
    public Actor.Parameter EquipWeapon(int itemID, Actor.Parameter actorParam)
    {
        ItemTableEntity item = DataBase.instance.GetItemTableEntity(itemID);
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
        ItemTableEntity item = DataBase.instance.GetItemTableEntity(itemID);
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
    public Actor.Parameter RemoveEquip(Actor.Parameter actorParam, EquipType type)
    {
        Actor.Parameter param = actorParam;
        if(type == EquipType.weapon)
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
