using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EnemyMGR : MonoBehaviour
{
    public static float MinAppearanceTime = 5.0f;   // 敵出現最小秒数
    public static float MaxAppearanceTime = 10.0f;  // 敵出現最大秒数
    public static int MaxEnemy = 6;

    public enum EnemyType
    {
        Normal = 0,
        Around,
        max
    }

    private List<EnemyBase> enemyList = new List<EnemyBase>();
    public List<EnemyBase> EnemyList
    {
        get { return enemyList; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator AppearanceTimer(float time)
    {
        yield return new WaitForSeconds(time);

        List<EnemyTableEntity> Candidates = new List<EnemyTableEntity>();
        foreach (var itr in DataBase.instance.GetEnemyTable())
        {
            if(FloorMGR.instance.FloorNum <= itr.MaxFloor &&
                FloorMGR.instance.FloorNum >= itr.MinFloor)
            {
                Candidates.Add(itr);
            }
        }

        int SumAppearance = 0;
        foreach (var itr in Candidates)
        {
            SumAppearance += itr.Appearance;
        }

        //0〜累計確率の間で乱数を作成
        float rand = Random.Range(0, SumAppearance);

        //乱数から各確率を引いていき、0未満になったら終了
        foreach (var itr in Candidates)
        {
            rand -= itr.Appearance;

            if (rand <= 0)
            {
                this.CreateEnemy((EnemyType)itr.TypeID);
                break;
            }
        }
    }


    public void CreateEnemy(EnemyType enemyType)
    {
        Point point = MapGenerator.instance.RandomPointInRoom();

        StartCoroutine(this.LoadEnemy(enemyType, point));
    }

    public void CreateEnemy_Random()
    {
        List<EnemyTableEntity> Candidates = new List<EnemyTableEntity>();
        foreach (var itr in DataBase.instance.GetEnemyTable())
        {
            if (FloorMGR.instance.FloorNum <= itr.MaxFloor &&
                FloorMGR.instance.FloorNum >= itr.MinFloor)
            {
                Candidates.Add(itr);
            }
        }

        int SumAppearance = 0;
        foreach (var itr in Candidates)
        {
            SumAppearance += itr.Appearance;
        }

        //0〜累計確率の間で乱数を作成
        float rand = Random.Range(0, SumAppearance);

        //乱数から各確率を引いていき、0未満になったら終了
        foreach (var itr in Candidates)
        {
            rand -= itr.Appearance;

            if (rand <= 0)
            {
                this.CreateEnemy((EnemyType)itr.TypeID);
                break;
            }
        }
    }

    private IEnumerator LoadEnemy(EnemyType type, Point point)
    {
        //Alchemist_ManというAddressのSpriteを非同期でロードの開始
        var handle = Addressables.InstantiateAsync($"Enemy_{type.ToString()}", MapData.GridToWorld(point), Quaternion.identity, MapData.instance.transform);

        //ロードが完了するまで待機
        yield return new WaitUntil(() => handle.IsDone);

        EnemyBase enemy = null;
        // エラーがなければロードしたSpriteの名前表示
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            {
                // 生成したオブジェクトからトラップを取得
                enemy = handle.Result.GetComponent<EnemyBase>();
            };

            // ID設定
            enemy.SetID(this.SetUniqueID());

            // 敵座標設定
            enemy.SetPoint(point);

            // 途中で出現した敵はターンエンド状態からスタート
            enemy.status.actType = Actor.ActType.TurnEnd;

            // マップUI上の敵生成
            UI_MGR.instance.Ui_Map.CreateMapEnemy(point);

            // リストに登録
            this.AddEnemyList(enemy);

            // マップ情報に登録
            MapData.instance.SetMapObject(point, MapData.MapObjType.enemy, enemy.GetID());

        }
        //エラー表示
        else
        {
            Debug.LogError(handle.Status);
        }
    }

    private void AddEnemyList(EnemyBase enemy)
    {
        // リストに登録
        enemyList.Add(enemy);
    }

    public void UpdateMapEnemy()
    {
        // マップのオブジェクトを更新
        foreach (var itr in enemyList)
        {
            // ほかのオブジェクトに更新されたのを元に戻す
            MapData.instance.SetMapObject(itr.GetPoint(), MapData.MapObjType.enemy, itr.GetID());
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
            foreach (var itr in enemyList)
            {// 1件でも同じIDがあれば次の数字へ
                if (itr.GetID() == ret)
                {
                    // 数字を進める
                    ++ret;
                    isContinue = true;
                    break;
                }
            }
            if (isContinue) continue;
            return ret;
        }
    }

    /// <summary>
    /// 指定したIDの敵を消去する
    /// </summary>
    /// <param name="id">消去する対象のID</param>
    /// <param name="IsXp">プレイヤーに経験値が入るか</param>
    /// <returns>消去成否</returns>
    public bool DestroyEnemy(int id, bool isXp)
    {
        // 敵を走査
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            // IDが一致する敵がいたら消去
            if (enemyList[i].Param.id == id)
            {
                enemyList[i].DestroyObject(isXp);
                enemyList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    #region シングルトン

    static EnemyMGR _instance;

    public static EnemyMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(EnemyMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use EnemyMGR in the scene hierarchy.");
                    _instance = (EnemyMGR)previous;
                }
                else
                {
                    var go = new GameObject("EnemyMGR");
                    _instance = go.AddComponent<EnemyMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
