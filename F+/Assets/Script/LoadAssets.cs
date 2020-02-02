using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadAssets : MonoBehaviour
{
    private enum AssetsType
    {
        trap = 0,
        enemy,
        effect,
        max
    }

    private bool[] Progress = new bool[(int)AssetsType.max];

    private GameObject[] TrapPrefabs = new GameObject[(int)TrapMGR.TrapType.max];
    private GameObject[] EnemyPrefabs = new GameObject[(int)EnemyMGR.EnemyType.max];
    private GameObject[] EffectPrefabs = new GameObject[(int)EffectMGR.EffectType.max];

    public GameObject GetEnemyPrefab(EnemyMGR.EnemyType type)
    {
        return EnemyPrefabs[(int)type];
    }

    public GameObject GetTrapPrefab(TrapMGR.TrapType type)
    {
        return TrapPrefabs[(int)type];
    }

    public GameObject GetEffectPrefab(EffectMGR.EffectType type)
    {
        return EffectPrefabs[(int)type];
    }

    // Start is called before the first frame update
    void Start()
    {
        // AddressableAssetSystemによるアセットのロード
        this.AssetLoad();

        DontDestroyOnLoad(this.gameObject);
    }

    private bool isDone = false;
    private void AssetLoad()
    {
        if (!isDone)
        {
            StartCoroutine(this.LoadEffectAsync()); // エフェクトの読み込み
            StartCoroutine(this.LoadEnemyAsync());  // 敵の読み込み
            StartCoroutine(this.LoadTrapAsync());   // 罠の読み込み
            this.LoadSound();
            this.isDone = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.IsDone())
        {
            // すべてロードしたらゲーム開始
        }
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

    private IEnumerator LoadEffectAsync()
    {
        AdDebug.Log("エフェクトのロード中...");
        var op = Addressables.LoadAssetsAsync<GameObject>("Effect", null);

        yield return op;

        List<GameObject> effectList = new List<GameObject>(op.Result);

        foreach(var itr in effectList)
        {
            EffectPrefabs[(int)itr.GetComponent<EffectBase>().EffectType] = itr;
        }

        int cnt = 0;
        foreach (var itr in EffectPrefabs)
        {
            if (itr == null)
            {
                AdDebug.Log($"エフェクトロードエラー！ : {(EffectMGR.EffectType)cnt}が正しくロードされませんでした。");
            }
            ++cnt;
        }

        AdDebug.Log("エフェクトのロード完了！");

        Progress[(int)AssetsType.effect] = true;
    }

    private IEnumerator LoadTrapAsync()
    {
        AdDebug.Log("トラップのロード中...");
        var op = Addressables.LoadAssetsAsync<GameObject>("Trap", null);

        yield return op;

        List<GameObject> trapList = new List<GameObject>(op.Result);

        foreach (var itr in trapList)
        {
            TrapPrefabs[(int)itr.GetComponent<TrapBase>().trapType] = itr;
        }

        int cnt = 0;
        foreach (var itr in TrapPrefabs)
        {
            if (itr == null)
            {
                AdDebug.Log($"トラップロードエラー！ : {(TrapMGR.TrapType)cnt}が正しくロードされませんでした。");
            }
            ++cnt;
        }

        AdDebug.Log("トラップのロード完了！");

        Progress[(int)AssetsType.trap] = true;
    }

    private IEnumerator LoadEnemyAsync()
    {
        AdDebug.Log("敵のロード中...");
        var op = Addressables.LoadAssetsAsync<GameObject>("Enemy", null);

        yield return op;

        List<GameObject> enemyList = new List<GameObject>(op.Result);

        foreach (var itr in enemyList)
        {
            EnemyPrefabs[(int)itr.GetComponent<EnemyBase>().enemyType] = itr;
        }

        int cnt = 0;
        foreach (var itr in EnemyPrefabs)
        {
            if (itr == null)
            {
                AdDebug.Log($"敵ロードエラー！ : {(EnemyMGR.EnemyType)cnt}が正しくロードされませんでした。");
            }
            ++cnt;
        }

        AdDebug.Log("敵のロード完了！");

        Progress[(int)AssetsType.enemy] = true;
    }

    private void LoadSound()
    {
        SoundMGR.LoadBgm("TitleBGM", "TitleBGM");
        SoundMGR.LoadBgm("GameBGM", "GameBGM");
        SoundMGR.LoadSe("Attack", "Attack");
        SoundMGR.LoadSe("BookRead", "BookRead");
        SoundMGR.LoadSe("Choice", "Choice");
        SoundMGR.LoadSe("CollectItem", "CollectItem");
        SoundMGR.LoadSe("Decision", "Decision");
        SoundMGR.LoadSe("Equip", "Equip");
        SoundMGR.LoadSe("Explosion", "Explosion");
        SoundMGR.LoadSe("Footsteps", "Footsteps");
        SoundMGR.LoadSe("Heal", "Heal");
        SoundMGR.LoadSe("Hunger_Recovery", "Hunger_Recovery");
        SoundMGR.LoadSe("LevelUp", "LevelUp");
        SoundMGR.LoadSe("Pitfall", "Pitfall");
        SoundMGR.LoadSe("Start", "Start");
        SoundMGR.LoadSe("Throw", "Throw");
        SoundMGR.LoadSe("Tornado", "Tornado");
        SoundMGR.LoadSe("Trap_Hunger", "Trap_Hunger");
        SoundMGR.LoadSe("Trap_Warp", "Trap_Warp");
        SoundMGR.LoadSe("Miss", "Miss");
        SoundMGR.LoadSe("EnemyDead", "EnemyDead");
        SoundMGR.LoadSe("UI_Return", "UI_Return");
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
