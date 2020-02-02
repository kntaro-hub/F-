using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Trap_Pitfall : TrapBase
{
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
        actor.transform.DOMoveY(-10.0f, 0.8f).SetEase(Ease.InOutBack);

        // 落とし穴音
        SoundMGR.PlaySe("Pitfall");

        SequenceMGR.instance.Player.IsCameraSet = false;

        // 時間間隔
        StartCoroutine(Timer());

        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Trap_Pitfall, point).transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.8f);
        FloorMGR.instance.NextFloor();
    }
}