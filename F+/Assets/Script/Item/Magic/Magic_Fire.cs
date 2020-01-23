﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Magic_Fire : MagicBase
{
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

        enemy.Damage(item.Atk, true);

        StartCoroutine(DestroyTimer(0.1f));

        // エフェクト生成
        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Wand_Fire_Hit, enemy.transform.position);
    }

    public override void MagicEffect_HitWall()
    {
        // 何も起こらない
        this.Destroy();

        SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;
    }

    
}
