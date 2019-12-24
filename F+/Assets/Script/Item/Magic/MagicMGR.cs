using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MagicType
{
    Fire,
    Tornado,
    max
}

public class MagicMGR : MonoBehaviour
{
    [SerializeField]
    private MagicBase[] magicPrefabs = new MagicBase[(int)MagicType.max];

    // 生成した魔法リスト
    private List<MagicBase> magicList = new List<MagicBase>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateMagic(MagicType magicType, int itemID)
    {
        // 杖の残り回数を1減らす

        // 魔法生成
        MagicBase magic = Instantiate(magicPrefabs[(int)magicType], this.transform);

        // ID割り振り
        magic.SetID(this.SetUniqueID());

        // アイテムID設定（これをもとにステータスを変動させる）
        magic.SetItemID(itemID);

        // 魔法効果発動
        magic.ActivateMagic();

        // 魔法リストに追加
        magicList.Add(magic);
    }

    public void Destroy(int id)
    {
        for (int i = magicList.Count - 1; i >= 0; i--)
        {// 逆順ループ
            // ID検索してヒットした敵を消す リストからも
            if (magicList[i].GetID() == id)
            {
                Destroy(magicList[i].gameObject);
                magicList.RemoveAt(i);
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
            foreach (var itr in magicList)
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

    static MagicMGR _instance;

    public static MagicMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(MagicMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use MagicMGR in the scene hierarchy.");
                    _instance = (MagicMGR)previous;
                }
                else
                {
                    var go = new GameObject("MagicMGR");
                    _instance = go.AddComponent<MagicMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion

}
