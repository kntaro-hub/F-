using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book_Explosion : BookBase
{
    private const float Range = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ActivateBook()
    {
        MessageWindow.instance.AddMessage($"{SequenceMGR.instance.Player.Param.Name}は{DataBase.instance.GetItemTableEntity(itemID).Name}をよんだ！", Color.white);

        this.transform.position = SequenceMGR.instance.Player.transform.position;

        bool isDestroy = false;

        var enemies = SequenceMGR.instance.Enemies;
        for (int i = enemies.Count - 1; i >= 0; i--)
        {// 逆順ループ
            float Dist = (enemies[i].transform.position - this.transform.position).sqrMagnitude;

            if (Dist <= Range)
            {
                enemies[i].Damage(100, true);
                isDestroy = true;
            }
        }

        // エフェクト生成
        GameObject effect = EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Book_Fire, this.transform.position);
        effect.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        effect.transform.localScale = new Vector3(6.0f, 6.0f, 6.0f);

        if (!isDestroy)
        {
            MessageWindow.instance.AddMessage($"しかしなにもおこらなかった…", Color.white);
        }
    }
}
