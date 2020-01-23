using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadAssets : MonoBehaviour
{
    private enum AssetsType
    {
        //trap = 0,
        //enemy,
        effect = 0,
        max
    }

    private bool[] Progress = new bool[(int)AssetsType.max];

    //private GameObject[] TrapPrefabs = new GameObject[(int)TrapBase.TrapType.max - 1];
    //private GameObject[] EnemyPrefabs = new GameObject[(int)EnemyMGR.EnemyType.max - 1];

    //public GameObject GetEnemyPrefab(EnemyMGR.EnemyType type)
    //{
    //    return EnemyPrefabs[(int)type];
    //}

    //public GameObject GetTrapPrefab(TrapBase.TrapType type)
    //{
    //    return TrapPrefabs[(int)type];
    //}

    private EffectBase[] EffectPrefabs = new EffectBase[(int)EffectMGR.EffectType.max];

    private Dictionary<string, GameObject> EffectDictionary = new Dictionary<string, GameObject>();

    public GameObject GetEffectDictionary(EffectMGR.EffectType type)
    {
        return EffectDictionary[$"Effect_{type.ToString()}"];
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < (int)EffectMGR.EffectType.max; ++i)
        {
            
        }

        LoadEffect();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.IsDone())
        {
            // すべてロードしたらゲーム開始
            
        }

        //if(Input.GetKeyDown(KeyCode.Y))
        //{
        //    this.AddDic();
        //}
    }

    private bool IsDone()
    {
        foreach (var itr in Progress)
        {
            if (!itr)
            {
                return false;
            }
        }
        return true;
    }

    //public void LoadTrap()
    //{
    //    // ロード済みトラップ数
    //    int cntTrap = 0;

    //    AdDebug.Log("トラップのロード中");
    //    for (int i = 0; i < (int)TrapBase.TrapType.max - 1; ++i)
    //    {
    //        //Assetのロード
    //        Addressables.LoadAssetAsync<GameObject>($"Trap_{((TrapBase.TrapType)cntTrap).ToString()}").Completed += op =>
    //        {
    //            TrapPrefabs[cntTrap] = op.Result;
    //            ++cntTrap;
    //        };
    //    }

    //    AdDebug.Log("トラップのロード完了！");
    //}

    //public void LoadEnemy()
    //{
    //    // ロード済みトラップ数
    //    int cntEnemy = 0;

    //    AdDebug.Log("敵のロード中");
    //    for (int i = 0; i < (int)EnemyMGR.EnemyType.max - 1; ++i)
    //    {
    //        //Assetのロード
    //        Addressables.LoadAssetAsync<GameObject>($"Enemy_{((EnemyMGR.EnemyType)cntEnemy).ToString()}").Completed += op =>
    //        {
    //            EnemyPrefabs[cntEnemy] = op.Result;
    //            ++cntEnemy;
    //        };
    //    }

    //    AdDebug.Log("敵のロード完了！");
    //}

    public void LoadEffect()
    {
        AdDebug.Log("エフェクトのロード中");
        // Assetのロード {((EffectMGR.EffectType)cntEffect).ToString()}
        Addressables.LoadAssetsAsync<GameObject>("Effect", this.OnLoadEffect);
    }

    int cntEffect = 0;
    private void OnLoadEffect(GameObject effect)
    {
        EffectDictionary.Add($"Effect_{(effect.GetComponent<EffectBase>().EffectType).ToString()}", effect);
        ++cntEffect;

        if (cntEffect == (int)EffectMGR.EffectType.max)
        {
            AdDebug.Log("エフェクトのロード完了！");
        }
    }

    #region singleton

    static LoadAssets _instance;

    public static LoadAssets instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(LoadAssets));
                if (previous)
                {
                    _instance = (LoadAssets)previous;
                }
                else
                {
                    var go = new GameObject("LoadAssets");
                    _instance = go.AddComponent<LoadAssets>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
