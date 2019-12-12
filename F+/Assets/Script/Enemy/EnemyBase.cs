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

    public enum EnemyType
    {
        normal = 0,
        max
    }
    public EnemyType enemyType;


    protected Point targetPoint;    // 目標地点
    public Point TargetPoint
    {
        get { return targetPoint; }
        set { targetPoint = value; }
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
        if (enemyAct == EnemyAct.move)
        {
            if (this.Move())
            {
                this.transform.DOMove(MapData.GridToWorld(this.status.gridPos), Actor.MoveTime).SetEase(Ease.Linear);
                this.status.actType = ActType.Move;

                // マップに敵を登録
                MapData.instance.SetMapObject(status.gridPos, MapData.MapObjType.enemy, param.id);

                // タイマー起動（指定秒数経過するとターンエンド状態になる）
                StartCoroutine(Timer());
            }
        }
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    public void ActProc()
    {
        if (enemyAct == EnemyAct.act)
        {
            this.Act();
        }
    }

    public void Destroy()
    {
        // マップに登録してある自分の情報を消す
        MapData.instance.ResetMapObject(status.gridPos);

        // 確率でアイテムをドロップ
        if (Percent.Per(DataBase.instance.GetEnemyTable((int)this.enemyType).DropPer))
        {
            // アイテムをランダム生成
            ItemMGR.instance.CreateItem(this.status.gridPos, Random.Range(0, DataBase.instance.GetItemTableCount() - 1));
        }

        // オブジェクト消去
        Destroy(this.gameObject);
    }

    public void MapDataUpdate()
    {
        MapData.instance.SetMapObject(this.status.gridPos, MapData.MapObjType.enemy, param.id);
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

    // =--------- // =--------- コルーチン ---------= // ---------= //

    /// <summary>
    /// ターンエンドタイマー
    /// 指定秒数経過すると自動的にターンエンドとなる
    /// BaseのMoveでは、秒数が決められているためBaseに直接書いてあるが、
    /// 行動は敵の種類によってターンエンドのタイミングが違うため
    /// BaseのActには書いていない（個別のscriptで書く）
    /// </summary>
    protected IEnumerator Timer()
    {
        yield return new WaitForSeconds(MoveTime);

        status.actType = ActType.TurnEnd;
    }

    // =--------- // =--------- ---------= // ---------= //
}
