using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 敵基底クラス
/// </summary>
public class EnemyBase : Actor
{
    public bool isDestroy = false;
    public bool IsDestroy
    {
        get { return isDestroy; }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = FieldMGR.GridToWorld(status.gridPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 目的地へ移動する
    /// </summary>
    /// <param name="setPoint">目的地</param>
    public void MoveProc(Point setPoint)
    {
        this.Move(setPoint);

        this.transform.DOMove(FieldMGR.GridToWorld(this.status.gridPos), Actor.MoveTime).SetEase(Ease.Linear);
        this.status.actType = ActType.Move;

        // 移動タイマー起動
        StartCoroutine(MoveTimer());
    }

    protected virtual void Move(Point setPoint)
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

    // 移動タイマー
    private IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(MoveTime);

        status.actType = ActType.TurnEnd;

        if (SequenceMGR.instance.IsTurnEnd())
        {
            SequenceMGR.instance.ResetAct();
        }
    }
}
