using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 敵基底クラス
/// </summary>
public class EnemyBase : Actor
{
    public EnemyMGR.EnemyType enemyType;

    protected Point targetPoint;    // 目標地点
    public Point TargetPoint
    {
        get { return targetPoint; }
        set { targetPoint = value; }
    }

    protected Point directPoint;

    // =--------- // =--------- unity execution ---------= // ---------= //

    // Start is called before the first frame update
    void Start()
    {
        this.status.characterType = CharaType.enemy;
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
        // DecideCommand()で決定したコマンドが移動だった場合
        if (status.actType == ActType.Move)
        {
            if (this.Move())
            {// 移動先が確定した場合
                this.transform.DOMove(MapData.GridToWorld(this.status.point), Actor.MoveTime).SetEase(Ease.Linear);

                // マップに敵を登録
                MapData.instance.SetMapObject(status.point, MapData.MapObjType.enemy, param.id);

                // タイマー起動（指定秒数経過するとターンエンド状態になる）
                StartCoroutine(Timer(MoveTime));
            }
            else
            {// 移動先に移動できない場合
                this.status.actType = ActType.TurnEnd;
            }
        }
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    public float ActProc()
    {
        if (status.actType == ActType.Act &&
            SequenceMGR.instance.Player != null)
        {
            return this.Act();
        }
        else return 0.0f;
    }

    public void MapDataUpdate()
    {
        MapData.instance.SetMapObject(this.status.point, MapData.MapObjType.enemy, param.id);
    }

    // =--------- // =--------- 継承先で変更する ---------= // ---------= //
    protected virtual bool Move()
    {
        return false;
    }
    protected virtual float Act()
    {
        return 0.0f;
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
    /// 行動決定2
    /// ここで敵はもう一度状況を確認する
    /// </summary>
    public virtual void DecideCommand2()
    {

    }

    public override void Damage(int damage, bool isXp)
    {
        int calcDamage = this.param.CalcDamage(damage);

        MessageWindow.instance.AddMessage($"{this.Param.Name}に{calcDamage}のダメージ！", Color.white);

        if (this.SubHP(calcDamage))
        {
            MessageWindow.instance.AddMessage($"{this.param.Name}をたおした！", Color.white);

            // 敵削除
            EnemyMGR.instance.DestroyEnemy(this.param.id, isXp);
        }
    }


    public override void DestroyObject(bool isXp)
    {
        if(isXp)
        {
            // プレイヤーに経験値加算
            SequenceMGR.instance.Player.AddXp(this.Param.xp);
        }

        // マップに登録してある自分の情報を消す
        MapData.instance.ResetMapObject(status.point);

        // 確率でアイテムをドロップ
        if (Percent.Per(DataBase.instance.GetEnemyTableEntity((int)this.enemyType).DropPer))
        {
            // アイテムをランダム生成
            ItemMGR.instance.CreateItem(this.status.point, Random.Range(0, DataBase.instance.GetItemTableCount() - 1));
        }

        // マップ上の自分を消す
        UI_MGR.instance.Ui_Map.RemoveMapEnemy(this.status.point);

        // オブジェクト削除
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 座標を返す
    /// </summary>
    public Point GetPoint()
    {
        return this.status.point;
    }

    public int GetID()
    {
        return this.param.id;
    }
    public void SetPoint(Point point)
    {
        this.status.point = point;
    }

    public void SetID(int id)
    {
        this.param.id = id;
    }

    // =--------- // =--------- コルーチン ---------= // ---------= //

    /// <summary>
    /// ターンエンドタイマー
    /// 指定秒数経過すると自動的にターンエンドとなる
    /// BaseのMoveでは、秒数が決められているためBaseに直接書いてあるが、
    /// 行動は敵の種類によってターンエンドのタイミングが違うため
    /// BaseのActには書いていない（個別のscriptで書く）
    /// </summary>
    protected IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);

        status.actType = ActType.TurnEnd;
    }

    // =--------- // =--------- ---------= // ---------= //
}
