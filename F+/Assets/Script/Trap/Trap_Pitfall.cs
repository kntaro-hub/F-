﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Trap_Pitfall : TrapBase
{
    // Start is called before the first frame update
    void Start()
    {
        trapType = TrapType.Pitfall;
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
        actor.transform.DOMoveY(-10.0f, 0.8f).SetEase(Ease.InOutBack);

        SequenceMGR.instance.Player.IsCameraSet = false;

        // 時間間隔
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.8f);
        FloorMGR.instance.NextFloor();
    }
}
