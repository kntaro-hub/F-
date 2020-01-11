using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TrapMGR : MonoBehaviour
{
    private List<TrapBase> trapList = new List<TrapBase>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateTrap(TrapBase.TrapType type)
    {
        Point point = MapGenerator.instance.RandomPointInRoom();

        TrapBase trap = Instantiate(LoadAssets.instance.GetTrapPrefab(type), MapData.GridToWorld(point), Quaternion.identity, this.transform).GetComponent<TrapBase>();

        // ID設定
        trap.SetID(this.SetUniqueID());

        // リストに登録
        this.AddTrapList(trap);

        // 座標設定
        trap.Point = point;

        // マップ情報に登録
        MapData.instance.SetMapObject(point, MapData.MapObjType.trap, trap.GetID());
        MapData.instance.SetMapChip(point, trap);
    }

    private void AddTrapList(TrapBase trap)
    {
        // リストに登録
        trapList.Add(trap);
    }

    public void UpdateMapTrap()
    {
        // マップのオブジェクトを更新
        foreach(var itr in trapList)
        {
            // ほかのオブジェクトに更新されたのを元に戻す
            MapData.instance.SetMapObject(itr.Point, MapData.MapObjType.trap, itr.GetID());
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
