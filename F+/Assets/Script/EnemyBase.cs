using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 敵基底クラス
/// </summary>
public class EnemyBase : Actor
{
    private AStarSys aStar = null;
    public AStarSys AStar
    {
        get { return aStar; }
    }

    public bool IsDestroy = false;

    // Start is called before the first frame update
    void Start()
    {
        aStar = GetComponent<AStarSys>();
        status.gridPos = aStar.StartPoint;
        this.transform.position = FieldMGR.GridToWorld(status.gridPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Point setPoint)
    {
        this.AStar.SetGoal(setPoint);
        this.status.gridPos = this.AStar.A_StarProc_Single();
        this.transform.DOMove(FieldMGR.GridToWorld(this.status.gridPos), Actor.MoveTime).SetEase(Ease.Linear);
        this.status.actType = ActType.Move;

        // 移動タイマー起動
        StartCoroutine(MoveTimer());
    }

    public void Destroy()
    {
        IsDestroy = true;
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
