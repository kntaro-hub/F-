﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 罠の基底クラス
/// </summary>
public class TrapBase : MapChipBase
{
    public TrapMGR.TrapType trapType;

    protected bool isActived = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// IDを設定する
    /// </summary>
    /// <param name="id">ID番号</param>
    public void SetID(int id)
    {
        base.ID = id;
    }

    /// <summary>
    /// トラップ発動前後処理
    /// </summary>
    public override void ActiveMapChip(Actor actor)
    {
        if(!isActived)
        {
            
        }

        SequenceMGR.instance.AddSeq(SequenceMGR.ActSeqType.trap);

        this.ActiveTrap(actor);


    }

    /// <summary>
    /// 罠起動（罠ごとに効果が違う）
    /// </summary>
    public virtual void ActiveTrap(Actor actor) { }

    public int GetID()
    {
        return ID;
    }
}
