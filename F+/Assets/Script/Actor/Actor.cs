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

    /// <summary>
    /// アクタの状態
    /// </summary>
    public enum ActType
    {
        Wait = 0,
        Act,      // 実行中    
        Move,      // 移動中
        Damage,    // ダメージ中
        TurnEnd,   // ターン終了
    };

    public struct Status
    {
        public Direct direct;
        public Point gridPos;  // グリッド座標
        public ActType actType;
    }
    public Status status;

    // キャラクターパラメータ
    public struct Parameter
    {
        public int id;      // ユニークID
        public int level;   // レベル
        public int hp;      // 体力
        public int maxHp;   // 体力（最大値）
        public int atk;     // 攻撃力
        public int def;     // 防御力
        public int exp;     // 今まで取得した経験値
        public int xp;      // 倒したとき得られる経験値

        /// <summary>
        /// トルネコ式ダメージ計算
        /// Aの攻撃力 * (0.9375 ^ Bの防御力)
        /// 1を下回った場合、最低1ダメージ
        /// </summary>
        /// <param name="atk">攻撃側の攻撃力</param>
        /// <returns>整数のダメージ値</returns>
        public int CalcDamage(int atk)
        {
            // ダメージ計算
            int damage = (int)(atk * Mathf.Pow(0.9375f, this.def));
            if (damage < 1)
            {// 計算結果が1を下回った場合

                // 最低でも1ダメージ
                damage = 1;
            }

            // 計算結果を返す
            return damage;
        }
    }
    protected Parameter param;
    public Parameter Param
    {
        get { return param; }
        set { param = value; }
    }
    public Parameter GetParam()
    {
        return param;
    }

    // 移動にかかる時間
    public const float MoveTime = 0.1f;

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
