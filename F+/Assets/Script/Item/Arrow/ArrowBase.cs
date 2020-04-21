using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArrowBase : MonoBehaviour
{
    // 魔法のユニークなID（これはマネージャから割り当てられる）
    protected int uniqueID = 0;

    // 速度
    protected float speed = 0.04f;

    // アイテムID
    protected int itemID = 0;

    // 当たった座標
    protected Point hitPoint;

    // 飛ぶ速度
    protected float shotTime;

    // 矢情報
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
        ArrowMGR.instance.Destroy(this.uniqueID);
    }

    public void ActivateArrow()
    {
          // プレイヤーの方向にむかって飛ぶ
          Actor player = SequenceMGR.instance.Player;
          Point playerPoint = player.status.point;
          Point movedPoint = new Point();
          item = DataBase.instance.GetItemTableEntity(itemID);

          // 魔法の位置をプレイヤーに合わせる
          this.transform.position = MapData.GridToWorld(playerPoint);

          movedPoint = MapData.DirectPoints[(int)SequenceMGR.instance.Player.status.direct];

        float rotY = 0.0f;
        switch(SequenceMGR.instance.Player.status.direct)
        {
            case Actor.Direct.left: rotY = 0.0f;  break;
            case Actor.Direct.right: rotY = 180.0f; break;
            case Actor.Direct.forward: rotY = 90.0f; break;
            case Actor.Direct.back: rotY = -90.0f; break;
            case Actor.Direct.left_forward: rotY = 45.0f; break;
            case Actor.Direct.left_back: rotY = -45.0f; break;
            case Actor.Direct.right_forward: rotY = 135.0f; break;
            case Actor.Direct.right_back: rotY = -135.0f; break;
        }

        this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, rotY , this.transform.rotation.z); 

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
                  MessageWindow.instance.AddMessage($"{player.Param.Name}は{item.Name}をはなった！", Color.white);

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
    }

    /// <summary>
    /// 敵に当たった場合
    /// </summary>
    public virtual void HitEnemy() {  }

    /// <summary>
    /// 魔法発動（壁に当たった場合）
    /// </summary>
    public virtual void HitWall() {  }

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

        SequenceMGR.instance.CallAct(SequenceMGR.PlayerActType.Item);
        SequenceMGR.instance.ActProc();
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;
    }

    private IEnumerator ShotTimer(float time, MapData.MapObjType objType)
    {
        yield return new WaitForSeconds(time);

        if (objType == MapData.MapObjType.enemy)
        {// 敵に当たった場合
            this.HitEnemy();
        }
        else
        {// 敵以外に当たった場合
            this.HitWall();
        }
    }
}
