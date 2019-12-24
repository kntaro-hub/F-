using UnityEngine;

public class MagicBase : MonoBehaviour
{
    // 魔法のユニークなID（これはマネージャから割り当てられる）
    protected int uniqueID = 0;

    // 速度
    protected float Speed = 0.04f;

    // アイテムID
    protected int itemID = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetID()
    {
        return uniqueID;
    }

    public void SetID(int id)
    {
        uniqueID = id;
    }

    public void SetItemID(int id)
    {
        itemID = id;
    }

    /// <summary>
    /// 魔法消去
    /// </summary>
    protected void Destroy()
    {
        // マネージャに削除してもらう
        MagicMGR.instance.Destroy(this.uniqueID);
    }

    /// <summary>
    /// 魔法発動
    /// </summary>
    public virtual void ActivateMagic() { }
}
