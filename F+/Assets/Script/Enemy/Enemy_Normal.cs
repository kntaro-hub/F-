﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy_Normal : EnemyBase
{
    // 索敵範囲
    [SerializeField]
    private int searchRange = 7;

    // ターゲットを発見したかどうか
    private bool isTarget;

    private AStarSys aStar = null;
    public AStarSys AStar
    {
        get { return aStar; }
    }

    // Start is called before the first frame update
    void Start()
    {
        aStar = GetComponent<AStarSys>();
        status.gridPos = aStar.StartPoint;

        param.hp = 4;
        param.atk = 2;
        param.def = 1;
        param.id = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override bool Move()
    {
        
        Point movedPoint = new Point();

        targetPoint = SequenceMGR.instance.Player.status.gridPos;

        this.aStar.CheckWall_StartPointSet(status.gridPos);
        
        // 目的地更新
        this.aStar.SetGoal(targetPoint);

        if (MapData.instance.GetRoomNum(status.gridPos) ==
            MapData.instance.GetRoomNum(SequenceMGR.instance.Player.status.gridPos))
        {// 部屋の中かつ、プレイヤーと同じ部屋
         // 一回分探索した後の座標を取得
            movedPoint = this.aStar.A_StarProc_Single();
        }
        else
        {// 経路探索を簡単なやつにする
            movedPoint = this.aStar.SimpleProc(status.gridPos);
        }

        // 探索した座標に別のオブジェクトがあった場合は移動しない
        if (MapData.instance.GetMapObject(movedPoint).objType != MapData.MapObjType.none)
        {// オブジェクトがある場合
            return false;
        }

        // 今いた場所をリセット
        MapData.instance.ResetMapObject(status.gridPos);

        status.gridPos = movedPoint;
        return true;
    }

    protected override void Act()
    {
        // 行動処理
        Point point;

        // プレイヤーへの距離を求める
        int dx = status.gridPos.x - SequenceMGR.instance.Player.status.gridPos.x;
        int dy = status.gridPos.y - SequenceMGR.instance.Player.status.gridPos.y;
        if (Mathf.Abs(dx) > Mathf.Abs(dy))
        {
            // X方向への距離の方が遠いのでそっちに進む
            if (dx < 0)
            {
                point = new Point(1, 0);
            } // 左
            else
            {
                point = new Point(-1, 0);
            } // 右
        }
        else
        {
            // Y方向へ進む
            if (dy < 0)
            {
                point = new Point(0, 1);
            } // 上
            else
            {
                point = new Point(0, -1);
            } // 下
        }

        this.transform.DOPunchPosition(MapData.GridToWorld(point), MoveTime);

        // プレイヤー被ダメモーション
        // 攻撃が成功した場合
        SequenceMGR.instance.Player.Damage(this.param.atk);

        // タイマー起動（指定秒数経過するとターンエンド状態になる）
        StartCoroutine(Timer());
    }

    /// <summary>
    /// 行動決定　
    /// ここで敵は挙動を自分で判断して決定する
    /// </summary>
    public override void DecideCommand()
    {
        #region 行動処理をここで決定

        // プレイヤーの移動後の座標を見て攻撃するか移動するか決定
        if (MapData.instance.GetMapObject(new Point(status.gridPos.x + 1, status.gridPos.y)).objType == MapData.MapObjType.player ||
            MapData.instance.GetMapObject(new Point(status.gridPos.x - 1, status.gridPos.y)).objType == MapData.MapObjType.player ||
            MapData.instance.GetMapObject(new Point(status.gridPos.x, status.gridPos.y + 1)).objType == MapData.MapObjType.player ||
            MapData.instance.GetMapObject(new Point(status.gridPos.x, status.gridPos.y - 1)).objType == MapData.MapObjType.player)
        {// プレイヤーが真横にいる

            // 行動に遷移
            enemyAct = EnemyAct.act;
        }
        else 
        {
            #region 移動処理をここで決定
            if (!isTarget)
            {// ターゲットを見つけられていない場合
                if (Point.Distance(SequenceMGR.instance.Player.status.gridPos, this.status.gridPos) < searchRange)
                {// プレイヤーとの距離が指定の範囲内なら

                    // ターゲット発見フラグon
                    isTarget = true;
                }
            }

            if (isTarget)
            {// ターゲットを見つけた場合

                // 移動に遷移
                enemyAct = EnemyAct.move;
            }
            #endregion
        }

        #endregion
    }
}
