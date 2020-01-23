using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMGR : MonoBehaviour
{
    public enum EffectType
    {
        // =--------- パラメータ変化系 ---------= //
        Heal = 0,           // 体力回復した時
        Hunger_Recovery,    // 満腹度回復した時:
        PowerUp,            // ちからアップ:
        LevelUp,            // レベルアップ:

        // =--------- プレイヤー回り ---------= //
        Player_EquipWeapon, // 武器装備時:
        Player_EquipShield, // 盾装備時:CFX2_PickupDiamond2
        //Player_Walk_Smoke,  // 歩いているときの煙:
        Player_Attack_Hit,  // 攻撃ヒット:CFX_MagicPoof
        Player_Dead,        // プレイヤー死亡:

        // =--------- 杖 ---------= //
        //Wand_Fire, // 炎の杖：
        Wand_Fire_Hit, // ヒット時:CFX3_Fire_Explosion

        //Wand_Tornado, // 竜巻の杖:
        Wand_Tornado_Hit,// ヒット時:CFX_Tornado

        // =--------- 本 ---------= //
        Book_Fire, // 爆発の書:CFX2_WWExplosion_C
        Book_Hunger, // 満腹の書:

         // 帰還の書:CFX3_ResurrectionLight_Circle

        // =--------- 罠 ---------= //
        //Trap_Spike_Hit,     // トゲ罠ヒット:CFX_Hit_C White
        //Trap_Hunger_Hit,    // ハラヘリ罠ヒット:
        Trap_EnemySpawn_Hit,// 呼び寄せの罠ヒット:けむり
        Trap_Pitfall,       // 落とし穴の罠; CFX3_Vortex_Ground 回転しながら床に沈んでいく
        Trap_Warp,          // ワープの罠:

        // =--------- 敵 ---------= //j
        //Enemy_Attack_Hit,   // 敵の攻撃ヒット:CFX_Hit_C White
        Enemy_Dead,         // 敵死亡:CFX2_EnemyDeathSkull

        // =--------- 汎用 ---------= //
        Hit_White,

        max
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject CreateEffect(EffectType type, Point point)
    {
        GameObject effect = LoadAssets.instance.GetEffectDictionary(type);
        if (effect != null)
            return Instantiate(effect, MapData.GridToWorld(point), Quaternion.identity);
        else return null;
    }

    public GameObject CreateEffect(EffectType type, Vector3 position)
    {
        GameObject effect = LoadAssets.instance.GetEffectDictionary(type);
        if (effect != null)
            return Instantiate(effect, position, Quaternion.identity);
        else return null;
    }

    public GameObject CreateEffect(EffectType type, Point point, Quaternion rot)
    {
        GameObject effect = LoadAssets.instance.GetEffectDictionary(type);
        if (effect != null)
            return Instantiate(effect, MapData.GridToWorld(point), rot);
        else return null;
    }

    public GameObject CreateEffect(EffectType type, Vector3 position, Quaternion rot)
    {
        GameObject effect = LoadAssets.instance.GetEffectDictionary(type);
        if (effect != null)
            return Instantiate(effect, position, rot);
        else return null;
    }

    //public GameObject CreateEffect(EffectType type, Point point)
    //{
    //    return Instantiate(EffectPrefabs[(int)type], MapData.GridToWorld(point), Quaternion.identity);
    //}

    //public GameObject CreateEffect(EffectType type, Vector3 position)
    //{
    //    return Instantiate(EffectPrefabs[(int)type], position, Quaternion.identity);
    //}

    //public GameObject CreateEffect(EffectType type, Point point, Quaternion rot)
    //{
    //    return Instantiate(EffectPrefabs[(int)type], MapData.GridToWorld(point), rot);
    //}

    //public GameObject CreateEffect(EffectType type, Vector3 position, Quaternion rot)
    //{
    //    return Instantiate(EffectPrefabs[(int)type], position, rot);
    //}

    #region singleton

    static EffectMGR _instance;

    public static EffectMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(EffectMGR));
                if (previous)
                {
                    _instance = (EffectMGR)previous;
                }
                else
                {
                    var go = new GameObject("EffectMGR");
                    _instance = go.AddComponent<EffectMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
