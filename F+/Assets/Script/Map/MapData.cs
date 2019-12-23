﻿using System.Collections;
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

    // マップ上のオブジェクト（これはオブジェクトが自分で設定、自分で消去する）
    public enum MapObjType
    {
        none = 0,
        player,
        enemy,
        item,
        goal,
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
    [SerializeField] private MapChip_Wall wallPrefab;
    [SerializeField] private MapChip_Goal goalPrefab;

    // 敵（本来は敵テーブルから）
    [SerializeField] private EnemyBase enemyPrefab;

    private void Awake()
    {
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
        mapValue[x, y].mapChip = Instantiate(wallPrefab, GridToWorld(new Point(x, y)), Quaternion.identity);
        mapValue[x, y].mapChip.transform.parent = this.transform;
        mapObjects.Add(mapValue[x, y].mapChip);
    }
    
    /// <summary>
    /// 階段生成
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CreateGoal(int x, int y)
    {
        mapValue[x, y].mapChip = Instantiate(goalPrefab, GridToWorld(new Point(x, y)), Quaternion.identity);
        UI_MGR.instance.Ui_Map.CreateMapGoal(new Point(x, y));
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
        enemy.status.point = new Point(x, y);
        SequenceMGR.instance.Enemies.Add(enemy);

        UI_MGR.instance.Ui_Map.CreateMapEnemy(enemy.status.point);
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