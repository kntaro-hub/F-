using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_EnemySpawn : TrapBase
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
        MessageWindow.instance.AddMessage($"{actor.Param.Name}は呼び寄せの罠にかかった！", Color.white);

        // トラップ音再生
        SoundMGR.PlaySe("Explosion", 0.6f);

        // 罠のまわりから最大4体分の座標を被りなしで取得
        List<Point> pointList = RandomPointList(Random.Range(2, 4), point);

        // いま居る階層に出現する敵の中から生成
        foreach(var itr in pointList)
        {
            // 敵生成
            EnemyMGR.instance.CreateEnemy_Random(itr);
        }

        GameObject effect = EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Trap_EnemySpawn_Hit, point);
        effect.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
        effect.transform.rotation = Quaternion.Euler(90.0f,0.0f,0.0f);

        // 時間間隔
        StartCoroutine(Timer());
    }

    /// <summary>
    /// 被りなしで範囲内をnum個取得
    /// </summary>
    public List<Point> RandomPointList(int num, Point center)
    {
        List<Point> randomList = new List<Point>();

        for (int i = 0; i < num; ++i)
        {
            while (true)
            {
                Point randomPoint = MapData.GetRandomPointFromAround(center);

                // リストの中に被りがあるか走査
                foreach (var itr in randomList)
                {
                    if (itr == randomPoint)
                    {
                        // 再抽選
                        continue;
                    }
                }
                // 被りがなければ格納する
                randomList.Add(randomPoint);
                break;
            }
        }
        return randomList;
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(Actor.MoveTime);
        MessageWindow.instance.AddMessage($"…敵に取り囲まれた！", Color.white);
        SequenceMGR.instance.ActProc();
    }
}
