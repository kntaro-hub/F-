﻿using System.Collections;
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

    // 敵のアニメーター
    protected Animator enemyAnimator;

    // =--------- // =--------- unity execution ---------= // ---------= //

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {

        // アニメーター取得
        enemyAnimator = this.GetComponent<Animator>();

        this.enemyAnimator.Play("IdleBattle");

        // キャラクタータイプ設定
        this.status.characterType = CharaType.enemy;

        // 敵情報取得
        EnemyTableEntity entity = DataBase.instance.GetEnemyTableEntity((int)enemyType);

        // 敵パラメータ設定
        enemyType = (EnemyMGR.EnemyType)entity.TypeID;
        param.hp = entity.MaxHP;
        param.atk = entity.Atk;
        param.def = entity.Def;
        param.Name = entity.Name;
        param.xp = entity.Xp;
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
                this.transform.LookAt(MapData.GridToWorld(this.status.point), Vector3.up);

                this.transform.DOMove(MapData.GridToWorld(this.status.point, -0.5f), Actor.MoveTime).SetEase(Ease.Linear);

                // マップに敵を登録
                MapData.instance.SetMapObject(status.point, MapData.MapObjType.enemy, param.id);

                // タイマー起動（指定秒数経過するとターンエンド状態になる）
                StartCoroutine(Timer(MoveTime));

                this.enemyAnimator.Play("WalkFWD");
            }
            else
            {// 移動先に移動できない場合
                this.status.actType = ActType.TurnEnd;
            }
        }
    }

    public void SkipProc()
    {
        // DecideCommand()で決定したコマンドが移動だった場合
        if (status.actType == ActType.Move)
        {
            if (this.Move())
            {// 移動先が確定した場合

                this.transform.LookAt(MapData.GridToWorld(this.status.point), Vector3.up);

                // マップに敵を登録
                MapData.instance.SetMapObject(status.point, MapData.MapObjType.enemy, param.id);

                this.status.actType = ActType.TurnEnd;
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
        if (status.actType == ActType.Act)
        {
            this.transform.LookAt(SequenceMGR.instance.Player.transform.position, Vector3.up);
            this.enemyAnimator.Play("Attack01");
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
    protected virtual bool Skip()
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
        int calcDamage = this.CalcDamage(damage);

        MessageWindow.instance.AddMessage($"{this.Param.Name}に{calcDamage}のダメージ！", Color.white);

        this.enemyAnimator.Play("GetHit");

        if (this.SubHP(calcDamage))
        {
            MessageWindow.instance.AddMessage($"{this.param.Name}をたおした！", Color.white);

            EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Enemy_Dead, this.status.point);

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
        UI_MGR.instance.Ui_Map.RemoveMapEnemy();

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

    #region ダメージ計算

    /// <summary>
    /// トルネコ式攻撃力計算
    /// </summary>
    /// <returns>整数の攻撃値</returns>
    public override int CalcAtk()
    { 
        // 計算結果を返す
        return this.param.atk;
    }

    /// <summary>
    /// トルネコ式ダメージ計算
    /// 1を下回った場合、最低1ダメージ
    /// </summary>
    /// <param name="atk">攻撃側の計算後攻撃力</param>
    /// <returns>整数のダメージ値</returns>
    public override int CalcDamage(int Atk)
    {
        if (this.param.shieldId != DataBase.instance.GetItemTableCount() - 1)
        {
            // 防御力計算
            Atk = (int)(Atk * Mathf.Pow((15.0f / 16.0f), this.param.def));  // 攻撃力と基本ダメージ
            Atk = (int)Mathf.Floor(Atk * Random.Range(112, 143) / 128);   // 結果
        }
        else
        {// なにも装備していない場合
         // 防御力計算
            Atk = (int)Mathf.Floor(Atk * Random.Range(112, 143) / 128);   // 結果
        }

        if (Atk < 1)
        {// 計算結果が1を下回った場合

            // 最低でも1ダメージ
            Atk = 1;
        }

        // 計算結果を返す
        return Atk;
    }

    #endregion

}
