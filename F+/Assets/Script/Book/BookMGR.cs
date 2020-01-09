using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BookType
{
    Explosion,
    Hunger,
    max
}


public class BookMGR : MonoBehaviour
{
    [SerializeField]
    private BookBase[] bookPrefabs = new BookBase[(int)BookType.max];

    // 生成した魔法リスト
    private List<BookBase> bookList = new List<BookBase>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateBook(BookType bookType, int itemID)
    {
        // 魔法生成
        BookBase book = Instantiate(bookPrefabs[(int)bookType], this.transform);

        // ID割り振り
        book.SetID(this.SetUniqueID());

        // アイテムID設定（これをもとにステータスを変動させる）
        book.SetItemID(itemID);

        // 魔法効果発動
        book.ActivateBook();

        // 魔法リストに追加
        bookList.Add(book);
    }

    public void Destroy(int id)
    {
        for (int i = bookList.Count - 1; i >= 0; i--)
        {// 逆順ループ
            // ID検索してヒットした敵を消す リストからも
            if (bookList[i].GetID() == id)
            {
                Destroy(bookList[i].gameObject);
                bookList.RemoveAt(i);
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
            foreach (var itr in bookList)
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

    static BookMGR _instance;

    public static BookMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(BookMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use BookMGR in the scene hierarchy.");
                    _instance = (BookMGR)previous;
                }
                else
                {
                    var go = new GameObject("BookMGR");
                    _instance = go.AddComponent<BookMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
