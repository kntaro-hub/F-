using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // アトリビュートを付与
public class WeaponTableEntity
{
    public int      TypeID; // publicでエクセルでインポートしたい型と名前を定義
    public string   Name;
    public int      Atk;
    public int      Buy;
    public int      Sell;
}