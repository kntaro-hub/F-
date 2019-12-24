using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Magic_Fire : MagicBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ActivateMagic()
    {
        // プレイヤーの方向にむかって飛ぶ
        Actor player            = SequenceMGR.instance.Player;
        Point playerPoint       = player.status.point;
        Point movedPoint        = new Point();
        ItemTableEntity item    = DataBase.instance.GetItemTable(itemID);

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

                float moveTime = Speed * (int)Mathf.Max(Mathf.Abs((float)(playerPoint.x - (playerPoint.x + movedPoint.x))), Mathf.Abs((float)(playerPoint.y - (playerPoint.y + movedPoint.y))));

                this.transform.DOMove(MapData.GridToWorld(hitedPoint), moveTime);

                MapData.ObjectOnTheMap mapObject = MapData.instance.GetMapObject(hitedPoint);
                if (mapObject.objType == MapData.MapObjType.enemy)
                {// 敵に当たった場合
                    StartCoroutine(this.DestroyTimer(moveTime, hitPoint));

                    // 敵にダメージを与える
                    EnemyBase enemy = SequenceMGR.instance.SearchEnemyFromID(mapObject.id);

                    enemy.Damage(item.Atk, true);
                    
                }
                else
                {// 敵以外に当たった場合
                    StartCoroutine(this.DestroyTimer(moveTime, hitPoint));
                }

                // ウィンドウを閉じる
                UI_MGR.instance.ReturnAllUI();

                SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;
                break;
            }
        }
    }

    private IEnumerator DestroyTimer(
        float time,
        Point point)
    {
        yield return new WaitForSeconds(time);

        this.Destroy();

        SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;
    }
}
