using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Magic_Tornado : MagicBase
{
    // ワープが終わるまでの時間
    private const float WarpTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        magicType = MagicType.shot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void MagicEffect_HitEnemy()
    {
        // 敵にダメージを与える
        EnemyBase enemy = SequenceMGR.instance.SearchEnemyFromID(hitObjID);

        SoundMGR.PlaySe("Tornado", 0.7f);

        StartCoroutine(WarpTimer(enemy));

        SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;
    }

    public override void MagicEffect_HitWall()
    {
        // 何も起こらない
        this.Destroy();

        SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;
    }

    // =--------- コルーチン ---------= //
    private IEnumerator WarpTimer(Actor actor)
    {
        // =--------- 飛び上がる直前 ---------= //

        // キャラクターが飛び上がる
        actor.transform.DOLocalMoveY(10.0f, WarpTime * 0.5f);

        // キャラクターの位置のマップ情報をリセット
        MapData.instance.ResetMapObject(actor.status.movedPoint);

        // その間はプレイヤーの入力は無視する
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;

        // エフェクト生成
       EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Wand_Tornado_Hit, actor.transform.position).transform.rotation = Quaternion.Euler(-90.0f,0.0f,0.0f);

        yield return new WaitForSeconds(WarpTime * 0.5f);

        // =--------- 飛び上がった頂点位置 ---------= //

        // 元の位置に降りる
        actor.transform.DOLocalMoveY(0.0f, WarpTime * 0.5f);

        // 座標をワープ後の座標に変更
        // 先にマップ上座標をワープ後の座標に変更
        Point warpedPoint = MapGenerator.instance.RandomPointInRoom();
        actor.status.point = warpedPoint;
        actor.status.movedPoint = warpedPoint;
        Vector3 warpedPos = MapData.GridToWorld(warpedPoint);
        actor.transform.position = new Vector3(warpedPos.x, actor.transform.position.y, warpedPos.z);

        yield return new WaitForSeconds(WarpTime * 0.5f);

        // =--------- 完全に降りた瞬間 ---------= //

        // プレイヤーの入力を許可する
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;

        // ワープ後座標にキャラクターを登録
        MapData.instance.SetMapObject(actor.status.point, MapData.MapObjType.enemy, actor.Param.id);

        // マップを更新
        UI_MGR.instance.Ui_Map.UpdateMap();

        this.Destroy();
    }
}
