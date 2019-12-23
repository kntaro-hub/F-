using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

/// <summary>
/// ダンジョンマップデータを自動生成する
/// ここで生成したデータをもとにマップを作成する
/// 分割法 と 穴掘り法
/// </summary>
public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// 区画と部屋の余白サイズ
    /// </summary>
    const int Outer_Mergin = 1;
    /// <summary>
    /// 部屋配置の余白サイズ
    /// </summary>
    const int Room_Mergin = 1;
    /// <summary>
    /// 最小の部屋サイズ
    /// </summary>
    const int Room_MinSize = 6;
    /// <summary>
    /// 最大の部屋サイズ
    /// </summary>
    const int Room_Max = 12;

    /// <summary>
    /// 区画リスト
    /// </summary>
    private List<Division> divList = null;
    public List<Division> DivList
    {
        get { return divList; }
    }

    /// チップ上のX座標を取得する.
    float GetChipX(int i)
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        return min.x;
    }

    /// チップ上のy座標を取得する.
    float GetChipY(int j)
    {
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        return max.y;
    }

    void Start()
    {
        MapData mapData = MapData.instance;

        // 1. 初期化
        // 区画リスト作成
        divList = new List<Division>();

        // 2. すべてを壁にする
        mapData.Fill((int)MapData.MapChipType.wall);

        // 3. 最初の区画を作る
        CreateDivision(0, 0, mapData.Width - 1, mapData.Height - 1);

        // 4. 区画を分割する
        // 垂直 or 水平分割フラグの決定
        bool bVertical = (Random.Range(0, 2) == 0);
        SplitDivison(bVertical);

        // 5. 区画に部屋を作る
        CreateRoom();

        // 6. 部屋同士をつなぐ
        ConnectRooms();

        // 7. 部屋のどこかにプレイヤーを配置する
        this.PlayerSet();

        // 8. ゴール設置
        this.GoalSet();

        // 9. 敵設置
        this.EnemySet();

        // 10. 壁を配置
        this.FillWall();

        // 11. 罠を配置
        for(int i = 0; i < 10;++i)
        TrapMGR.instance.CreateTrap(TrapBase.TrapType.Warp);
    }    

    private void FillWall()
    {
        MapData mapData = MapData.instance;
        for (int j = 0; j < mapData.Height; j++)
        {
            for (int i = 0; i < mapData.Width; i++)
            {
                if (mapData.GetMapChipType(i, j) == MapData.MapChipType.wall)
                {
                    // 壁生成
                    float x = GetChipX(i);
                    float y = GetChipY(j);

                    mapData.CreateWall(i, j);
                }
            }
        }
    }

    public Point RandomPointInRoom()
    {
        // 生成した部屋のリストからランダムな部屋を指定
        Division.Div_Room room = divList[Random.Range(0, divList.Count - 1)].Room;

        // 指定した部屋のランダムな位置を算出
        int X = Random.Range(room.Left + 1, room.Right);
        int Y = Random.Range(room.Bottom - 1, room.Top);

        return new Point(X, Y);
    }

    /// <summary>
    /// プレイヤーをランダムな部屋のどこかに生成する
    /// </summary>
    private void PlayerSet()
    {
        Point point = this.RandomPointInRoom();

        // プレイヤーのグリッド座標を更新
        SequenceMGR.instance.Player.status.point = point;
        SequenceMGR.instance.Player.transform.position = MapData.GridToWorld(point);
    }
    private void GoalSet()
    {
        Point point = this.RandomPointInRoom();

        // ゴールのマスを設定
       	MapData.instance.SetMapChipType(point.x, point.y, MapData.MapChipType.goal);
        MapData.instance.CreateGoal(point.x, point.y);
    }

    /// <summary>
    /// 現在の階層の敵テーブルからランダムで敵を生成する（予定）
    /// </summary>
    private void EnemySet()
    {
        // 生成した部屋のリストからランダムな部屋を指定
        Division.Div_Room room = divList[Random.Range(0, divList.Count - 1)].Room;

        // 指定した部屋のランダムな位置を算出
        int X = Random.Range(room.Left + 1, room.Right);
        int Y = Random.Range(room.Bottom - 1, room.Top);

        MapData.instance.CreateEnemy(X, Y);
    }

    /// <summary>
    /// 最初の区画を作る
    /// </summary>
    /// <param name="left">左</param>
    /// <param name="top">上</param>
    /// <param name="right">右</param>
    /// <param name="bottom">下</param>
    void CreateDivision(int left, int top, int right, int bottom)
    {
        Division div = new Division();
        div.Outer.Set(left, top, right, bottom);
        divList.Add(div);
    }

    /// <summary>
    /// 区画を分割する
    /// </summary>
    /// <param name="bVertical">垂直分割するかどうか</param>
    void SplitDivison(bool bVertical)
    {
        // 末尾の要素を取り出し
        Division parent = divList[divList.Count - 1];
        divList.Remove(parent);

        // 子となる区画を生成
        Division child = new Division();

        if (bVertical)
        {
            // =--------- 縦方向に分割する ---------= //
            if (CheckDivisionSize(parent.Outer.Height) == false)
            {
                // 縦の高さが足りない
                // 親区画を戻しておしまい
                divList.Add(parent);
                return;
            }

            // 分割ポイントを求める
            int a = parent.Outer.Top    + (Room_MinSize + Outer_Mergin);
            int b = parent.Outer.Bottom - (Room_MinSize + Outer_Mergin);
            // AB間の距離を求める
            int ab = b - a;
            // 最大の部屋サイズを超えないようにする
            ab = Mathf.Min(ab, Room_Max);

            // 分割点を決める
            int p = a + Random.Range(0, ab + 1);

            // 子区画に情報を設定
            child.Outer.Set(
                parent.Outer.Left, p, parent.Outer.Right, parent.Outer.Bottom);

            // 親の下側をp地点まで縮める
            parent.Outer.Bottom = child.Outer.Top;
        }
        else
        {
            // =--------- 横方向に分割する ---------= //
            if (CheckDivisionSize(parent.Outer.Width) == false)
            {
                // 横幅が足りない
                // 親区画を戻しておしまい
                divList.Add(parent);
                return;
            }

            // 分割ポイントを求める
            int a = parent.Outer.Left  + (Room_MinSize + Outer_Mergin);
            int b = parent.Outer.Right - (Room_MinSize + Outer_Mergin);
            // AB間の距離を求める
            int ab = b - a;
            // 最大の部屋サイズを超えないようにする
            ab = Mathf.Min(ab, Room_Max);

            // 分割点を求める
            int p = a + Random.Range(0, ab + 1);

            // 子区画に情報を設定
            child.Outer.Set(
                p, parent.Outer.Top, parent.Outer.Right, parent.Outer.Bottom);

            // 親の右側をp地点まで縮める
            parent.Outer.Right = child.Outer.Left;
        }

        // 次に分割する区画をランダムで決める
        if (Random.Range(0, 2) == 0)
        {
            // 子を分割する
            divList.Add(parent);
            divList.Add(child);
        }
        else
        {
            // 親を分割する
            divList.Add(child);
            divList.Add(parent);
        }

        // 分割処理を再帰呼び出し (分割方向は縦横交互にする)
        SplitDivison(!bVertical);
    }

    /// <summary>
    /// 指定のサイズを持つ区画を分割できるかどうか
    /// </summary>
    /// <param name="size">チェックする区画のサイズ</param>
    /// <returns>分割できればtrue</returns>
    bool CheckDivisionSize(int size)
    {
        // (最小の部屋サイズ + 余白)
        // 2分割なので x2 する
        // +1 して連絡通路用のサイズも残す
        int min = (Room_MinSize + Outer_Mergin) * 2 + 1;

        return size >= min;
    }

    /// <summary>
    /// 区画に部屋を作る
    /// </summary>
    void CreateRoom()
    {
        int SetRoomNumber = 0;
        foreach (Division div in divList)
        {
            // 基準サイズを決める
            int dw = div.Outer.Width - Outer_Mergin;
            int dh = div.Outer.Height - Outer_Mergin;

            // 大きさをランダムに決める
            int sw = Random.Range(Room_MinSize, dw);
            int sh = Random.Range(Room_MinSize, dh);

            // 最大サイズを超えないようにする
            sw = Mathf.Min(sw, Room_Max);
            sh = Mathf.Min(sh, Room_Max);

            // 空きサイズを計算 (区画 - 部屋)
            int rw = (dw - sw);
            int rh = (dh - sh);
            
            // 部屋の左上位置を決める
            int rx = Random.Range(0, rw) + Room_Mergin;
            int ry = Random.Range(0, rh) + Room_Mergin;

            int left   = div.Outer.Left + rx;
            int right  = left + sw;
            int top    = div.Outer.Top + ry;
            int bottom = top + sh;

            // 部屋のサイズを設定
            div.Room.Set(left, top, right, bottom);

            // 部屋番号設定
            div.Room.RoomNumber = SetRoomNumber;

            // 部屋を通路にする
            FillRoom(div.Room);

            // マップデータに登録
            for(int i = 0; i < div.Room.Right - div.Room.Left; ++i)
            {
                for (int j = 0; j < div.Room.Bottom - div.Room.Top; ++j)
                {
                    MapData.instance.SetMapChipType(div.Room.Left + i, div.Room.Top + j, MapData.MapChipType.room);
                    MapData.instance.SetRoomNum(div.Room.Left + i, div.Room.Top + j, SetRoomNumber);
                }
            }
        }
    }

    /// <summary>
    /// 部屋を塗りつぶす
    /// </summary>
    /// <param name="rect">部屋矩形情報</param>
    void FillRoom(Division.Div_Room r)
    {
        MapData.instance.FillRectLTRB(r.Left, r.Top, r.Right, r.Bottom, (int)MapData.MapChipType.none);
    }

    /// <summary>
    /// 部屋同士を通路でつなぐ
    /// </summary>
    void ConnectRooms()
    {
        for (int i = 0; i < divList.Count - 1; i++)
        {
            // リストの前後の区画は必ず接続できる
            Division a = divList[i];
            Division b = divList[i + 1];

            // 2つの部屋をつなぐ通路を作成
            CreateRoad(a, b);
        }

        if (divList.Count >= 4)
        {

            for (int i = 0; i < divList.Count - 1; i++)
            {
                divList[i].ConnectNum[(int)ConnectInfo.prev]   = i - 1;
                divList[i].ConnectNum[(int)ConnectInfo.id]     = i;
                divList[i].ConnectNum[(int)ConnectInfo.next]   = i + 1;
            }

            Division[] divisions = this.ConnectRoad();
            // 2つの部屋をつなぐ通路を作成
            CreateRoad(divisions[0], divisions[1]);
        }
    }

    /// <summary>
    /// リスト前後以外のランダムな部屋同士で道をつなげる
    /// </summary>
    /// <returns></returns>
    private Division[] ConnectRoad()
    {
        // ランダムな部屋番号を取得
        int room_cNum = Random.Range(0, divList.Count - 1);

        // 部屋情報をかぶりの内容に2つ取得
        Division[] divisions = new Division[2];
        divisions[0] = divList[room_cNum];
        divisions[1] = divList[this.GetRandomRoom(room_cNum)];
        
        // 道がつながっているか調べる
        if (divisions[0].ConnectNum[(int)ConnectInfo.prev] == divisions[1].ConnectNum[(int)ConnectInfo.id] ||
            divisions[0].ConnectNum[(int)ConnectInfo.next] == divisions[1].ConnectNum[(int)ConnectInfo.id] ||
            divisions[1].ConnectNum[(int)ConnectInfo.prev] == divisions[0].ConnectNum[(int)ConnectInfo.id] ||
            divisions[1].ConnectNum[(int)ConnectInfo.next] == divisions[0].ConnectNum[(int)ConnectInfo.id])
        {// 既に道がつながっていたら再抽選
            return this.ConnectRoad();
        }
        // つながっていなかったら2部屋の情報を返す
        return divisions;
    }

    /// <summary>
    /// 被りの無いように部屋番号を抽選する
    /// </summary>
    /// <param name="roomNum">すでに出た番号。これと被らないように抽選する</param>
    /// <returns>部屋番号</returns>
    private int GetRandomRoom(int roomNum)
    {
        // 抽選
        int num = Random.Range(0, divList.Count - 1);
        if(num == roomNum)
        {// 被っていた場合再抽選
           return this.GetRandomRoom(roomNum);
        }
        // 被っていなかった場合は抽選した番号を返す
        return num;
    }

    /// <summary>
    /// 指定した部屋の間を通路でつなぐ
    /// </summary>
    /// <param name="divA">部屋1</param>
    /// <param name="divB">部屋2</param>
    /// <returns>つなぐことができたらtrue</returns>
    bool CreateRoad(Division divA, Division divB)
    {
        if (divA.Outer.Bottom == divB.Outer.Top || divA.Outer.Top == divB.Outer.Bottom)
        {
            // 上下でつながっている
            // 部屋から伸ばす通路の開始位置を決める
            int x1 = Random.Range(divA.Room.Left, divA.Room.Right);
            int x2 = Random.Range(divB.Room.Left, divB.Room.Right);
            int y  = 0;

            if (divA.Outer.Top > divB.Outer.Top)
            {
                // B - A (Bが上側)
                y = divA.Outer.Top;
                // 通路を作成
                MapData.instance.FillRectLTRB(x1, y + 1, x1 + 1, divA.Room.Top, (int)MapData.MapChipType.none);
                MapData.instance.FillRectLTRB(x2, divB.Room.Bottom, x2 + 1, y, (int)MapData.MapChipType.none);
            }
            else
            {
                // A - B (Aが上側)
                y = divB.Outer.Top;
                // 通路を作成
                MapData.instance.FillRectLTRB(x1, divA.Room.Bottom, x1 + 1, y, (int)MapData.MapChipType.none);
                MapData.instance.FillRectLTRB(x2, y, x2 + 1, divB.Room.Top, (int)MapData.MapChipType.none);
            }

            // 通路同士を接続する
            FillHLine(x1, x2, y);

            // 通路を作れた
            //return true;
        }

        if (divA.Outer.Left == divB.Outer.Right || divA.Outer.Right == divB.Outer.Left)
        {
            // 左右でつながっている
            // 部屋から伸ばす通路の開始位置を決める
            int y1 = Random.Range(divA.Room.Top, divA.Room.Bottom);
            int y2 = Random.Range(divB.Room.Top, divB.Room.Bottom);
            int x  = 0;

            if (divA.Outer.Left > divB.Outer.Left)
            {
                // B - A (Bが左側)
                x = divA.Outer.Left;
                // 通路を作成
                MapData.instance.FillRectLTRB(divB.Room.Right, y2, x, y2 + 1, (int)MapData.MapChipType.none);
                MapData.instance.FillRectLTRB(x + 1, y1, divA.Room.Left, y1 + 1, (int)MapData.MapChipType.none);
            }
            else
            {
                // A - B (Aが左側)
                x = divB.Outer.Left;
                MapData.instance.FillRectLTRB(divA.Room.Right, y1, x, y1 + 1, (int)MapData.MapChipType.none);
                MapData.instance.FillRectLTRB(x, y2, divB.Room.Left, y2 + 1, (int)MapData.MapChipType.none);
            }

            // 通路同士を接続する
            FillVLine(y1, y2, x);

            // 通路を作れた
            //return true;
        }


        // つなげなかった
        return false;
    }

    /// <summary>
    /// 水平方向に線を引く (左と右の位置は自動で反転する)
    /// </summary>
    /// <param name="left">左</param>
    /// <param name="right">右</param>
    /// <param name="y">Y座標</param>
    void FillHLine(int left, int right, int y)
    {
        if (left > right)
        {
            // 左右の位置関係が逆なので値をスワップする
            int work = left;
            left = right;
            right = work;
        }
        MapData.instance.FillRectLTRB(left, y, right + 1, y + 1, (int)MapData.MapChipType.none);
    }

    /// <summary>
    /// 垂直方向に線を引く (上と下の位置は自動で反転する)
    /// </summary>
    /// <param name="top">上</param>
    /// <param name="bottom">下</param>
    /// <param name="x">X座標</param>
    void FillVLine(int top, int bottom, int x)
    {
        if (top > bottom)
        {
            // 上下の位置関係が逆なので値をスワップする
            int work = top;
            top = bottom;
            bottom = work;
        }
        MapData.instance.FillRectLTRB(x, top, x + 1, bottom + 1, (int)MapData.MapChipType.none);
    }

    #region singleton

    static MapGenerator _instance;

    public static MapGenerator instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(MapGenerator));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use MapGenerator in the scene hierarchy.");
                    _instance = (MapGenerator)previous;
                }
                else
                {
                    var go = new GameObject("MapGenerator");
                    _instance = go.AddComponent<MapGenerator>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
