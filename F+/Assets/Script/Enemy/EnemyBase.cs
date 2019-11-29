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
        move = 0,
        act,
        max
    }

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

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = MapData.GridToWorld(status.gridPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 目的地へ移動する
    /// </summary>
    /// <param name="setPoint">目的地</param>
    public void MoveProc()
    {
        this.Move();

        this.transform.DOMove(MapData.GridToWorld(this.status.gridPos), Actor.MoveTime).SetEase(Ease.Linear);
        this.status.actType = ActType.Move;

        // 移動タイマー起動
        StartCoroutine(MoveTimer());
    }

    protected virtual void Move()
    {
        // 各敵ごとに処理が異なる
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    public void AttackProc()
    {
        this.Attack();
    }

    protected virtual void Attack()
    {
        // 各敵ごとに処理が異なる
    }

    public void Destroy()
    {
        isDestroy = true;
    }

    /// <summary>
    /// 行動決定　
    /// ここで敵は挙動を自分で判断して決定する
    /// </summary>
    public void DecideCommand()
    {

    }

    /// <summary>
    /// DecideCommandで決定した挙動を実行する
    /// </summary>
    public void ExecuteCommand()
    {

    }

    // 移動タイマー
    private IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(MoveTime);

        status.actType = ActType.TurnEnd;
    }
}
