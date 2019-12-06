using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // アトリビュートを付与
public class LevelTableEntity
{
    public int  Level; // publicでエクセルでインポートしたい型と名前を定義
    public int  BasicAtk;
    public int  Xp; 
}