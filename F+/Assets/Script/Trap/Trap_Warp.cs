using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Trap_Warp : TrapBase
{
    private const float WarpTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

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
        StartCoroutine(WarpTimer(actor));
    }

    // =--------- コルーチン ---------= //
    private IEnumerator WarpTimer(Actor actor)
    {
        // =--------- 飛び上がる直前 ---------= //

        // 乗ったキャラクターが飛び上がる
        actor.transform.DOLocalMoveY(10.0f, WarpTime * 0.5f);

        // 罠起動音
        SoundMGR.PlaySe("Trap_Warp");

        // キャラクターの位置のマップ情報をリセット
        MapData.instance.ResetMapObject(actor.status.movedPoint);

        // その間はプレイヤーの入力は無視する
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;

        // エフェクト
        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Trap_Warp, point).transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

        yield return new WaitForSeconds(WarpTime * 0.5f);

        // =--------- 飛び上がった頂点位置 ---------= //

        // 元の位置に降りる
        actor.transform.DOLocalMoveY(0.0f, WarpTime * 0.5f);

        // プレイヤーの座標をワープ後の座標に変更
        // 先にマップ上プレイヤーの座標をワープ後の座標に変更
        Point warpedPoint = MapGenerator.instance.RandomPointInRoom();
        actor.status.point = warpedPoint;
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
