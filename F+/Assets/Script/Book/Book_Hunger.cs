using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book_Hunger : BookBase
{
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

        SequenceMGR.instance.Player.AddHunger(100);

        MessageWindow.instance.AddMessage($"まほうのちからでおなかがふくれた！", Color.white);

        // エフェクト生成
        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Hunger_Recovery, SequenceMGR.instance.Player.transform.position);
    }
}