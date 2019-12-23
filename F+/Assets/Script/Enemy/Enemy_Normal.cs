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

    private AStarSys aStar = null;
    public AStarSys AStar
    {
        get { return aStar; }
    }

    // Start is called before the first frame update
    void Start()
    {
        aStar = GetComponent<AStarSys>();
        status.point = aStar.StartPoint;

        // 敵情報取得
        EnemyTableEntity entity = DataBase.instance.GetEnemyTable((int)enemyType);

        // 敵パラメータ設定
        enemyType   = (EnemyType)entity.TypeID;
        param.hp    = entity.MaxHP;
        param.atk   = entity.Atk;
        param.Name  = entity.Name;
        param.xp    = entity.Xp;
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

        if (MapData.instance.GetRoomNum(status.point) ==
            MapData.instance.GetRoomNum(SequenceMGR.instance.Player.status.point))
        {// 部屋の中かつ、プレイヤーと同じ部屋
         // 一回分探索した後の座標を取得
            this.status.movedPoint = this.aStar.A_StarProc_Single();
        }
        else
        {// 経路探索を簡単なやつにする
            this.status.movedPoint = this.aStar.SimpleProc(status.point);
        }

        this.status.actType = ActType.TurnEnd;

        // 探索した座標に別のオブジェクトがあった場合は移動しない
        MapData.MapObjType objType = MapData.instance.GetMapObject(this.status.movedPoint).objType;
        if (objType != MapData.MapObjType.none && objType != MapData.MapObjType.item)
        {// オブジェクトがある場合
            return false;
        }

        // 今いた場所をリセット
        MapData.instance.ResetMapObject(status.point);

        status.point = this.status.movedPoint;
        return true;
    }

    protected override void Act()
    {
        // 行動処理
        Point point;

        // プレイヤーへの距離を求める
        int dx = status.point.x - SequenceMGR.instance.Player.status.point.x;
        int dy = status.point.y - SequenceMGR.instance.Player.status.point.y;
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
        SequenceMGR.instance.Player.Damage(this.param.CalcAtk());


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

        if (this.status.actType == ActType.TurnEnd)
        {
            if (MapData.instance.GetMapObject(new Point(status.point.x + 1, status.point.y)).objType != MapData.MapObjType.player &&
            MapData.instance.GetMapObject(new Point(status.point.x - 1, status.point.y)).objType != MapData.MapObjType.player &&
            MapData.instance.GetMapObject(new Point(status.point.x, status.point.y + 1)).objType != MapData.MapObjType.player &&
            MapData.instance.GetMapObject(new Point(status.point.x, status.point.y - 1)).objType != MapData.MapObjType.player)
            {// プレイヤーが真横にいない
                return;
            }
        }
        else
        {

            // プレイヤーの移動後の座標を見て攻撃するか移動するか決定
            if (MapData.instance.GetMapObject(new Point(status.point.x + 1, status.point.y)).objType == MapData.MapObjType.player ||
                MapData.instance.GetMapObject(new Point(status.point.x - 1, status.point.y)).objType == MapData.MapObjType.player ||
                MapData.instance.GetMapObject(new Point(status.point.x, status.point.y + 1)).objType == MapData.MapObjType.player ||
                MapData.instance.GetMapObject(new Point(status.point.x, status.point.y - 1)).objType == MapData.MapObjType.player)
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
                    this.status.actType = ActType.TurnEnd;
                }
                #endregion
            }

            #endregion
        }
    }
}
