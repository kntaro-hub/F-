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
        public int basicAtk;// 基本攻撃力
        public int atk;     // 攻撃力
        public int exp;     // 今まで取得した経験値
        public int xp;      // 倒したとき得られる経験値

        public int weaponId;// 装備中武器ID
        public int shieldId;// 装備中盾ID

        public int hunger;      // 満腹度
        public int maxHunger;   // 満腹度（最大値）

        /// <summary>
        /// トルネコ式攻撃力計算
        /// </summary>
        /// <returns>整数の攻撃値</returns>
        public int CalcAtk()
        {
            // Atk計算                                                                                      // 力の初期値

            float WeaponAtk = (this.basicAtk * (DataBase.instance.GetWeaponTable(this.weaponId).Atk + this.atk - 8.0f) / 16.0f);

            int Atk = (int)(this.basicAtk * Mathf.Round(WeaponAtk));
            // 計算結果を返す
            return Atk;
        }

        /// <summary>
        /// トルネコ式ダメージ計算
        /// 1を下回った場合、最低1ダメージ
        /// </summary>
        /// <param name="atk">攻撃側の計算後攻撃力</param>
        /// <returns>整数のダメージ値</returns>
        public int CalcDamage(int Atk)
        {
            // 防御力計算
            Atk = (int)(Atk * Mathf.Pow((15.0f / 16.0f), DataBase.instance.GetShiledTable(this.weaponId).Def));  // 攻撃力と基本ダメージ
            Atk = (int)Mathf.Floor(Atk * Random.Range(112, 143) / 128);   // 結果

            if (Atk < 1)
            {// 計算結果が1を下回った場合

                // 最低でも1ダメージ
                Atk = 1;
            }

            // 計算結果を返す
            return Atk;
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
