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

    /// <summary>
    /// キャラクターの種類
    /// </summary>
    public enum CharaType
    {
        player = 0,
        enemy,
        max
    }

    public struct Status
    {
        public Direct direct;
        public Point point;  // グリッド座標
        public Point movedPoint;
        public ActType actType;
        public CharaType characterType;
    }
    public Status status;

    // キャラクターパラメータ
    [System.Serializable]
    public struct Parameter
    {
        public int id;      // ユニークID
        public int level;   // レベル
        public int hp;      // 体力
        public int maxHp;   // 体力（最大値）
        public int basicAtk;// 基本攻撃力（レベルで上昇）
        public int maxAtk;  // ちから最大値
        public int atk;     // ちから
        public int exp;     // 今まで取得した経験値
        public int xp;      // 倒したとき得られる経験値

        public string Name; // 名前

        public int weaponId;// 装備中武器アイテムID
        public int shieldId;// 装備中盾アイテムID

        public int hunger;      // 満腹度
        public int maxHunger;   // 満腹度（最大値）

        /// <summary>
        /// トルネコ式攻撃力計算
        /// </summary>
        /// <returns>整数の攻撃値</returns>
        public int CalcAtk()
        {
            // Atk計算                                                                                      // 力の初期値
            float WeaponAtk;
            if (this.weaponId != DataBase.instance.GetItemTableCount() - 1)
            {
                WeaponAtk = (this.basicAtk * (DataBase.instance.GetItemTable(this.weaponId).Atk + this.atk - 8.0f) / 16.0f);
            }
            else // なにも装備していない場合
            {
                WeaponAtk = (this.basicAtk + (this.atk - 8.0f) / 16.0f);
            }
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
            if (this.shieldId != DataBase.instance.GetItemTableCount() - 1)
            {
                // 防御力計算
                Atk = (int)(Atk * Mathf.Pow((15.0f / 16.0f), DataBase.instance.GetItemTable(this.weaponId).Def));  // 攻撃力と基本ダメージ
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

        /// <summary>
        /// 経験値加算（規定値を超えた場合はレベルアップ）
        /// </summary>
        public void AddXp(int addXp)
        {
            // 経験値加算
            this.exp += addXp;

            bool isUp = false;

            // 現経験値からレベル最大値まで上げる
            while (true)
            {
                // レベルテーブルから情報取得
                LevelTableEntity levelTableEntity = DataBase.instance.GetLevelTable(level);
                if(levelTableEntity.Xp <= this.exp)
                {
                    // レベルアップ
                    this.level = levelTableEntity.Level;
                    this.atk = levelTableEntity.atk;

                    // 体力UP
                    this.maxHp += 3;
                    this.hp += 3;

                    // オーバーしてしまった場合は矯正
                    if (this.hp > this.maxHp) this.hp = this.maxHp;
                    

                    isUp = true;
                    continue;
                }

                if(isUp)
                {
                    MessageWindow.instance.AddMessage($"レベル{this.level}に上がった！", Color.white);
                }
                break;
            }
        }

        public void AddLevel(int addLevel)
        {
            this.level += addLevel;

            this.exp = DataBase.instance.GetLevelTable(level).Xp;

            MessageWindow.instance.AddMessage($"レベル{this.level}に上がった！", Color.white);
        }
        public void SubLevel(int subLevel)
        {
            this.level -= subLevel;

            this.exp = DataBase.instance.GetLevelTable(level).Xp;

            MessageWindow.instance.AddMessage($"レベルが{subLevel}下がってしまった…", Color.white);
        }
        public void AddHP(int addHP)
        {
            this.hp += addHP;
            if (this.hp > this.maxHp) this.hp = this.maxHp;
        }
        public bool SubHP(int subHP)
        {
            this.hp -= subHP;

            if (hp <= 0)
            {
                hp = 0;
                return true;
            }

            return false;
        }
        public void AddMaxHP(int addMaxHP)
        {
            this.maxHp += addMaxHP;
        }
        public void SubMaxHP(int subMaxHP)
        {
            this.maxHp -= subMaxHP;
        }
        public void AddHunger(int addHunger)
        {
            this.hunger += addHunger;
            if (this.hunger > this.maxHunger) hunger = maxHunger;
        }
        public bool SubHunger(int subHunger)
        {
            this.hunger -= subHunger;

            if (hunger <= 0)
            {
                hunger = 0;
                return true;
            }

            return false;
        }
        public void AddMaxHunger(int addMaxHunger)
        {
            this.maxHunger += addMaxHunger;
        }
        public void SubMaxHunger(int subMaxHunger)
        {
            this.maxHunger -= subMaxHunger;
        }

        public bool CheckDestroy()
        {
            // hpが0以下なら死亡
            if (this.hp <= 0)
            {
                this.hp = 0;
                return true;
            }
            return false;
        }
        
        public void SetShieldID(int itemID)
        {
            this.shieldId = itemID;
        }
        public void SetWeaponID(int itemID)
        {
            this.weaponId = itemID;
        }

        // 非装備時武器盾ID
        public static int notEquipValue = DataBase.instance.GetItemTableCount() - 1;
    }
    protected Parameter param;
    public Parameter Param
    {
        get { return param; }
        set { param = value; }
    }

    // 移動にかかる時間
    public static float MoveTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        status.actType = ActType.Wait;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void ChangeSpeed()
    {
        if (Input.GetKey(KeyCode.D))
        {
            MoveTime = 0.01f;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            MoveTime = 0.1f;
        }
    }
    
    public void Equip(int itemID, EquipType equipType)
    {
        switch(equipType)
        {
            case EquipType.weapon: this.param.SetWeaponID(itemID); break;
            case EquipType.shield: this.param.SetShieldID(itemID); break;
        }
    }

    public void RemoveEquip(EquipType equipType)
    {
        switch (equipType)
        {
            case EquipType.weapon: this.param.SetWeaponID(Parameter.notEquipValue); break;
            case EquipType.shield: this.param.SetShieldID(Parameter.notEquipValue); break;
        }
    }

    /// <summary>
    /// アイテムの効果を反映させる
    /// </summary>
    /// <param name="itemID">アイテムID</param>
    public void UseItem(int itemID)
    {
        ItemTableEntity item = DataBase.instance.GetItemTable(itemID);
        this.param.AddHP(item.HP);
        this.param.basicAtk += item.Atk;
        this.param.AddHunger(item.Hunger);
    }

    public virtual void Damage(int damage){}
    public virtual void Damage(int damage, bool isXp) { }

    public virtual void DestroyObject() { }
    public virtual void DestroyObject(bool isXp) { }
}
