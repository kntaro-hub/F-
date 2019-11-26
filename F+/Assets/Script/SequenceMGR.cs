using System.Collections;
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
    public List<EnemyBase> Enemies
    {
        get { return enemies; }
    }

    public enum SeqType
    {
        action,     // 行動可
        menu,       // メニュー表示
        max
    }

    // 今の状態
    public SeqType seqType;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckDestroy()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].isDestroy)
            {
                Destroy(enemies[i].gameObject);
                enemies.RemoveAt(i);
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
            itr.MoveProc(player.GetPoint());
        }
    }

    // 全員がターンエンド状態か調べる
    public bool IsTurnEnd()
    {
        if(player.status.actType != Actor.ActType.TurnEnd)
        {
            return false;
        }

        foreach(var itr in enemies)
        {
            if(itr.status.actType != Actor.ActType.TurnEnd)
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
            if(itr.status.gridPos == point)
            {
                return itr;
            }
        }
        return null;
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