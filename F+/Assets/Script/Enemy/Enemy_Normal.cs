using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Normal : EnemyBase
{
    // 索敵範囲
    [SerializeField]
    private int searchRange = 5;

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

    protected override void Move()
    {
        // =--------- 移動処理 ---------= //
        targetPoint = SequenceMGR.instance.Player.status.gridPos;
        // 目的地更新
        this.aStar.SetGoal(targetPoint);

        if (!isTarget)
        {
            if (Point.Distance(SequenceMGR.instance.Player.status.gridPos, this.status.gridPos) < searchRange)
            {
                isTarget = true;
            }
        }
        else
        {
            if (MapData.instance.GetRoomNum(status.gridPos) ==
                MapData.instance.GetRoomNum(SequenceMGR.instance.Player.status.gridPos))
            {// 部屋の中かつ、プレイヤーと同じ部屋
             // 一回分探索した後の座標を取得
                this.status.gridPos = this.aStar.A_StarProc_Single();
            }
            else
            {// 経路探索を簡単なやつにする
                this.status.gridPos = this.aStar.SimpleProc(status.gridPos);
            }
        }
    }

    

    protected override void Attack()
    {
        // 攻撃処理
    }
}
