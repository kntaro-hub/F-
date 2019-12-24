using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 敵基底クラス
/// </summary>
public class EnemyBase : Actor
{
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
        this.transform.position = MapData.GridToWorld(status.point);
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
        if (status.actType == ActType.Move)
        {
            if (this.Move())
            {
                this.transform.DOMove(MapData.GridToWorld(this.status.point), Actor.MoveTime).SetEase(Ease.Linear);

                // マップに敵を登録
                MapData.instance.SetMapObject(status.point, MapData.MapObjType.enemy, param.id);

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
        if (status.actType == ActType.Act)
        {
            this.Act();
        }
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

        if (this.param.SubHP(calcDamage))
        {
            MessageWindow.instance.AddMessage($"{this.param.Name}をたおした！", Color.white);
            this.DestroyObject(isXp);
        }
    }


    public override void DestroyObject(bool isXp)
    {
        if(isXp)
        {
            // プレイヤーに経験値加算
            SequenceMGR.instance.Player.Param.AddXp(this.Param.xp);
        }

        // マップに登録してある自分の情報を消す
        MapData.instance.ResetMapObject(status.point);

        // 確率でアイテムをドロップ
        if (Percent.Per(DataBase.instance.GetEnemyTable((int)this.enemyType).DropPer))
        {
            // アイテムをランダム生成
            ItemMGR.instance.CreateItem(this.status.point, Random.Range(0, DataBase.instance.GetItemTableCount() - 1));
        }

        // マップ上の自分を消す
        UI_MGR.instance.Ui_Map.RemoveMapEnemy(this.status.point);

        SequenceMGR.instance.DestroyEnemyFromID(this.param.id);

        // オブジェクト消去
        Destroy(this.gameObject);
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
