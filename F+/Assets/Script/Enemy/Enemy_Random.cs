using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy_Random : EnemyBase
{
    // 攻撃するとき移動に遷移する確率
    private const int ChangeMoveRate = 40;

    // 移動するとき何もしない確率
    private const int NotMoveRate = 20;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override bool Move()
    {
        this.status.movedPoint = MapData.GetRandomPointFromAround(this.status.point);

        // 探索した座標に別のオブジェクトがあった場合は移動しない
        MapData.MapObjType objType = MapData.instance.GetMapObject(this.status.movedPoint).objType;
        if (objType != MapData.MapObjType.none &&
            objType != MapData.MapObjType.item &&
            objType != MapData.MapObjType.trap)
        {
            return false;
        }

        // 壁があった場合も移動しない
        MapData.MapChipType MapChipType = MapData.instance.GetMapChipType(this.status.movedPoint);
        if (MapChipType == MapData.MapChipType.wall)
        {
            return false;
        }
        
        // 20%の確立で何もしない
        if(Percent.Per(NotMoveRate))
        {
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
            directPoint = MapData.GetPointFromAround(this.status.point, MapData.MapObjType.player);

            // プレイヤーの移動後の座標を見て攻撃するか移動するか決定
            if (directPoint != 0)
            {// プレイヤーが真横にいる

                // 行動に遷移
                this.status.actType = ActType.Act;
                
                // 確率で移動に遷移
                if(Percent.Per(ChangeMoveRate))
                {
                    this.status.actType = ActType.Move;
                }
            }
            else
            {
                #region 移動処理をここで決定
                // 移動に遷移
                this.status.actType = ActType.Move;
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
