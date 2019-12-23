using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Trap_Warp : TrapBase
{
    private const float WarpTime = 1.0f;
    private Actor actor;

    // Start is called before the first frame update
    void Start()
    {
        trapType = TrapType.Warp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// トラップ起動
    /// </summary>
    public override void ActiveTrap(Actor actor)
    {
        SequenceMGR.instance.AddSeq(SequenceMGR.ActSeqType.trap);

        Point warpedPoint = MapGenerator.instance.RandomPointInRoom();
        actor.status.point = warpedPoint;

        StartCoroutine(WarpTimer(actor, warpedPoint));
    }

    // =--------- コルーチン ---------= //
    private IEnumerator WarpTimer(Actor actor, Point warpedPoint)
    {
        // =--------- 飛び上がる直前 ---------= //

        // 乗ったキャラクターが飛び上がる
        actor.transform.DOLocalMoveY(10.0f, WarpTime * 0.5f);

        // キャラクターの位置のマップ情報をリセット
        MapData.instance.ResetMapObject(actor.status.movedPoint);

        // その間はプレイヤーの入力は無視する
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;

        yield return new WaitForSeconds(WarpTime * 0.5f);

        // =--------- 飛び上がった頂点位置 ---------= //

        // 元の位置に降りる
        actor.transform.DOLocalMoveY(0.0f, WarpTime * 0.5f);

        // プレイヤーの座標をワープ後の座標に変更
        // 先にマップ上プレイヤーの座標をワープ後の座標に変更
        Vector3 warpedPos = MapData.GridToWorld(warpedPoint);
        actor.transform.position = new Vector3(warpedPos.x, actor.transform.position.y, warpedPos.z);

        yield return new WaitForSeconds(WarpTime * 0.5f);

        // =--------- 完全に降りた瞬間 ---------= //

        // プレイヤーの入力を許可する
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;

        // ワープ後座標にキャラクターを登録
        switch(actor.status.characterType)
        {
            // プレイヤーの場合
            case Actor.CharaType.player:
                MapData.instance.SetMapObject(actor.status.point, MapData.MapObjType.player, actor.Param.id);
                break;

            // 敵の場合
            case Actor.CharaType.enemy:
                MapData.instance.SetMapObject(actor.status.point, MapData.MapObjType.enemy, actor.Param.id);
                break;
        }

        

        SequenceMGR.instance.ActProc();
    }
}
