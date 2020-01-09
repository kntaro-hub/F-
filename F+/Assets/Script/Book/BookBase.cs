using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookBase : MonoBehaviour
{
    // 魔法のユニークなID（これはマネージャから割り当てられる）
    protected int uniqueID = 0;

    // 速度
    protected float speed = 0.04f;

    // アイテムID
    protected int itemID = 0;

    // 魔法が当たった座標
    protected Point hitPoint;

    // 魔法が飛ぶ速度
    protected float shotTime;

    // 杖情報
    protected ItemTableEntity item;

    // ヒットしたオブジェクトのID
    protected int hitObjID;

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
        BookMGR.instance.Destroy(this.uniqueID);
    }

    public virtual void ActivateBook()
    {
        // 本の効果発動
    }

    protected void SetHitPoint(Point hitPoint)
    {
        this.hitPoint = hitPoint;
    }

    protected void SetTime(float shotSpeed)
    {
        this.shotTime = shotSpeed;
    }

    protected void SetItemInfo(ItemTableEntity itemEntity)
    {
        item = itemEntity;
    }

    protected void SetHitObjID(int id)
    {
        hitObjID = id;
    }

    protected IEnumerator DestroyTimer(
        float time)
    {
        yield return new WaitForSeconds(time);

        this.Destroy();

        SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;
    }
}
