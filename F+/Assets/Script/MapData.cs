using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapData : MonoBehaviour
{
    // =--------- 構造体定義 ---------= //
    private struct SaveData
    {
        public FieldInformation.GridInfo[,] infos;
    }

    // =--------- 変数宣言 ---------= //
    // グリッド座標→ワールド座標変換時のズレ
    private float initPosY = 0.0f;

    // マップデータ保存用構造体
    private SaveData saveData = new SaveData();

    // 生成したマップオブジェクトリスト
    private List<GameObject> mapObjects = new List<GameObject>();

    // マップデータ
    private FieldInformation.GridInfo[,] gridInfos;
    public FieldInformation.GridInfo[,] GridInfos
    {
        get { return gridInfos; }
    }

    int width; // 幅
    int height; // 高さ
    int outOfRange = -1; // 領域外を指定した時の値
    int[] mapValue = null; // マップデータ

    // =--------- プレハブ ---------= //
    // 壁
    [SerializeField] private GameObject wallPrefab;

    private void Awake()
    {
        this.Create(20, 20);
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
        gridInfos = new FieldInformation.GridInfo[Width, Height];

        this.CreateStage();
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


    // =---------  ---------= //

    private void SaveField()
    {
        StreamWriter writer;

        writer = new StreamWriter(Application.dataPath + "/MapData.json", false);

        saveData.infos = new FieldInformation.GridInfo[width, height];
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                saveData.infos[i, j] = gridInfos[i, j];
                string jsonstr = JsonUtility.ToJson(saveData.infos[i, j]);
                jsonstr = jsonstr + "\n";
                writer.Write(jsonstr);
                writer.Flush();
            }
        }


        writer.Close();
    }

    private void LoadField()
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader(Application.dataPath + "/MapData.json");
        datastr = reader.ReadToEnd();
        reader.Close();

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                gridInfos[i, j] = JsonUtility.FromJson<FieldInformation.GridInfo>(datastr);
            }
        }
        foreach (var itr in mapObjects)
        {
            Destroy(itr);
        }
        mapObjects.Clear();
    }

    /// <summary>
    /// 配列からステージを生成する
    /// </summary>
    private void CreateStage()
    {
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                gridInfos[i, j].GridNum = new Vector2Int(i, j);
                gridInfos[i, j].Type = FieldInformation.FieldType.none;
                gridInfos[i, j].OnFieldObj = null;

                if(i - 1 < 0)
                {
                    mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(-1, j)), Quaternion.identity));
                }
                if (j - 1 < 0)
                {
                    mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(i, -1)), Quaternion.identity));
                }
                if (i + 1 == width)
                {
                    mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(width + 1, j)), Quaternion.identity));
                }
                if (j + 1 == height)
                {
                    mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(i, height + 1)), Quaternion.identity));
                }
            }
        }

        gridInfos[1, 3].Type = FieldInformation.FieldType.wall;
        mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(1, 3)), Quaternion.identity));
        gridInfos[1, 4].Type = FieldInformation.FieldType.wall;
        mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(1, 4)), Quaternion.identity));
        gridInfos[1, 5].Type = FieldInformation.FieldType.wall;
        mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(1, 5)), Quaternion.identity));
        gridInfos[1, 2].Type = FieldInformation.FieldType.wall;
        mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(1, 2)), Quaternion.identity));
        gridInfos[1, 6].Type = FieldInformation.FieldType.wall;
        mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(1, 6)), Quaternion.identity));
        gridInfos[1, 7].Type = FieldInformation.FieldType.wall;
        mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(1, 7)), Quaternion.identity));
        gridInfos[1, 8].Type = FieldInformation.FieldType.wall;
        mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(1, 8)), Quaternion.identity));
        gridInfos[1, 9].Type = FieldInformation.FieldType.wall;
        mapObjects.Add(Instantiate(wallPrefab, FieldMGR.GridToWorld(new Point(1, 9)), Quaternion.identity));
    }

    public FieldInformation.GridInfo GetGrid(Point point)
    {
        if (point.x < 0 || point.x >= Width || point.y < 0 || point.y >= Height)
        {
            AdDebug.Log("範囲外参照");
        }

        return gridInfos[point.x, point.y];
    }

    public void SetInitY(float posY)
    {
        initPosY = posY;
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