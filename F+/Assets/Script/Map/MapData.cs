using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapData : MonoBehaviour
{
    // =--------- 列挙体定義 ---------= //
    public enum MapChipType
    {
        none = 0,   // 何もない
        wall,       // 壁
        room,
        goal,       // 階段
        max
    }

    // =--------- 定数定義 ---------= //
    public const float GridSize = 1.0f; // グリッドサイズ

    // =--------- 変数宣言 ---------= //
    // グリッド座標→ワールド座標変換時のズレ
    private float initPosY = 0.0f;

    // 生成したマップオブジェクトリスト
    private List<GameObject> mapObjects = new List<GameObject>();

    int width; // 幅
    int height; // 高さ
    int outOfRange = -1; // 領域外を指定した時の値

    struct MapValue
    {
        public int value;       // マップチップ
        public int roomNumber;  // 部屋番号
    }
    MapValue[,] mapValue = null; // マップデータ

    private List<EnemyBase> enemies = new List<EnemyBase>();

    private Goal goal = null;
    public Goal GetGoal
    {
        get { return goal; }
    }

    // =--------- プレハブ ---------= //
    // 壁
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private Goal goalPrefab;

    // 敵（本来は敵テーブルから）
    [SerializeField] private EnemyBase enemyPrefab;

    private void Awake()
    {
        this.Create(50  , 50);
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
        mapValue = new MapValue[Width , Height];
    }

    // 座標をインデックスに変換する
    public int ToIdx(int x, int y)
    {
        return x + (y * Width);
    }

    // 領域外かどうかチェックする
    public bool IsOutOfRange(int x, int y)
    {
        if (x < 0 || x >= Width) { return true; }
        if (y < 0 || y >= Height) { return true; }

        return false;
    }

    // 値の取得
    // 指定の座標の値（領域外を指定したら_outOfRangeを返す）
    public int GetValue(int x, int y)
    {
        return mapValue[x , y].value;
    }
    public int GetValue(Point point)
    {
        return mapValue[point.x , point.y].value;
    }

    // 値の設定
    public void SetValue(int x, int y, int v)
    {
        mapValue[x , y].value = v;
    }

    public void SetRoomNum(int x, int y, int v)
    {
        mapValue[x, y].roomNumber = v;
    }

    public int GetRoomNum(int x, int y)
    {
        return mapValue[x, y].roomNumber;
    }
    public int GetRoomNum(Point point)
    {
        return mapValue[point.x, point.y].roomNumber;
    }


    public void Fill(int val)
    {
        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                this.SetValue(i, j, val);
              
            }
        }
    }

    /// <summary>
    /// 矩形領域を指定の値で埋める
    /// </summary>
    /// <param name="x">矩形の左上(X座標)</param>
    /// <param name="y">矩形の左上(Y座標)</param>
    /// <param name="w">矩形の幅</param>
    /// <param name="h">矩形の高さ</param>
    /// <param name="val">埋める値</param>
    public void FillRect(int x, int y, int w, int h, int val)
    {
        for (int j = 0; j < h; j++)
        {
            for (int i = 0; i < w; i++)
            {
                int px = x + i;
                int py = y + j;
                SetValue(px, py, val);
            }
        }
    }

    /// <summary>
    /// 矩形領域を指定の値で埋める（4点指定)
    /// </summary>
    /// <param name="left">左</param>
    /// <param name="top">上</param>
    /// <param name="right">右</param>
    /// <param name="bottom">下</param>
    /// <param name="val">埋める値</param>
    public void FillRectLTRB(int left, int top, int right, int bottom, int val)
    {
        FillRect(left, top, right - left, bottom - top, val);
    }

    // =---------  ---------= //

    //private void SaveField()
    //{
    //    StreamWriter writer;

    //    writer = new StreamWriter(Application.dataPath + "/MapData.json", false);

    //    saveData.infos = new GridInfo[width, height];
    //    //for (int i = 0; i < width; ++i)
    //    //{
    //    //    for (int j = 0; j < height; ++j)
    //    //    {
    //    //        saveData.infos[i, j] = gridInfos[i, j];
    //    //        string jsonstr = JsonUtility.ToJson(saveData.infos[i, j]);
    //    //        jsonstr = jsonstr + "\n";
    //    //        writer.Write(jsonstr);
    //    //        writer.Flush();
    //    //    }
    //    //}


    //    writer.Close();
    //}

    //private void LoadField()
    //{
    //    string datastr = "";
    //    StreamReader reader;
    //    reader = new StreamReader(Application.dataPath + "/MapData.json");
    //    datastr = reader.ReadToEnd();
    //    reader.Close();

    //    //for (int i = 0; i < width; ++i)
    //    //{
    //    //    for (int j = 0; j < height; ++j)
    //    //    {
    //    //        gridInfos[i, j] = JsonUtility.FromJson<GridInfo>(datastr);
    //    //    }
    //    //}
    //    foreach (var itr in mapObjects)
    //    {
    //        Destroy(itr);
    //    }
    //    mapObjects.Clear();
    //}    //private void SaveField()
    //{
    //    StreamWriter writer;

    //    writer = new StreamWriter(Application.dataPath + "/MapData.json", false);

    //    saveData.infos = new GridInfo[width, height];
    //    //for (int i = 0; i < width; ++i)
    //    //{
    //    //    for (int j = 0; j < height; ++j)
    //    //    {
    //    //        saveData.infos[i, j] = gridInfos[i, j];
    //    //        string jsonstr = JsonUtility.ToJson(saveData.infos[i, j]);
    //    //        jsonstr = jsonstr + "\n";
    //    //        writer.Write(jsonstr);
    //    //        writer.Flush();
    //    //    }
    //    //}
    //
    //
    //    writer.Close();
    //}
    //
    //private void LoadField()
    //{
    //    string datastr = "";
    //    StreamReader reader;
    //    reader = new StreamReader(Application.dataPath + "/MapData.json");
    //    datastr = reader.ReadToEnd();
    //    reader.Close();

    //    //for (int i = 0; i < width; ++i)
    //    //{
    //    //    for (int j = 0; j < height; ++j)
    //    //    {
    //    //        gridInfos[i, j] = JsonUtility.FromJson<GridInfo>(datastr);
    //    //    }
    //    //}
    //    foreach (var itr in mapObjects)
    //    {
    //        Destroy(itr);
    //    }
    //    mapObjects.Clear();
    //}

    public void CreateWall(int x, int y)
    {
        mapValue[x, y].value = (int)MapData.MapChipType.wall;
        GameObject wall = Instantiate(wallPrefab, GridToWorld(new Point(x, y)), Quaternion.identity);
        wall.transform.parent = this.transform;
        mapObjects.Add(wall);
    }
    
    /// <summary>
    /// 階段生成
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CreateGoal(int x, int y)
    {
        mapValue[x, y].value = (int)MapData.MapChipType.goal;
        goal = Instantiate(goalPrefab, GridToWorld(new Point(x, y)), Quaternion.identity);
        goal.transform.parent = this.transform;
        mapObjects.Add(goal.gameObject);
        goal.GoalPoint = new Point(x, y);
    }

    /// <summary>
    /// 敵生成
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CreateEnemy(int x, int y)
    {
        EnemyBase enemy = Instantiate(enemyPrefab, GridToWorld(new Point(x, y)), Quaternion.identity);
        enemy.transform.parent = this.transform;
        enemy.GetComponent<AStarSys>().SetStartPoint(new Point(x, y));
        enemy.status.gridPos = new Point(x, y);
        SequenceMGR.instance.Enemies.Add(enemy);
    }

    /// <summary>
    /// 渡したグリッド座標とゴールの座標を比較
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool CheckGoal(Point point)
    {
        if (goal.GoalPoint == point)
        {
            return true;
        }
        return false;
    }

    public int GetGrid(Point point)
    {
        if (point.x < 0 || point.x >= Width || point.y < 0 || point.y >= Height)
        {
            AdDebug.Log("範囲外参照");
        }

        return mapValue[point.x, point.y].value;
    }

    public void SetInitY(float posY)
    {
        initPosY = posY;
    }

    public Division.Div_Room GetRoom(Point point)
    {
        foreach (var itr in MapGenerator.instance.DivList)
        {
            if (itr.Room.Left   <= point.x &&
               itr.Room.Right   > point.x &&
               itr.Room.Top     <= point.y &&
               itr.Room.Bottom  > point.y)
            {
                return itr.Room;
            }
        }
        return null;
    }

    // =--------- // =--------- static ---------= // ---------= //
    public static Vector3 GridToWorld(Point grid)
    {
        Vector3 world = new Vector3(
            (float)(grid.x * GridSize),
            0.0f,
            (float)(grid.y * GridSize));

        return world;
    }

    public static Point WorldToGrid(Vector3 world)
    {
        Point grid = new Point(
            (int)(world.x / GridSize),
            (int)(world.z / GridSize));

        return grid;
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