using System.Collections;
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

    // AStarアルゴリズムで経路探索する用
    private AStarSys aStar = null;

    // Start is called before the first frame update
    void Start()
    {
        aStar = GetComponent<AStarSys>();
        status.point = aStar.StartPoint;

        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override bool Move()
    {
        targetPoint = SequenceMGR.instance.Player.status.point;

        this.aStar.CheckWall_StartPointSet(status.point);
        
        // 目的地更新
        this.aStar.SetGoal(targetPoint);

        if (MapData.instance.GetMapChipType(targetPoint) == MapData.MapChipType.room)
        {
            if (MapData.instance.GetRoomNum(status.point) ==
                MapData.instance.GetRoomNum(targetPoint))
            {// 部屋の中かつ、プレイヤーと同じ部屋
             // 一回分探索した後の座標を取得  // 4方向
                this.status.movedPoint = this.aStar.A_StarProc_Single2(false);
            }
            else
            {// 経路探索を簡単なやつにする
                this.status.movedPoint = this.aStar.SimpleProc(status.point);
            }
        }
        else
        {// 経路探索を簡単なやつにする
            this.status.movedPoint = this.aStar.SimpleProc(status.point);
        }

        // 探索した座標に別のオブジェクトがあった場合は移動しない
        MapData.MapObjType objType = MapData.instance.GetMapObject(this.status.movedPoint).objType;
        if (objType != MapData.MapObjType.none && 
            objType != MapData.MapObjType.item && 
            objType != MapData.MapObjType.trap)
        {// オブジェクトがある場合
            return false;
        }

        // 今いた場所をリセット
        MapData.instance.ResetMapObject(status.point);

        status.point = this.status.movedPoint;
        return true;
    }

    protected override float Act()
    {
        this.transform.DOPunchPosition(MapData.GridToWorld(directPoint), MoveTime * 2.0f);

        // 攻撃が成功した場合
        SequenceMGR.instance.Player.Damage(this.CalcAtk());

        // タイマー起動（指定秒数経過するとターンエンド状態になる）
        StartCoroutine(Timer(MoveTime * 3.0f));

        return MoveTime * 3.0f;
    }

    /// <summary>
    /// 行動決定　
    /// ここで敵は挙動を自分で判断して決定する
    /// </summary>
    public override void DecideCommand()
    {
        #region 行動処理をここで決定
        {
            if(MapData.instance.GetMapChipType(this.status.point) == MapData.MapChipType.room &&
                MapData.instance.GetMapChipType(SequenceMGR.instance.Player.status.point) == MapData.MapChipType.room)
            {
                if (MapData.instance.GetRoomNum(status.point) ==
                MapData.instance.GetRoomNum(targetPoint))
                {// 部屋の中かつ、プレイヤーと同じ部屋
                    directPoint = MapData.GetPointFromAround(this.status.point, MapData.MapObjType.player);
                }
                else
                {
                    directPoint = MapData.GetPointFromUDRL(this.status.point, MapData.MapObjType.player);
                }
            }
            else
            {
                directPoint = MapData.GetPointFromUDRL(this.status.point, MapData.MapObjType.player);
            }

            // プレイヤーの移動後の座標を見て攻撃するか移動するか決定
            if (directPoint != 0)
            {// プレイヤーが真横にいる

                // 行動に遷移
                this.status.actType = ActType.Act;
            }
            else
            {
                #region 移動処理をここで決定
                if (!isTarget)
                {// ターゲットを見つけられていない場合
                    if (Point.Distance(SequenceMGR.instance.Player.status.point, this.status.point) < searchRange)
                    {// プレイヤーとの距離が指定の範囲内なら

                        // ターゲット発見フラグon
                        isTarget = true;
                    }
                }

                if (isTarget)
                {// ターゲットを見つけた場合

                    // 移動に遷移
                    this.status.actType = ActType.Move;
                }
                else
                {

                    // ターゲットが見つかっていない場合はターンエンド
                    this.status.actType = ActType.TurnEnd;
                }
                #endregion
            }

            #endregion
        }
    }

    /// <summary>
    /// 行動決定　
    /// ここで敵は挙動をもう一度判断する
    /// </summary>
    public override void DecideCommand2()
    {
        if (this.status.actType == ActType.Act)
        {// プレイヤーに攻撃しようとしているとき
            // プレイヤーの移動後の座標を見て攻撃するか移動するか決定
            if (MapData.instance.GetMapObject(this.status.point + directPoint).objType != MapData.MapObjType.player)
            {// プレイヤーが真横からいなくなった

                // その場でターン終了
                this.status.actType = ActType.TurnEnd;
                return;
            }
        }
    }
}
