using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MagicBase : MonoBehaviour
{
    public enum MagicType
    {
        shot = 0,   // 射撃系
        status,     // 変化系
        max
    }
    // 魔法系統
    protected MagicType magicType;

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

    // 使用回数
    protected int ammo;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAmmo()
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

    public void ActivateMagic()
    {
        switch (magicType)
        {
            case MagicType.shot:
                // プレイヤーの方向にむかって飛ぶ
                Actor player = SequenceMGR.instance.Player;
                Point playerPoint = player.status.point;
                Point movedPoint = new Point();
                item = DataBase.instance.GetItemTableEntity(itemID);

                // 魔法の位置をプレイヤーに合わせる
                this.transform.position = MapData.GridToWorld(playerPoint);

                switch (SequenceMGR.instance.Player.status.direct)
                {
                    case Actor.Direct.right: movedPoint.x = 1; break;
                    case Actor.Direct.left: movedPoint.x = -1; break;
                    case Actor.Direct.forward: movedPoint.y = 1; break;
                    case Actor.Direct.back: movedPoint.y = -1; break;

                    case Actor.Direct.right_forward: movedPoint.x = 1; movedPoint.y = 1; break;
                    case Actor.Direct.left_forward: movedPoint.x = -1; movedPoint.y = 1; break;
                    case Actor.Direct.right_back: movedPoint.x = 1; movedPoint.y = -1; break;
                    case Actor.Direct.left_back: movedPoint.x = -1; movedPoint.y = -1; break;
                }

                while (true)
                {
                    // 当たったあとの座標
                    Point hitedPoint = playerPoint + movedPoint;

                    bool isHit = false;

                    if (MapData.instance.GetMapChipType(hitedPoint) == MapData.MapChipType.wall) isHit = true;
                    if (MapData.instance.GetMapObject(hitedPoint).objType == MapData.MapObjType.enemy) isHit = true;

                    if (!isHit)
                    {// 1マス先へ
                        if (movedPoint.x != 0) movedPoint.x += (int)Mathf.Sign((float)movedPoint.x);
                        if (movedPoint.y != 0) movedPoint.y += (int)Mathf.Sign((float)movedPoint.y);
                    }
                    else
                    {
                        // 壁or敵にぶつかったら1マス下がる
                        if (movedPoint.x != 0) movedPoint.x -= (int)Mathf.Sign((float)movedPoint.x);
                        if (movedPoint.y != 0) movedPoint.y -= (int)Mathf.Sign((float)movedPoint.y);

                        // あたる直前の座標
                        Point hitPoint = playerPoint + movedPoint;

                        // メッセージ表示
                        MessageWindow.instance.AddMessage($"{player.Param.Name}は{item.Name}をふった！", Color.white);

                        float moveTime = speed * (int)Mathf.Max(Mathf.Abs((float)(playerPoint.x - (playerPoint.x + movedPoint.x))), Mathf.Abs((float)(playerPoint.y - (playerPoint.y + movedPoint.y))));

                        this.transform.DOMove(MapData.GridToWorld(hitedPoint), moveTime);

                        this.SetTime(moveTime);
                        this.SetHitPoint(hitPoint);
                        this.SetItemInfo(item);

                        MapData.ObjectOnTheMap mapObject = MapData.instance.GetMapObject(hitedPoint);

                        this.SetHitObjID(mapObject.id);

                        StartCoroutine(this.ShotTimer(moveTime, mapObject.objType));

                        SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;
                        break;
                    }
                }
                break;

            case MagicType.status:
                break;
        }
}

    /// <summary>
    /// 魔法発動（敵に当たった場合）
    /// </summary>
    public virtual void MagicEffect_HitEnemy() { }

    /// <summary>
    /// 魔法発動（壁に当たった場合）
    /// </summary>
    public virtual void MagicEffect_HitWall() { }

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

    private IEnumerator ShotTimer(float time, MapData.MapObjType objType)
    {
        yield return new WaitForSeconds(time);

        if (objType == MapData.MapObjType.enemy)
        {// 敵に当たった場合
            this.MagicEffect_HitEnemy();
        }
        else
        {// 敵以外に当たった場合
            this.MagicEffect_HitWall();
        }
    }
}
