using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public enum Direct
    {
        right = 0,
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
        public int def;
        public int exp;     // 今まで取得した経験値
        public int xp;      // 倒したとき得られる経験値

        public string Name; // 名前

        public int weaponId;// 装備中武器アイテムID
        public int shieldId;// 装備中盾アイテムID

        public int hunger;      // 満腹度
        public int maxHunger;   // 満腹度（最大値）
        
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

    public virtual void UpdatePosition()
    {
        this.transform.position = MapData.GridToWorld(this.status.point);
    }

    protected void ChangeRotate()
    {
        float rotY = this.transform.rotation.y;

        switch (status.direct)
        {
            case Direct.right: rotY = 90.0f; break;
            case Direct.left: rotY = 270.0f; break;
            case Direct.forward: rotY = 0.0f; break;
            case Direct.back: rotY = 180.0f; break;
            case Direct.right_forward: rotY = 45.0f; break;
            case Direct.left_forward: rotY = 315.0f; break;
            case Direct.right_back: rotY = 135.0f; break;
            case Direct.left_back: rotY = 225.0f; break;
            default: break;
        }

        this.transform.rotation = (Quaternion.Euler(
               this.transform.rotation.x,
               rotY,
               this.transform.rotation.z));
    }

    public void Equip(int itemID, EquipType equipType)
    {
        switch(equipType)
        {
            case EquipType.weapon:
                this.SetWeaponID(itemID);
                EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Player_EquipWeapon, this.status.point);
                break;

            case EquipType.shield:
                this.SetShieldID(itemID);
                EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Player_EquipShield, this.status.point);
                break;
        }

        // 装備音
        SoundMGR.PlaySe("Equip");
    }

    public void RemoveEquip(EquipType equipType)
    {
        switch (equipType)
        {
            case EquipType.weapon: this.SetWeaponID(Parameter.notEquipValue); break;
            case EquipType.shield: this.SetShieldID(Parameter.notEquipValue); break;
        }
    }

    /// <summary>
    /// アイテムの効果を反映させる
    /// </summary>
    /// <param name="itemID">アイテムID</param>
    public void UseItem(int itemID)
    {
        ItemTableEntity item = DataBase.instance.GetItemTableEntity(itemID);
        this.AddHP(item.HP, true);
        this.param.basicAtk += item.Atk;
        this.AddHunger(item.Hunger);
    }

    public virtual void Damage(int damage){}
    public virtual void Damage(int damage, bool isXp) { }

    /// <summary>
    /// 経験値加算（規定値を超えた場合はレベルアップ）
    /// </summary>
    public void AddXp(int addXp)
    {
        // 最大レベルなら上がらない
        if(this.param.level >= 37) { return; }

        // 経験値加算
        this.param.exp += addXp;

        bool isUp = false;

        // 現経験値からレベル最大値まで上げる
        while (true)
        {
            // レベルテーブルから情報取得
            LevelTableEntity levelTableEntity = DataBase.instance.GetLevelTableEntity(this.param.level);
            if (levelTableEntity.Xp <= this.param.exp)
            {
                // レベルアップ
                this.param.level = levelTableEntity.Level;
                this.param.atk = levelTableEntity.Atk;

                // 体力UP
                this.param.maxHp += 3;
                this.param.hp += 3;

                // オーバーしてしまった場合は矯正
                if (this.param.hp > this.param.maxHp) this.param.hp = this.param.maxHp;

                isUp = true;
                continue;
            }

            if (isUp)
            {
                EffectMGR.instance.CreateEffect(EffectMGR.EffectType.LevelUp, this.status.point);
                MessageWindow.instance.AddMessage($"レベル{this.param.level}に上がった！", Color.white);

                // レベルアップ音
                SoundMGR.PlaySe("LevelUp");
            }
            break;
        }
    }

    public void AddLevel(int addLevel)
    {
        // 最大レベルなら上がらない
        if (this.param.level >= 37) { return; }

        this.param.level += addLevel;

        this.param.exp = DataBase.instance.GetLevelTableEntity(this.param.level).Xp;

        MessageWindow.instance.AddMessage($"レベル{this.param.level}に上がった！", Color.white);

        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.LevelUp, this.status.point);
    }
    public void SubLevel(int subLevel)
    {
        this.param.level -= subLevel;

        this.param.exp = DataBase.instance.GetLevelTableEntity(this.param.level).Xp;

        MessageWindow.instance.AddMessage($"レベルが{subLevel}下がってしまった…", Color.white);
    }

    public void AddAtk(int addAtk)
    {
        this.param.atk += addAtk;

        MessageWindow.instance.AddMessage($"ちからが{addAtk}あがった！", Color.white);

        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.PowerUp, this.status.point);
    }
    public void SubAtk(int subAtk)
    {
        this.param.atk -= subAtk;

        MessageWindow.instance.AddMessage($"ちからが{subAtk}さがってしまった…", Color.white);
    }

    public void AddHP(int addHP, bool isEffect)
    {
        this.param.hp += addHP;
        if (this.param.hp > this.param.maxHp) this.param.hp = this.param.maxHp;

        if (isEffect)
        {
            EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Heal, this.status.point);

            // 回復音
            SoundMGR.PlaySe("Heal", 0.4f);
        }
    }
    public bool SubHP(int subHP)
    {
        this.param.hp -= subHP;

        if (this.param.hp <= 0)
        {
            this.param.hp = 0;
            return true;
        }

        return false;
    }
    public void AddMaxHP(int addMaxHP)
    {
        this.param.maxHp += addMaxHP;
    }
    public void SubMaxHP(int subMaxHP)
    {
        this.param.maxHp -= subMaxHP;
    }
    public void AddHunger(int addHunger)
    {
        this.param.hunger += addHunger;
        if (this.param.hunger > this.param.maxHunger) this.param.hunger = this.param.maxHunger;

        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Hunger_Recovery, this.status.point);

        // 満腹度回復音
        SoundMGR.PlaySe("Hunger_Recovery");
    }
    public bool SubHunger(int subHunger)
    {
        this.param.hunger -= subHunger;

        if (this.param.hunger <= 0)
        {
            this.param.hunger = 0;
            return true;
        }

        return false;
    }
    public void AddMaxHunger(int addMaxHunger)
    {
        this.param.maxHunger += addMaxHunger;
    }
    public void SubMaxHunger(int subMaxHunger)
    {
        this.param.maxHunger -= subMaxHunger;
    }

    public void SetShieldID(int itemID)
    {
        this.param.shieldId = itemID;
    }
    public void SetWeaponID(int itemID)
    {
        this.param.weaponId = itemID;
    }

    public bool CheckDestroy()
    {
        // hpが0以下なら死亡
        if (this.param.hp <= 0)
        {
            this.param.hp = 0;
            return true;
        }
        return false;
    }

    /// <summary>
    /// トルネコ式攻撃力計算
    /// </summary>
    /// <returns>整数の攻撃値</returns>
    public virtual int CalcAtk()
    {
        return 0;
    }

    /// <summary>
    /// トルネコ式ダメージ計算
    /// 1を下回った場合、最低1ダメージ
    /// </summary>
    /// <param name="atk">攻撃側の計算後攻撃力</param>
    /// <returns>整数のダメージ値</returns>
    public virtual int CalcDamage(int Atk)
    {
        return 0;
    }

    public virtual void DestroyObject() { }
    public virtual void DestroyObject(bool isXp) { }
}
