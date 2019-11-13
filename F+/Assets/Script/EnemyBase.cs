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
}
