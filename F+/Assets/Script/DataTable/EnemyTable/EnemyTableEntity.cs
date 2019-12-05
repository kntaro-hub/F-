using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // アトリビュートを付与
public class EnemyTableEntity
{
    public int      TypeID; // publicでエクセルでインポートしたい型と名前を定義
    public string   Name;
    public int      MaxHP; 
    public int      Atk;
    public int      Def;
    public int      Xp;
}