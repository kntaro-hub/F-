﻿using System.Collections;
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
        public Point point;
    }
    private struct UIMapChip
    {
        public Image image;
        public bool isShow;
        public Point point;
    }

    // =--------- プレハブ ---------= //
    // マップチッププレハブ
    [SerializeField, Tooltip("マップチップのプレハブです。")]
    private Image mapChipPrefab;    // マップ上のプレイヤープレハブ

    [SerializeField, Tooltip("マップ上のプレイヤーのプレハブ")]
    private Image mapPlayerPrefab;  // マップ上プレイヤーのプレハブ

    [SerializeField, Tooltip("マップ上の敵のプレハブ")]
    private Image mapEnemyPrefab;  // マップ上敵のプレハブ

    [SerializeField, Tooltip("マップ上のゴールのプレハブ")]
    private Image mapGoalPrefab;  // マップ上ゴールのプレハブ

    [SerializeField, Tooltip("マップ上のアイテムのプレハブ")]
    private Image mapItemPrefab;  // マップ上アイテムのプレハブ

    [SerializeField, Tooltip("マップの色")]
    private Color MapColor = new Color(1.0f, 0.0f, 1.0f, 0.3f);

    [SerializeField, Tooltip("マップ背景")]
    private Image MapBG;
    

    // =--------- 変数宣言 ---------= //

    private bool        isShowMap = false;  // マップ表示中かどうか
    private UIMapChip[,]    mapChips;           // マップチップ配列
    private Image       mapPlayer;          // マップ上のプレイヤー
    private UIMapChip mapGoal;            // マップ上のゴール
    private List<UIMapChip> mapEnemies = new List<UIMapChip>();         // マップ上の敵
    private List<UIMapChip> mapItems = new List<UIMapChip>();         // マップ上のアイテム

    private bool isShowBG = false;
    public bool IsShowBG{ get { return isShowBG; } }

    // Start is called before the first frame update
    void Start()
    {
        this.CreateMapChips();
    }

    // Update is called once per frame
    void Update()
    {
        if(isShowBG)
        {
            if(PS4Input.GetButtonDown( PS4ButtonCode.Triangle) ||
                PS4Input.GetButtonDown(PS4ButtonCode.Square) ||
                PS4Input.GetButtonDown(PS4ButtonCode.Cross) ||
                PS4Input.GetButtonDown(PS4ButtonCode.TouchPad) ||
                PS4Input.GetButtonDown(PS4ButtonCode.PS) ||
                PS4Input.GetButtonDown(PS4ButtonCode.Option))
            {
                this.HideMap();
            }
        }

//#if UNITY_EDITOR
        if (PS4Input.GetButtonDown(PS4ButtonCode.PS))
        {
            this.ShowMapUI();
        }
//#endif
    }

    /// <summary>
    /// マップチップ作成
    /// </summary>
    private void CreateMapChips()
    {
        // マップチップ配列生成
        mapChips = new UIMapChip[MapData.instance.Width, MapData.instance.Height];
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
                mapChips[i, j].image = (Instantiate(mapChipPrefab,
                    new Vector3(
                        mapChipPrefab.rectTransform.sizeDelta.x * j + Screen.width * 0.5f - v.x, 
                        mapChipPrefab.rectTransform.sizeDelta.y * i + Screen.height * 0.5f - v.y),
                    Quaternion.identity, 
                    this.transform));
            }
        }

        foreach(var itr in mapChips)
        {// マップチップをすべて透明に
            itr.image.color = Color.clear;
        }

        // マップ上のプレイヤー生成
        mapPlayer = Instantiate(mapPlayerPrefab, this.transform);
        mapPlayer.color = Color.clear;

        this.ShowPlayer();

        this.UpdateMap();
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
                if (type != MapData.MapChipType.wall)
                {// 道に色を付ける
                    mapChips[j, i].image.color = MapColor;
                    mapChips[j, i].isShow = true;
                }
            }
        }
    }
    public void ShowMapUI(int x, int y)
    {
        // 部分的にマップ開放
        mapChips[x, y].isShow = true;
    }
    public void ShowMapUI(int x, int y, bool isColor)
    {
        // 部分的にマップ開放
        mapChips[x, y].isShow = true;
        if(isColor)
        mapChips[x, y].image.color = MapColor;
    }

    private void  ShowMapUI_Around(Point point)
    {
        foreach(var itr in MapData.DirectPoints)
        {
            if(MapData.instance.GetMapChipType(point + itr) != MapData.MapChipType.wall)
            {
                this.ShowMapUI((point + itr).y, (point + itr).x);
            }
        }
    }
    private void ShowMapUI_Around(Point point, bool isColor)
    {
        foreach (var itr in MapData.DirectPoints)
        {
            if (MapData.instance.GetMapChipType(point + itr) != MapData.MapChipType.wall)
            {
                this.ShowMapUI((point + itr).y, (point + itr).x, isColor);
            }
        }
    }

    private void ShowPlayer()
    {
        mapPlayer.color = new Color(1.0f, 1.0f, 1.0f, 0.9f);
    }

    public void UpdateMap(bool isColor = false)
    {
        //if (IsShowMap)
        {
            // マップ上プレイヤー更新
            UpdateMapPlayer(isColor);

            // マップ上敵更新
            int cnt = 0;
            foreach (var itr in mapEnemies)
            {
                itr.image.color = Color.clear;
            }
            foreach (var itr in EnemyMGR.instance.EnemyList)
            {
                // 敵は塗りつぶさずに位置だけ更新
                Point enemyPoint = itr.status.point;
                mapEnemies[cnt].image.rectTransform.position = mapChips[itr.status.point.y, itr.status.point.x].image.transform.position;
                mapEnemies[cnt].point.SetVal(mapChips[itr.status.point.y, itr.status.point.x].point);
                if (mapChips[itr.status.point.y, itr.status.point.x].isShow)
                {// マップ解放されている場合
                    var obj = mapEnemies[cnt];
                    mapEnemies[cnt] = obj;
                    //if (MapData.instance.GetRoom(itr.status.point) == MapData.instance.GetRoom(SequenceMGR.instance.Player.status.point))
                    {
                        obj.image.color = Color.white;
                    }
                    obj.isShow = true;
                }
                else // 解放されていない
                {
                    var obj = mapEnemies[cnt];
                    mapEnemies[cnt] = obj;
                    obj.image.color = Color.clear;
                }
                ++cnt;
            }

            if (mapChips[mapGoal.point.y, mapGoal.point.x].isShow)
            {
                mapGoal.image.color = Color.white;
                mapGoal.image.rectTransform.position = mapChips[mapGoal.point.y, mapGoal.point.x].image.transform.position;
            }

            // マップ上アイテム更新
            cnt = 0;
            foreach (var itr in mapItems)
            {
                itr.image.color = Color.clear;
            }
            foreach (var itr in ItemMGR.instance.Items)
            {
                // 敵は塗りつぶさずに位置だけ更新
                Point itemPoint = itr.Point;
                mapItems[cnt].image.rectTransform.position = mapChips[itr.Point.y, itr.Point.x].image.transform.position;
                mapItems[cnt].point.SetVal(mapChips[itr.Point.y, itr.Point.x].point);
                if (mapChips[itr.Point.y, itr.Point.x].isShow)
                {// マップ解放されている場合
                    var obj = mapItems[cnt];
                    mapItems[cnt] = obj;
                    obj.image.color = Color.white;
                    obj.isShow = true;
                }
                else
                {
                    var obj = mapItems[cnt];
                    mapItems[cnt] = obj;
                    obj.image.color = Color.clear;
                }
                ++cnt;
            }
        }
    }

    public void UpdateMapPlayer(bool isColor)
    {
        Point playerPoint = SequenceMGR.instance.Player.status.point;
        mapPlayer.rectTransform.position = mapChips[playerPoint.y, playerPoint.x].image.transform.position;

        // プレイヤーの位置と周り8マスを塗りつぶす
        this.ShowMapUI_Around(playerPoint, isColor);

        // プレイヤーが部屋にいた場合部屋を塗りつぶす
        this.ShowMap_InRoom(playerPoint, isColor);
    }

    public void CreateMapEnemy(Point point)
    {
        UIMapChip obj = new UIMapChip();
        obj.image = Instantiate(mapEnemyPrefab, this.transform);
        obj.image.color = Color.clear;
        obj.point = point;
        mapEnemies.Add(obj);
    }

    public void CreateMapItem(Point point)
    {
        UIMapChip obj = new UIMapChip();
        obj.image = Instantiate(mapItemPrefab, this.transform);
        obj.image.color = Color.clear;
        obj.point = point;
        mapItems.Add(obj);
    }

    public void RemoveMapEnemy()
    {
        if (mapEnemies.Count > 0)
        {
            Destroy(mapEnemies[0].image.gameObject);
            mapEnemies.RemoveAt(0);
        }
    }

    public void RemoveMapItem()
    {
        if (mapItems.Count > 0)
        {
            Destroy(mapItems[0].image.gameObject);
            mapItems.RemoveAt(0);
            this.UpdateMap();
        }
    }

    private void ShowMap_InRoom(Point point)
    {
        // 部屋の中だった場合
        Division.Div_Room room = this.IsPointInRoom(point);
        if (room != null)
        {
            for (int i = 0; i < room.Width + 2; ++i)
            {
                for (int j = 0; j < room.Height + 2; ++j)
                {
                    var mapChipType = MapData.instance.GetMapChipType(room.Left + i - 1, room.Top + j - 1);
                    if(mapChipType == MapData.MapChipType.room           ||
                       mapChipType == MapData.MapChipType.roomAround ||
                       mapChipType == MapData.MapChipType.aisleGate      ||
                       mapChipType == MapData.MapChipType.goal)
                        this.ShowMapUI(room.Top + j - 1, room.Left + i - 1);
                }
            }
        }

    }

    private void ShowMap_InRoom(Point point, bool isColor)
    {
        // 部屋の中だった場合
        Division.Div_Room room = this.IsPointInRoom(point);
        if (room != null)
        {
            for (int i = 0; i < room.Width + 2; ++i)
            {
                for (int j = 0; j < room.Height + 2; ++j)
                {
                    var mapChipType = MapData.instance.GetMapChipType(room.Left + i - 1, room.Top + j - 1);
                    if (mapChipType == MapData.MapChipType.room ||
                       mapChipType == MapData.MapChipType.roomAround ||
                       mapChipType == MapData.MapChipType.aisleGate ||
                       mapChipType == MapData.MapChipType.goal)
                        this.ShowMapUI(room.Top + j - 1, room.Left + i - 1, isColor);
                }
            }
        }

    }

    public void CreateMapGoal(Point point)
    {
        mapGoal.image = Instantiate(mapGoalPrefab, this.transform);
        mapGoal.image.color = Color.clear;
        mapGoal.point = point;
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

    public void HideMap()
    {
        if (!isShowMap)
        {
            foreach (var itr in mapChips) { itr.image.color = Color.clear; }
            foreach(var itr in mapItems){ itr.image.color = Color.clear; }
            foreach (var itr in mapEnemies){ itr.image.color = Color.clear; }
            mapPlayer.color = Color.clear;
            mapGoal.image.color = Color.clear;
            isShowMap = true;

            this.MapBG.color = Color.clear;
            isShowBG = false;
        }
    }

    public void ShowMap()
    {
        if (isShowMap)
        {
            foreach (var itr in mapChips)
            {
                if(itr.isShow)
                    itr.image.color = MapColor;
            }

            foreach (var itr in mapItems)
            {
                if(itr.isShow)
                itr.image.color = Color.clear;
            }

            foreach (var itr in mapEnemies)
            {
                if (itr.isShow)
                    itr.image.color = Color.clear;
            }

            mapPlayer.color = Color.white;

            if(mapGoal.isShow)
                mapGoal.image.color = Color.white;
            isShowMap = false;
        }

        this.UpdateMap();
    }
    public void ShowMap(bool isBG)
    {
        this.ShowMap();

        this.MapBG.color = Color.black;
        isShowBG = true;
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;
    }
}
