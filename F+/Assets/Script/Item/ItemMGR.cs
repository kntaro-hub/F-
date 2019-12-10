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
            item.ItemID = Random.Range(0, DataBase.instance.GetItemTableCount());
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
