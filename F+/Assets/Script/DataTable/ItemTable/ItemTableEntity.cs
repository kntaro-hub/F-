[System.Serializable] // アトリビュートを付与
public class ItemTableEntity
{
    public int      ID;             // アイテムID
    public ItemType Type;           // アイテムタイプ
    public string   Name;           // 名前
    public int      HP;             // HP回復値
    public int      Hunger;         // 満腹度回復値
    public int      Atk;            // 攻撃力
    public int      Def;            // 防御力
    public string   Ex;             // 追加効果  
    public int      ExValue;        // 追加効果値
    public int      Buy;            // 買値
    public int      Sell;           // 売値
    public int      ExType;         // 追加タイプ（魔法の種類とか）
    public string   Detail;         // 説明文
    public string   UseMessage;     // 使った時の文
    public string   UsedMessage;    // 使った後の文
}

// アイテムジャンル
public enum ItemType
{
    Consumables = 0, // 消耗品
    Weapon,
    Shield,
    Magic,
    max
}