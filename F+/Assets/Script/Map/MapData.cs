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
        room,       // 部屋
        roomAround, // 部屋の周り
        goal,       // 階段
        aisleGate,  // 部屋のつなぎ目
        max
    }

    // マップ上のオブジェクト（これはオブジェクトが自分で設定、自分で消去する）
    public enum MapObjType
    {
        none = 0,
        player,
        enemy,
        item,
        trap,
        max
    }

    public struct ObjectOnTheMap
    {
        public MapObjType objType;
        public int id;
    }

    // =--------- 定数定義 ---------= //
    public const float GridSize = 1.0f; // グリッドサイズ
    public const int None_Id = 9999;

    public static readonly Point[] DirectPoints =
                            new Point[(int)(Actor.Direct.max + 1)] {
                           new Point(-1, -1) , new Point(0, -1), new Point(1, -1),
                           new Point(-1, 0) , new Point(0, 0), new Point(1, 0),
                           new Point(-1, 1) , new Point(0, 1), new Point(1, 1)};

    // =--------- 変数宣言 ---------= //
    // グリッド座標→ワールド座標変換時のズレ
    private float initPosY = 0.0f;

    // 生成したマップオブジェクトリスト
    private List<MapChipBase> mapObjects = new List<MapChipBase>();

    int width; // 幅
    int height; // 高さ
    int outOfRange = -1; // 領域外を指定した時の値

    struct MapValue
    {
        public MapChipBase mapChip;           // マップチップ
        public MapChipType mapChipType;     // マップチップの種類
        public int roomNumber;              // 部屋番号
        public ObjectOnTheMap mapObject;    // マップ上のオブジェクトタイプ
    }
    MapValue[,] mapValue = null; // マップデータ

    private List<EnemyBase> enemies = new List<EnemyBase>();

    // =--------- プレハブ ---------= //
    // 壁
    [SerializeField] private MapChip_Wall[] floorPrefab = new MapChip_Wall[5];
    [SerializeField] private MapChip_Wall[] wallPrefab = new MapChip_Wall[3];
    [SerializeField] private MapObject[] mapDecorations = new MapObject[6];
    [SerializeField] private MapChip_Goal goalPrefab;

    // 敵（本来は敵テーブルから）
    [SerializeField] private EnemyBase enemyPrefab;

    private void Awake()
    {
        // 方向の情報をセット
        DirectPoints[(int)Actor.Direct.right] = new Point(1, 0);
        DirectPoints[(int)Actor.Direct.left] = new Point(-1, 0);
        DirectPoints[(int)Actor.Direct.forward] = new Point(0, 1);
        DirectPoints[(int)Actor.Direct.back] = new Point(0, -1);
        DirectPoints[(int)Actor.Direct.right_forward] = new Point(1, 1);
        DirectPoints[(int)Actor.Direct.left_forward] = new Point(-1, 1);
        DirectPoints[(int)Actor.Direct.right_back] = new Point(1, -1);
        DirectPoints[(int)Actor.Direct.left_back] = new Point(-1, -1);
        DirectPoints[(int)Actor.Direct.max] = new Point(0, 0);

        this.Create(50  , 50);
        
        for(int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                mapValue[i, j].mapObject.objType = MapObjType.none;
            }
        }
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
    public MapChipType GetMapChipType(int x, int y)
    {
        return mapValue[x , y].mapChipType;
    }
    public MapChipType GetMapChipType(Point point)
    {
        return mapValue[point.x , point.y].mapChipType;
    }

    // 値の設定
    public void SetMapChipType(int x, int y, MapChipType type)
    {
        mapValue[x , y].mapChipType = type;
    }
    public void SetMapChipType(Point point, MapChipType type)
    {
        mapValue[point.x, point.y].mapChipType = type;
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
    public ObjectOnTheMap GetMapObject(int x, int y)
    {
        return mapValue[x, y].mapObject;
    }
    public ObjectOnTheMap GetMapObject(Point point)
    {
        return mapValue[point.x, point.y].mapObject;
    }
    public void SetMapObject(Point point, MapObjType obj, int id)
    {
        mapValue[point.x, point.y].mapObject.objType = obj;
        mapValue[point.x, point.y].mapObject.id = id;
    }
    public void SetMapObject(int x, int y, MapObjType obj, int id)
    {
        mapValue[x, y].mapObject.objType = obj;
        mapValue[x, y].mapObject.id = id;
    }
    public void ResetMapObject(Point point)
    {
        mapValue[point.x, point.y].mapObject.objType = MapObjType.none;
        mapValue[point.x, point.y].mapObject.id = None_Id;
    }
    public void ResetMapObject(int x, int y)
    {
        mapValue[x, y].mapObject.objType = MapObjType.none;
        mapValue[x, y].mapObject.id = None_Id;
    }

    public void SetMapChip(Point setPoint, MapChipBase mapChip)
    {
        mapValue[setPoint.x, setPoint.y].mapChip = mapChip;
    }
	public void ActiveMapChip(int x, int y, Actor actor)
    {
        if (mapValue[x, y].mapChip != null)
        {
            mapValue[x, y].mapChip.ActiveMapChip(actor);
        }
    }
    public void ActiveMapChip(Point point, Actor actor)
    {
        if (mapValue[point.x, point.y].mapChip != null)
        {
            mapValue[point.x, point.y].mapChip.ActiveMapChip(actor);
        }
    }

    public void Fill(int val)
    {
        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                this.SetMapChipType(i, j, (MapChipType)val);

            }
        }
    }

    public static Point GetPointFromAround(Point Center, MapObjType objType)
    {
        Point ret;
        ret = new Point(1, 0);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        ret = new Point(-1, 0);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        ret = new Point(0, 1);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        ret = new Point(0, -1);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        ret = new Point(1, 1);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        ret = new Point(1, -1);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        ret = new Point(-1, 1);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        ret = new Point(-1, -1);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;

        return new Point();
    }
    public static bool IsPointFromAround(Point Center, MapObjType objType)
    {
        Point ret;
        ret = new Point(Center.x + 1, Center.y);
        if (MapData.instance.GetMapObject(ret).objType == objType) return true;
        ret = new Point(Center.x - 1, Center.y);
        if (MapData.instance.GetMapObject(ret).objType == objType) return true;
        ret = new Point(Center.x, Center.y + 1);
        if (MapData.instance.GetMapObject(ret).objType == objType) return true;
        ret = new Point(Center.x, Center.y - 1);
        if (MapData.instance.GetMapObject(ret).objType == objType) return true;
        ret = new Point(Center.x + 1, Center.y + 1);
        if (MapData.instance.GetMapObject(ret).objType == objType) return true;
        ret = new Point(Center.x + 1, Center.y - 1);
        if (MapData.instance.GetMapObject(ret).objType == objType) return true;
        ret = new Point(Center.x - 1, Center.y + 1);
        if (MapData.instance.GetMapObject(ret).objType == objType) return true;
        ret = new Point(Center.x - 1, Center.y - 1);
        if (MapData.instance.GetMapObject(ret).objType == objType) return true;

        return false;
    }

    public static Point GetPointFromUDRL(Point Center, MapObjType objType)
    {
        Point ret;
        ret = new Point(1, 0);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        ret = new Point(-1, 0);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        ret = new Point(0, 1);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        ret = new Point(0, -1);
        if (MapData.instance.GetMapObject(Center + ret).objType == objType) return ret;
        return new Point();
    }
    public static Point GetPointFromUDRL_Chip(Point Center, MapChipType chipType)
    {
        Point ret;
        ret = new Point(1, 0);
        if (MapData.instance.GetMapChipType(Center + ret) == chipType) return ret;
        ret = new Point(-1, 0);
        if (MapData.instance.GetMapChipType(Center + ret) == chipType) return ret;
        ret = new Point(0, 1);
        if (MapData.instance.GetMapChipType(Center + ret) == chipType) return ret;
        ret = new Point(0, -1);
        if (MapData.instance.GetMapChipType(Center + ret) == chipType) return ret;
        return new Point();
    }

    public static Point GetRandomPointFromAround(Point Center)
    {
        switch(Random.Range(1, (int)Actor.Direct.max - 1))
        {
            case (int)Actor.Direct.right:           return (Center + new Point( 1, 0));
            case (int)Actor.Direct.left:            return (Center + new Point(-1, 0));
            case (int)Actor.Direct.forward:         return (Center + new Point( 0, 1));
            case (int)Actor.Direct.back:            return (Center + new Point( 0,-1));
            case (int)Actor.Direct.right_forward:   return (Center + new Point( 1, 1));
            case (int)Actor.Direct.left_forward:    return (Center + new Point(-1, 1));
            case (int)Actor.Direct.right_back:      return (Center + new Point( 1,-1));
            case (int)Actor.Direct.left_back:       return (Center + new Point(-1,-1));
            default:return new Point();
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
               	SetMapChipType(px, py, (MapChipType)val);
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

    public void CreateWall(int x, int y)
    {

        mapValue[x, y].mapChip = Instantiate(wallPrefab[Random.Range(0, 3)], GridToWorld(new Point(x, y)), Quaternion.identity);
        mapValue[x, y].mapChip.transform.parent = this.transform;
        mapObjects.Add(mapValue[x, y].mapChip);
        
        if(Percent.Per(10))
        {
            MapObject deco = Instantiate(
                mapDecorations[Random.Range(0, 6)], 
                new Vector3(mapValue[x, y].mapChip.transform.position.x, 
                            mapValue[x, y].mapChip.transform.position.y + (GridSize * 0.5f), 
                            mapValue[x, y].mapChip.transform.position.z), 
                Quaternion.Euler(0.0f,Random.Range(0.0f,360.0f),0.0f), 
                mapValue[x, y].mapChip.transform);

            float scale = Random.Range(1.0f, 5.0f);
            deco.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void CreateStageAround()
    {
        const int aroundSize = 20;
        for(int i = 0; i  < width + aroundSize; ++i)
        {
            for (int j = 0; j < height + aroundSize; ++j)
            {
                if(i < (aroundSize / 2) || j < (aroundSize / 2))
                {
                    Instantiate(wallPrefab[Random.Range(0, 3)], GridToWorld(new Point(i - (aroundSize / 2), j - (aroundSize / 2))), Quaternion.identity);
                }
                else if (i >= width + (aroundSize / 2) || j >= height + (aroundSize / 2))
                {
                    Instantiate(wallPrefab[Random.Range(0, 3)], GridToWorld(new Point(i - (aroundSize / 2), j - (aroundSize / 2))), Quaternion.identity);
                }
            }
        }
    }

    public void CreateFloor(int x, int y)
    {
        Vector3 position = GridToWorld(new Point(x, y), -GridSize);

        MapChip_Wall floor = Instantiate(floorPrefab[Random.Range(0, 5)], position, Quaternion.identity);

        floor.transform.parent = this.transform;
    }

    /// <summary>
    /// 階段生成
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CreateGoal(int x, int y)
    {
        mapValue[x, y].mapChip = Instantiate(goalPrefab);
        mapValue[x, y].mapChip.transform.position = GridToWorld(new Point(x, y));
        UI_MGR.instance.Ui_Map.CreateMapGoal(new Point(x, y));
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
    public static Vector3 GridToWorld(Point grid, float posY)
    {
        Vector3 world = new Vector3(
            (float)(grid.x * GridSize),
            posY,
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