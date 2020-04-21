using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Wood : ArrowBase
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void HitEnemy()
    {
        // 敵にダメージを与える
        EnemyBase enemy = SequenceMGR.instance.SearchEnemyFromID(hitObjID);

        SoundMGR.PlaySe("Explosion", 0.5f);

        enemy.Damage(item.Atk, true);

        StartCoroutine(DestroyTimer(0.1f));

        // エフェクト生成
        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Hit_White, enemy.transform.position);
    }

    public override void HitWall()
    {
        // 何も起こらない
        StartCoroutine(DestroyTimer(0.1f));
    }
}
