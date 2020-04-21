using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ArrowType
{
    wood,
    max
}

class ArrowMGR:MonoBehaviour
{
    

    [SerializeField]
    private ArrowBase[] arrowPrefabs = new ArrowBase[(int)ArrowType.max];

    // 生成した矢リスト
    private List<ArrowBase> arrowList = new List<ArrowBase>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateArrow(ArrowType arrowType, int itemID)
    {
        // 矢生成
        ArrowBase arrow = Instantiate(arrowPrefabs[(int)arrowType], this.transform);

        // ID割り振り
        arrow.SetID(this.SetUniqueID());

        // アイテムID設定（これをもとにステータスを変動させる）
        arrow.SetItemID(itemID);

        // 矢効果発動
        arrow.ActivateArrow();

        // 矢リストに追加
        arrowList.Add(arrow);

        // 杖の残り回数を1減らす

    }

    public void Destroy(int id)
    {
        for (int i = arrowList.Count - 1; i >= 0; i--)
        {// 逆順ループ
            // ID検索してヒットした敵を消す リストからも
            if (arrowList[i].GetID() == id)
            {
                Destroy(arrowList[i].gameObject);
                arrowList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ユニークなIDを設定する
    /// </summary>
    /// <returns>決定したID</returns>
    private int SetUniqueID()
    {
        int ret = 0;
        while (true)
        {
            bool isContinue = false;
            foreach (var itr in arrowList)
            {// 1件でも同じIDがあれば次の数字へ
                if (itr.GetID() == ret)
                {
                    // 数字を進める
                    ++ret;
                    isContinue = true;
                    break;
                }
            }
            if (isContinue) continue;
            return ret;
        }
    }

    #region singleton

    static ArrowMGR _instance;

    public static ArrowMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(ArrowMGR));
                if (previous)
                {
                    _instance = (ArrowMGR)previous;
                }
                else
                {
                    var go = new GameObject("ArrowMGR");
                    _instance = go.AddComponent<ArrowMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}