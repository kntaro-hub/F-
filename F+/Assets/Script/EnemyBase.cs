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

    // Start is called before the first frame update
    void Start()
    {
        aStar = GetComponent<AStarSys>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            aStar.SetGoal(SequenceMGR.instance.Player.GetPoint());
            status.gridPos = aStar.A_StarProc_Single();
            this.transform.DOMove(FieldMGR.instance.GridToWorld(status.gridPos),0.1f);
        }
    }

    override protected void UpdateProc()
    {
        if (actType == ActType.MoveBegin)
        {// 移動開始

        }
        else if (actType == ActType.ActBegin)
        {// 行動開始

        }
    }
}
