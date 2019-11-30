using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 敵基底クラス
/// </summary>
public class EnemyBase : Actor
{
    public enum EnemyAct
    {
        move = 0,   // 移動 // 敵それぞれで移動する条件を書く
        act,        // 行動 // これもそれぞれ、内容も
        max
    }
    protected EnemyAct enemyAct;

    protected Point targetPoint;    // 目標地点
    public Point TargetPoint
    {
        get { return targetPoint; }
        set { targetPoint = value; }
    }

    public bool isDestroy = false;
    public bool IsDestroy
    {
        get { return isDestroy; }
    }

    // =--------- // =--------- unity execution ---------= // ---------= //

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = MapData.GridToWorld(status.gridPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // =--------- // =--------- 外部で呼ばれる ---------= // ---------= //

    /// <summary>
    /// 目的地へ移動する
    /// </summary>
    /// <param name="setPoint">目的地</param>
    public void MoveProc()
    {
        if (this.Move())
        {
            this.transform.DOMove(MapData.GridToWorld(this.status.gridPos), Actor.MoveTime).SetEase(Ease.Linear);
            this.status.actType = ActType.Move;

            // マップに敵を登録
            MapData.instance.SetMapObject(status.gridPos, MapData.ObjectOnTheMap.enemy);

            // 移動タイマー起動
            StartCoroutine(MoveTimer());
        }
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    public void ActProc()
    {
        this.Act();
    }

    public void Destroy()
    {
        // マップに登録してある自分の情報を消す
        MapData.instance.ResetMapObject(status.gridPos);
        isDestroy = true;
    }

    // =--------- // =--------- 継承先で変更する ---------= // ---------= //
    protected virtual bool Move()
    {
        return false;
    }
    protected virtual void Act()
    {
        // 各敵ごとに処理が異なる
    }
    /// <summary>
    /// 行動決定　
    /// ここで敵は挙動を自分で判断して決定する
    /// </summary>
    public virtual void DecideCommand()
    {

    }

    /// <summary>
    /// DecideCommandで決定した挙動を実行する
    /// </summary>
    public virtual void ExecuteCommand()
    {

    }

    // =--------- // =--------- コルーチン ---------= // ---------= //

    // 移動タイマー
    private IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(MoveTime);

        status.actType = ActType.TurnEnd;
    }

    // =--------- // =--------- ---------= // ---------= //
}
