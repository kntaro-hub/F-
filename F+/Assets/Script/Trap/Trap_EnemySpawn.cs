using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_EnemySpawn : TrapBase
{
    // Start is called before the first frame update
    void Start()
    {
        trapType = TrapType.EnemySpawn;
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
        MessageWindow.instance.AddMessage($"{actor.Param.Name}は呼び寄せの罠にかかった！", Color.white);

        // いま居る階層に出現する敵の中から最大4体生成
        for(int i = 0; i < Random.Range(2,4); ++i)
        {
            EnemyMGR.instance.CreateEnemy_Random(MapData.GetRandomPointFromAround(point));
        }

        // 時間間隔
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(Actor.MoveTime);
        MessageWindow.instance.AddMessage($"…敵に取り囲まれた！", Color.white);
        SequenceMGR.instance.ActProc();
    }
}
