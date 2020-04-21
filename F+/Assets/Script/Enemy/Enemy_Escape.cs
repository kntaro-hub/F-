using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy_Escape : EnemyBase
{
    private enum TargetType
    {
        escape = 0,
        target,
        max
    }
    // 動作タイプ
    private TargetType targetType = TargetType.escape;


    // 索敵範囲
    [SerializeField]
    private int searchRange = 5;

    // ターゲットを発見したかどうか
    private bool isTarget;

    // AStarアルゴリズムで経路探索する用
    private AStarSys aStar = null;

    // 部屋リスト
    private List<Division> rooms = new List<Division>();

    private bool isEscapeTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        aStar = GetComponent<AStarSys>();

        aStar.SetStartPoint(this.status.point);

        base.Init();

        // 部屋リスト取得
        rooms = MapGenerator.instance.DivList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override bool Move()
    {
        if (this.status.point == TargetPoint)
        {
            // ターゲットに到着している場合
            isEscapeTarget = false;
        }

        if (!isEscapeTarget)
        {
            this.SetTargetRoom();
        }

        this.aStar.CheckWall_StartPointSet(status.point);
        
        // 目的地更新
        this.aStar.SetGoal(targetPoint);
        {// 経路探索を簡単なやつにする
            this.status.movedPoint = this.aStar.A_StarProc_Single2(false);
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
        return 0.0f;
    }

    /// <summary>
    /// 行動決定　
    /// ここで敵は挙動を自分で判断して決定する
    /// </summary>
    public override void DecideCommand()
    {
        // プレイヤーの移動後の座標を見て攻撃するか移動するか決定
        {
            #region 移動処理をここで決定
            if (!isTarget)
            {// ターゲットを見つけられていない場合
                if (Point.Distance(SequenceMGR.instance.Player.status.point, this.status.point) < searchRange)
                {// プレイヤーとの距離が指定の範囲内なら

                    // ターゲット発見フラグon
                    isTarget = true;
                }

                // 1でもダメージを受けていたら
                if(this.param.maxHp > this.param.hp)
                {
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
    }

    private void SetTargetRoom()
    {
        Division.Div_Room room;
        // ランダムな部屋の中心をターゲットにする（プレイヤーの部屋以外）
        do
        {
            room = rooms[Random.Range(0, rooms.Count)].Room;
        } while (MapData.instance.GetRoom(SequenceMGR.instance.Player.status.point) == room);
        TargetPoint = room.Center();
        isEscapeTarget = true;
    }

    /// <summary>
    /// 行動決定　
    /// ここで敵は挙動をもう一度判断する
    /// </summary>
    public override void DecideCommand2()
    {

    }
}
