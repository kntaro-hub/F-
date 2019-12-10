using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemObject(Point point, int id)
    {
        this.point = point;
        this.itemID = id;
        
    }

    // =--------- 変数宣言 ---------= //

    // マップ上座標
    public Point point;

    // アイテムID
    private int itemID;
    public int ItemID
    {
        get { return itemID; }
        set { itemID = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
