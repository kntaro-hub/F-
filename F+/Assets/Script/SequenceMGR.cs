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

    private List<EnemyBase> enemies = new List<EnemyBase>();

    public enum SeqType
    {
        KeyInput = 0,

    }

    // Start is called before the first frame update
    void Awake()
    { 
        if(player == null)
        {
            player = FindObjectOfType<PlayerControll>();
        }
    }

    private void Start()
    {
        EnemyBase[] enemy = FindObjectsOfType<EnemyBase>();
        foreach(var itr in enemy)
        {
            enemies.Add(itr);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            foreach (var itr in enemies)
            {
                itr.AStar.SetGoal(Player.GetPoint());
                itr.status.gridPos = itr.AStar.A_StarProc_Single2();
                itr.transform.DOMove(FieldMGR.GridToWorld(itr.status.gridPos), 0.1f);
            }
        }
    }

    // 敵はプレイヤーとの距離を測り、
    // それによって攻撃or行動or移動を分ける


    /// <summary>
    /// プレイヤーから呼ばれ、
    /// プレイヤーの行動に合わせて
    /// この関数から敵に指示を出す。
    /// </summary>
    public void ActProc()
    {
        switch(player.GetAct)
        {
            case Actor.ActType.Act:    // プレイヤーが行動開始
                this.MoveEnemy();
                break;

            case Actor.ActType.Move:   // プレイヤーが移動開始
                this.MoveEnemy();
                break;
        }
    }

    private void MoveEnemy()
    {
        foreach(var itr in enemies)
        {
            itr.AStar.SetGoal(Player.GetPoint());
            itr.status.gridPos = itr.AStar.A_StarProc_Single();
            itr.transform.DOMove(FieldMGR.GridToWorld(itr.status.gridPos), 0.1f);
        }
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