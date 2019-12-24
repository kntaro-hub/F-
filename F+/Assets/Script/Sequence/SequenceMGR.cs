﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SequenceMGR : MonoBehaviour
{
    // プレイヤーを監視して
    // 敵の動きを決定する

    
    // プレイヤー
    private PlayerControll player = null;
    public PlayerControll Player
    {
        get { return player; }
        set { player = value; }
    }
    public ref PlayerControll refPlayer
    {
        get { return ref player; }
    }

    private List<EnemyBase> enemies = new List<EnemyBase>();
    public List<EnemyBase> Enemies
    {
        get { return enemies; }
    }

    public enum SeqType
    {
        keyInput = 0,   // キー入力待ち      // 1番最初にこの状態
        moveImpossible,           // メニュー表示中
        max
    }

    public SeqType seqType;

    public enum ActSeqType
    {
        playerAct = 0,  // プレイヤー行動     // 行動が選ばれた場合 
        requestEnemy,   // 敵行動決め         // プレイヤーの行動の後、プレイヤー移動が決定した後
        requestEnemy2,  // 敵行動決め（2回目）// プレイヤーが実際に行動した後にもう一度状況を確認する
        enemyAct,       // 敵行動実行         // プレイヤーの行動場合、行動決めの後すぐに実行
        move,           // 移動               // プレイヤー移動の場合、敵行動決めの後すぐに実行
        trap,           // トラップ動作中      // プレイヤー行動直後に罠にかかっている場合すぐ実行
        turnEnd,
        max
    }

    public enum PlayerActType
    {
        move = 0,   // 移動
        act,        // 行動
        max
    }

    // 今の状態
    public List<ActSeqType> seqList;

    // これがtrueの時、全員がターンエンド状態かを調べる
    private bool IsCheckTurnEnd = false;

    // 自然回復
    const int HealBorder = 150;
    // 回復値
    int cntHeal = 0;

    /// <summary>
    /// プレイヤーの行動を入力すると遷移情報がリストに追加される
    /// </summary>
    /// <param name="actType">プレイヤーの予定</param>
    public void CallAct(PlayerActType actType)
    {
        switch(actType)
        {
            case PlayerActType.move:
                seqList.Add(ActSeqType.requestEnemy);
                seqList.Add(ActSeqType.move);
                seqList.Add(ActSeqType.requestEnemy2);
                seqList.Add(ActSeqType.enemyAct);
                seqList.Add(ActSeqType.turnEnd);
                break;

            case PlayerActType.act:
                seqList.Add(ActSeqType.playerAct);
                seqList.Add(ActSeqType.requestEnemy);
                seqList.Add(ActSeqType.move);
                seqList.Add(ActSeqType.enemyAct);
                seqList.Add(ActSeqType.turnEnd);
                break;
        }
       
    }

    // Start is called before the first frame update
    void Awake()
    { 
        
    }

    private void Start()
    {
        EnemyBase[] enemy = FindObjectsOfType<EnemyBase>();
        foreach(var itr in enemy)
        {
            enemies.Add(itr);
        }
        if (player == null)
        {
            player = FindObjectOfType<PlayerControll>();
        }
        cntHeal = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsCheckTurnEnd)
        {// 全員がターンエンド状態か調べ続ける
            if(this.IsTurnEnd())
            {// 全員がターンエンド状態だった場合

                // 1ターン経過したとみなして、行動可能状態に戻す（全員をwait状態にする）
                this.ResetAct();
            }
        }

        Actor.ChangeSpeed();
    }

    public void AddSeq(ActSeqType type)
    {
        seqList.Insert(0, type);
    }

    /// <summary>
    /// 予約を一つ処理する
    /// 各キャラクター達が自分の番の最後に呼び出す
    /// </summary>
    public void ActProc()
    {
        if (seqList.Count != 0)
        {// 行動予約がある場合

            // 予約の先頭を実行
            switch (seqList[0])
            {
                case ActSeqType.playerAct:
                    // プレイヤー行動
                    seqList.RemoveAt(0);
                    player.Attack();
                    StartCoroutine(this.MoveTimer());
                    break;

                case ActSeqType.requestEnemy:
                    // 敵に行動を判断させる
                    seqList.RemoveAt(0);
                    this.RequestEnemyAI();
                    break;

                case ActSeqType.requestEnemy2:
                    // 敵に行動を判断させる
                    seqList.RemoveAt(0);
                    this.RequestEnemyAI2();
                    break;

                case ActSeqType.move:
                    // 移動
                    seqList.RemoveAt(0);
                    if (player.Move())
                    {
                        this.MoveEnemy();
                        StartCoroutine(this.MoveTimer());
                    }
                    break;

                case ActSeqType.enemyAct:
                    // 敵行動
                    seqList.RemoveAt(0);
                    this.ActEnemy();
                    break;
                case ActSeqType.turnEnd:
                    // ターンエンド処理
                    seqList.RemoveAt(0);
                    cntHeal += player.Param.maxHp;
                    if (HealBorder < cntHeal)
                    {
                        cntHeal -= HealBorder;
                        player.Param.AddHP(1);
                        Actor.Parameter parameter = player.Param;
                        parameter.hp += 1;
                        player.Param = parameter;
                        if (player.Param.hp > player.Param.maxHp)
                        {
                            parameter.hp = player.Param.maxHp;
                            player.Param = parameter;
                        }
                    }
                    IsCheckTurnEnd = true;
                    break;

                case ActSeqType.trap:
                    seqList.RemoveAt(0);
                    break;
            }


        }
        else
        {// 予約が一件もない場合

        }
    }

    public void CheckTurnEnd_Reset()
    {
        if (this.IsTurnEnd())
        {
            this.ResetAct();
        }
    }

    private void MoveEnemy()
    {
        foreach(var itr in enemies)
        {
            itr.MoveProc();
        }
    }

    private void ActEnemy()
    {
        foreach (var itr in enemies)
        {
            itr.ActProc();
        }
        this.ActProc();
    }

    private void RequestEnemyAI()
    {
        foreach(var itr in enemies)
        {// それぞれの敵に行動を判断させる
            itr.DecideCommand();
        }
        this.ActProc();
    }

    private void RequestEnemyAI2()
    {
        foreach (var itr in enemies)
        {// それぞれの敵に行動を判断させる
            itr.DecideCommand2();
        }
        this.ActProc();
    }

    public void ActFailed()
    {
        // プレイヤーが移動に失敗したとき
        // （移動先に敵がいた場合、壁があった場合）

        // 状態をwaitにリセット
        this.ResetAct();

        // 予約リストを消去して次の入力を待つ
        seqList.Clear();
    }

    public void MapDataUpdate_Enemy()
    {
        foreach (var itr in enemies)
        {// 
            itr.MapDataUpdate();
        }
    }

    public void DestroyEnemyFromID(int id)
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {// 逆順ループ
            // ID検索してヒットした敵を消す リストからも
            if (enemies[i].Param.id == id)
            {
                enemies.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 敵をID検索
    /// </summary>
    /// <returns>一致した敵</returns>
    public EnemyBase SearchEnemyFromID(int id)
    {

        foreach(var itr in enemies)
        {
            if(itr.Param.id == id)
            {
                return itr;
            }
        }
        AdDebug.Log("敵のID検索に失敗しました");
        return null;
    }

    // 全員がターンエンド状態か調べる
    public bool IsTurnEnd()
    {
        if(player.status.actType != Actor.ActType.TurnEnd)
        {
            return false;
        }

        foreach (var itr in enemies)
        {
            if (itr.status.actType != Actor.ActType.TurnEnd)
            {
                return false;
            }
        }
        return true;
    }

    public void ResetAct()
    {
        player.status.actType = Actor.ActType.Wait;

        foreach (var itr in enemies)
        {
            itr.status.actType = Actor.ActType.Wait;
        }
    }

    public EnemyBase GetEnemyFromPoint(Point point)
    {
        foreach(var itr in enemies)
        {
            if(itr.status.point == point)
            {
                return itr;
            }
        }
        return null;
    }

    // =--------- // =--------- コルーチン ---------= // ---------= //
    private IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(Actor.MoveTime);

        // 移動後は次の予約を実行
        this.ActProc();
    }

    

    #region singleton

    static SequenceMGR _instance;

    public static SequenceMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(SequenceMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use SequenceMGR in the scene hierarchy.");
                    _instance = (SequenceMGR)previous;
                }
                else
                {
                    var go = new GameObject("SequenceMGR");
                    _instance = go.AddComponent<SequenceMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion

}