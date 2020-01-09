﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Trap_Spike : TrapBase
{
    private const int spikeDamage = 10;
    private const float DamageTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        trapType = TrapType.Spike;
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
        MessageWindow.instance.AddMessage($"{actor.Param.Name}はトゲの罠にかかった！", Color.white);
        
        actor.Damage(spikeDamage);

        StartCoroutine(this.SpikeDamageTimer());
    }

    private IEnumerator SpikeDamageTimer()
    {
        yield return new WaitForSeconds(DamageTime);
        SequenceMGR.instance.ActProc();
    }
}
