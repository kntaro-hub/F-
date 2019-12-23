using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マップUI表示クラス
/// </summary>
public class UI_Map : MonoBehaviour
{
    // =--------- 構造体定義 ---------= //
    private struct UIMapObject
    {
        public Image image;
        public bool isShow;
        public Point point;
    }

    // =--------- プレハブ ---------= //
    // マップチッププレハブ
    [SerializeField, Tooltip("マップチップのプレハブです。")]
    private Image mapChipPrefab;    // マップ上のプレイヤープレハブ

    [SerializeField, Tooltip("マップ上のプレイヤーのプレハブです。")]
    private Image mapPlayerPrefab;  // マップ上プレイヤーのプレハブ

    [SerializeField, Tooltip("マップ上の敵のプレハブです。")]
    private Image mapEnemyPrefab;  // マップ上敵のプレハブ

    [SerializeField, Tooltip("マップ上のゴールのプレハブです。")]
    private Image mapGoalPrefab;  // マップ上ゴールのプレハブ

    // =--------- 変数宣言 ---------= //

    private bool        IsShowMap = false;  // マップ表示中かどうか
    private Image[,]    mapChips;           // マップチップ配列
    private Image       mapPlayer;          // マップ上のプレイヤー
    private UIMapObject mapGoal;            // マップ上のゴール
    private List<UIMapObject> mapEnemies = new List<UIMapObject>();         // マップ上の敵

    // =---------  ---------= //

    // Start is called before the first frame update
    void Start()
    {
        this.CreateMapChips();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// マップチップ作成
    /// </summary>
    private void CreateMapChips()
    {
        // マップチップ配列生成
        mapChips = new Image[MapData.instance.Width, MapData.instance.Height];
    }

    /// <summary>
    /// マップUI生成
    /// </summary>
    public void CreateMapUI()
    {
        // マップチップを並べる際に発生するズレを修正する用
        Vector2 v = new Vector2(mapChipPrefab.rectTransform.sizeDelta.x * MapData.instance.Width * 0.5f,
                        mapChipPrefab.rectTransform.sizeDelta.y * MapData.instance.Height * 0.5f);

        for(int i = 0; i < (MapData.instance.Height); ++i)
        {// マップチップ分ループ
            for (int j = 0; j < (MapData.instance.Width); ++j)
            {
                // 座標、角度、親を設定して一枚ずつ生成
                mapChips[i, j] = (Instantiate(mapChipPrefab,
                    new Vector3(
                        mapChipPrefab.rectTransform.sizeDelta.x * j + Screen.width * 0.5f - v.x, 
                        mapChipPrefab.rectTransform.sizeDelta.y * i + Screen.height * 0.5f - v.y),
                    Quaternion.identity, 
                    this.transform));
            }
        }

        foreach(var itr in mapChips)
        {// マップチップをすべて透明に
            itr.color = Color.clear;
        }

        // マップ上のプレイヤー生成
        mapPlayer = Instantiate(mapPlayerPrefab, this.transform);
        mapPlayer.color = Color.clear;

        //this.ShowMapUI();
        this.ShowPlayer();

        this.UpdateMapPlayer();
    }

    /// <summary>
    /// マップUI表示
    /// </summary>
    public void ShowMapUI()
    {
        for (int i = 0; i < (MapData.instance.Width); ++i)
        {// マップチップ分ループ
            for (int j = 0; j < (MapData.instance.Height); ++j)
            {
                MapData.MapChipType type = MapData.instance.GetMapChipType(i, j);
                if (type == MapData.MapChipType.none ||
                    type == MapData.MapChipType.room)
                {// 道に色を付ける
                    mapChips[j, i].color = new Color(0.0f, 1.0f, 0.3f, 0.2f);
                }
            }
        }
        IsShowMap = true;
    }
    public void ShowMapUI(int x, int y)
    {
        // 部分的にマップ開放
        mapChips[x, y].color = new Color(0.0f, 1.0f, 0.3f, 0.2f);
        IsShowMap = true;
    }

    private void ShowPlayer()
    {
        mapPlayer.color = new Color(1.0f, 0.0f, 0.0f, 0.4f);
    }

    public void UpdateMapPlayer()
    {
        // マップ上プレイヤー更新
        Point playerPoint = SequenceMGR.instance.Player.GetPoint();
        mapPlayer.rectTransform.position = mapChips[playerPoint.y, playerPoint.x].transform.position;
        
        // プレイヤーの位置を塗りつぶす
        this.ShowMapUI(playerPoint.y, playerPoint.x);

        // プレイヤーが部屋にいた場合部屋を塗りつぶす
        this.ShowMap_InRoom(playerPoint);

        // マップ上敵更新
        int cnt = 0;
        foreach(var itr in SequenceMGR.instance.Enemies)
        {
            // 敵は塗りつぶさずに位置だけ更新
            Point enemyPoint = itr.status.point;
            mapEnemies[cnt].image.rectTransform.position = mapChips[itr.status.point.y, itr.status.point.x].transform.position;
            if(!mapEnemies[cnt].isShow)
            {// まだ発見していない場合
                // 解放されたマップなのか判定
                if(mapChips[itr.status.point.y, itr.status.point.x].color.a > 0.0f)
                {// 解放されたマップの場合は敵の姿をみえるようにする
                    var obj = mapEnemies[cnt];
                    obj.isShow = true;
                    mapEnemies[cnt] = obj;
                    obj.image.color = Color.white;
                }
            }
            ++cnt;
        }

        if(!mapGoal.isShow)
        {
            if (mapChips[mapGoal.point.y, mapGoal.point.x].color.a > 0.0f)
            {// 解放されたマップの上なら
                mapGoal.isShow = true;
                mapGoal.image.color = Color.white;
                mapGoal.image.rectTransform.position = mapChips[mapGoal.point.y, mapGoal.point.x].transform.position;
            }
        }
    }

    public void CreateMapEnemy(Point point)
    {
        UIMapObject obj = new UIMapObject();
        obj.image = Instantiate(mapEnemyPrefab, this.transform);
        obj.isShow = false;
        obj.image.color = Color.clear;
        obj.point = point;
        mapEnemies.Add(obj);
    }

    public void RemoveMapEnemy(Point point)
    {
        for (int i = mapEnemies.Count - 1; i >= 0; i--)
        {// 逆順ループ
            // ID検索してヒットした敵を消す リストからも
            if (mapEnemies[i].image.rectTransform.position == mapChips[point.x, point.y].transform.position)
            {
                Destroy(mapEnemies[i].image.gameObject);
                mapEnemies.RemoveAt(i);
            }
        }
    }

    private void ShowMap_InRoom(Point point)
    {
        // 部屋の中だった場合
        Division.Div_Room room = this.IsPointInRoom(point);
        if (room != null)
        {
            for (int i = 0; i < room.Width; ++i)
            {
                for (int j = 0; j < room.Height; ++j)
                {
                    this.ShowMapUI(room.Top + j, room.Left + i);
                }
            }
        }

    }
    
    public void CreateMapGoal(Point point)
    {
        mapGoal.image = Instantiate(mapGoalPrefab, this.transform);
        mapGoal.isShow = false;
        mapGoal.image.color = Color.clear;
        mapGoal.point = point;
    }

    private void ShowMapGoal()
    {
        mapGoal.image.color = Color.white;
    }

    /// <summary>
    /// pointがどれかの部屋にいるかどうか
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Division.Div_Room IsPointInRoom(Point point)
    {
        foreach (var itr in MapGenerator.instance.DivList)
        {
            if (point.x >= itr.Room.Left &&
                point.x <= itr.Room.Right &&
                point.y >= itr.Room.Top &&
                point.y <= itr.Room.Bottom)
            {// 部屋の中にポイントがある
                return itr.Room;
            }
        }
        return null;
    }
}
