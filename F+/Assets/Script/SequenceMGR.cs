using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private List<EnemyBase> enemies = null;

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

    // Update is called once per frame
    void Update()
    {

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
            case Actor.ActType.ActBegin:    // プレイヤーが行動開始
                
                break;

            case Actor.ActType.MoveBegin:   // プレイヤーが移動開始
                
                break;
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