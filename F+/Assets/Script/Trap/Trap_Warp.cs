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

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ActiveTrap()
    {

    }

    // =--------- コルーチン ---------= //
    private IEnumerator WarpTimer()
    {
        yield return new WaitForSeconds(WarpTime * 0.5f);


        yield return new WaitForSeconds(WarpTime * 0.5f);


    }
}
