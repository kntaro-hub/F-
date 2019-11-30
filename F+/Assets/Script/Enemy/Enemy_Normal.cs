using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override bool Move()
    {
        Point movedPoint = new Point();

        // 今いた場所をリセット
        MapData.instance.ResetMapObject(status.gridPos);

        targetPoint = SequenceMGR.instance.Player.status.gridPos;

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
        if (MapData.instance.GetMapObject(movedPoint) != MapData.ObjectOnTheMap.none)
        {// オブジェクトがある場合
            return false;
        }

        status.gridPos = movedPoint;

        return true;
    }

    protected override void Act()
    {
        // 行動処理
    }

    /// <summary>
    /// 行動決定　
    /// ここで敵は挙動を自分で判断して決定する
    /// </summary>
    public override void DecideCommand()
    {
        if (!isTarget)
        {
            if (Point.Distance(SequenceMGR.instance.Player.status.gridPos, this.status.gridPos) < searchRange)
            {
                isTarget = true;
            }
        }
        
        if(isTarget)
        {
            enemyAct = EnemyAct.move;
        }
    }

    /// <summary>
    /// DecideCommandで決定した挙動を実行する
    /// </summary>
    public override void ExecuteCommand()
    {
        switch(enemyAct)
        {
            case EnemyAct.move:
                this.Move();
                break;


            case EnemyAct.act:
                break;

        }
    }
}
