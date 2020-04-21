using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TrapMGR : MonoBehaviour
{
    // =--------- 列挙体定義 ---------= //
    public enum TrapType
    {
        Warp = 0,
        Spike,
        Hunger,
        EnemySpawn,
        Pitfall,
        max
    }

    // 生成したトラップリスト
    private List<TrapBase> trapList = new List<TrapBase>();

    private static readonly int TrapNum_Min = 0;
    private static readonly int TrapNum_Max = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateTrap()
    {
        for (int i = 0; i < Random.Range(TrapNum_Min, TrapNum_Max); ++i)
        {
            if (Percent.Per(30))
            {
                Point point = MapGenerator.instance.RandomPointInRoom();

                TrapBase trap =
                    Instantiate(LoadAssets.instance.GetTrapPrefab(
                        (TrapType)Random.Range(0, (int)TrapType.max)),
                        MapData.GridToWorld(point),
                        Quaternion.identity,
                        this.transform)
                    .GetComponent<TrapBase>();

                // ID設定
                trap.SetID(this.SetUniqueID());

                // 座標設定
                trap.point = point;

                // マップ情報に登録
                MapData.instance.SetMapObject(point, MapData.MapObjType.trap, trap.GetID());
                MapData.instance.SetMapChip(point, trap);

                // リストに登録
                trapList.Add(trap);
            }
        }
    }

    /// <summary>
    /// ユニークなIDを設定する
    /// </summary>
    /// <returns>決定したID</returns>
    private int SetUniqueID()
    {
        int ret = 0;
        while (true)
        {
            bool isContinue = false;
            foreach (var itr in trapList)
            {// 1件でも同じIDがあれば次の数字へ
                if (itr.GetID() == ret)
                {
                    // 数字を進める
                    ++ret;
                    isContinue = true;
                    break;
                }
            }
            if(isContinue) continue;
            return ret;
        }
    }

    #region singleton

    static TrapMGR _instance;

    public static TrapMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(TrapMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use TrapMGR in the scene hierarchy.");
                    _instance = (TrapMGR)previous;
                }
                else
                {
                    var go = new GameObject("TrapMGR");
                    _instance = go.AddComponent<TrapMGR>();
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
