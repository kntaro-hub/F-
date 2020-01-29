using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Hunger : TrapBase
{
    private const int SubHungerValue = 10;
    private const float SubHungerTime = 0.5f;

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
        MessageWindow.instance.AddMessage($"{actor.Param.Name}はハラヘリの罠にかかった！", Color.white);

        actor.SubHunger(SubHungerValue);

        StartCoroutine(this.SubHungerTimer());

        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Hit_White, point);
    }

    private IEnumerator SubHungerTimer()
    {
        yield return new WaitForSeconds(SubHungerTime);
        MessageWindow.instance.AddMessage($"…少しおなかが減った気がする。", Color.white);
        SequenceMGR.instance.ActProc();
    }
}
