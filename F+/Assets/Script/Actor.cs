using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{

    public enum Direct
    {
        right = 1,
        left,
        forward,
        back,
        right_forward,
        left_forward,
        right_back,
        left_back,
        max
    }

    public enum ActType
    {
        Wait, // キー入力待ち。もしくは待機中

        // アクション
        ActBegin, // 開始
        Act,      // 実行中
        ActEnd,   // 終了
        
        // 移動
        MoveBegin, // 開始
        Move,      // 移動中
        MoveEnd,   // 完了

        TurnEnd,   // ターン終了
    };

    public struct Status
    {
        public Direct direct;
        public Point gridPos;  // グリッド座標
        public ActType actType;
    }
    public Status status;

    // 移動にかかる時間
    public const float MoveTime = 0.1f;

    protected virtual void UpdateProc()
    {// 行動時のアップデート
        
    }

    // Start is called before the first frame update
    void Start()
    {
        status.actType = ActType.Wait;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
