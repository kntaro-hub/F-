using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フィールドの情報を格納する
/// </summary>
public class FieldInformation : MonoBehaviour
{
    public enum FieldType
    {
        none = 0,   // 何もない
        wall,       // 壁
        max
    }

    // マス一個分の情報
    public struct GridInfo
    {
        public GridBase     onFieldObj;
        public Vector2Int   gridNum;
        public FieldType    type;

        public GridBase OnFieldObj
        {
            get { return onFieldObj; }
            set { onFieldObj = value; }
        }

        public Vector2Int GridNum
        {
            get { return gridNum; }
            set { gridNum = value; }
        }

        public FieldType Type
        {
            get { return type; }
            set { type = value; }
        }
    }

    // グリッドサイズ
    public const float GridSize = 1.0f;
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class MapData:MonoBehaviour
{
    int width; // 幅
    int height; // 高さ
    int outOfRange = -1; // 領域外を指定した時の値
    int[] mapValue = null; // マップデータ

    private void Awake()
    {
        this.Create(10, 10);
    }

    // 幅
    public int Width
    {
        get { return width; }
    }
    // 高さ
    public int Height
    {
        get { return height; }
    }

    // 作成
    public void Create(int width, int height)
    {
        this.width = width;
        this.height = height;
        mapValue = new int[Width * Height];
    }

    /// 座標をインデックスに変換する
    public int ToIdx(int x, int y)
    {
        return x + (y * Width);
    }

    // 領域外かどうかチェックする
    public bool IsOutOfRange(int x, int y)
    {
        if (x < 0 || x >= Width) { return true; }
        if (y < 0 || y >= Height) { return true; }

        // 領域内
        return false;
    }

    // 値の取得
    // 指定の座標の値（領域外を指定したら_outOfRangeを返す）
    public int Get(int x, int y)
    {
        if (IsOutOfRange(x, y))
        {
            return outOfRange;
        }

        return mapValue[y * Width + x];
    }

    // 値の設定
    public void Set(int x, int y, int v)
    {
        if (IsOutOfRange(x, y))
        {
            // 領域外を指定した
            return;
        }

        mapValue[y * Width + x] = v;
    }
    #region singleton

    static MapData _instance;

    public static MapData instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(MapData));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use MapData in the scene hierarchy.");
                    _instance = (MapData)previous;
                }
                else
                {
                    var go = new GameObject("MapData");
                    _instance = go.AddComponent<MapData>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion

}
