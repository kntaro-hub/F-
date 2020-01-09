using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // アトリビュートを付与
public class EnemyTableEntity
{
    public int      TypeID;         // 種類
    public string   Name;           // 名前
    public int      MaxHP;          // 体力
    public int      Atk;            // 攻撃力
    public int      Xp;             // 経験値
    public int      DropPer;        // アイテムドロップ率
    public int      MinFloor;       // 出現する最低階層
    public int      MaxFloor;       // 出現する最大階層
    public int      Appearance;     // 出現しやすさ
}