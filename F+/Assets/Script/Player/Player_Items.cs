using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Items : MonoBehaviour
{
    private struct StockItem
    {
        public int itemID;
        public int stockID;

        public StockItem(int itemId, int stockId) : this()
        {
            this.itemID = itemId;
            this.stockID = stockId;
        }
    }

    private List<StockItem> stocks = new List<StockItem>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int StockCount()
    {
        return stocks.Count;
    }

    public int GetItemID(int i)
    {
        return stocks[i].itemID;
    }

    public int GetStockID(int i)
    {
        return stocks[i].stockID;
    }

    public void AddItem(int itemId)
    {
        stocks.Add(new StockItem(itemId, this.GetID()));
    }
    public void Erase(int StockID)
    {
        for (int i = stocks.Count - 1; i >= 0; i--)
        {// 逆順ループ
            if (stocks[i].stockID == StockID)
            {
                stocks.RemoveAt(i);
                break;
            }
        }
    }

    private int GetID()
    {
        int cnt = 0;
        while (true)
        {
            bool isDecision = true;
            foreach (var itr in stocks)
            {
                if(itr.stockID == cnt)
                {// すでにIDがあった場合
                    isDecision = false;
                    break;
                }
            }
            if(isDecision)
            {// IDが見つかった場合
                return cnt;
            }
            ++cnt;
        }
    }
}
