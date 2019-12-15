using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMGR : MonoBehaviour
{
    [SerializeField]
    private PlayerControll playerPrefab;

    public enum ActorType
    {
        player = 0, // プレイヤー
        enemy,      // 敵
        max         
    }

    private PlayerControll player = null;
    public PlayerControll Player
    {
        get { return player; }
    }

    private List<Actor> actors = new List<Actor>();

    // Start is called before the first frame update
    void Start()
    {
        this.CreatePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePlayer()
    {
        Point point = MapGenerator.instance.RandomPointInRoom();
        player = Instantiate(playerPrefab, MapData.GridToWorld(point), Quaternion.identity);
        player.status.point = point;
        player.param.id = 0;
        actors.Add(player);
    }

    public void SetEnemy(ref EnemyBase enemy)
    {
        // ユニークなIDを設定する
        enemy.param.id = this.GetID();

        // キャラリストに登録
        actors.Add(enemy);
    }

    /// <summary>
    /// ユニークIDを取得する
    /// </summary>
    private int GetID()
    {
        int cnt = 0;
        
        while(true)
        {
            foreach(var itr in actors)
            {
                if (cnt == itr.param.id)
                {
                    ++cnt;
                    continue;
                }
            }
            return cnt;
        }
    }

    /// <summary>
    /// IDが一致するキャラを検索する
    /// </summary>
    /// <param name="id">目的のID</param>
    /// <returns>検索結果</returns>
    public Actor GetActorFromID(int id)
    {
        // IDが一致するキャラを探す
        foreach(var itr in actors)
        {
            if(itr.param.id == id)
            {
                // 一致したら返す
                return itr;
            }
        }
        // いない場合
        return null;
    }

    #region singleton

    static ActorMGR _instance;

    public static ActorMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(ActorMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use ActorMGR in the scene hierarchy.");
                    _instance = (ActorMGR)previous;
                }
                else
                {
                    var go = new GameObject("ActorMGR");
                    _instance = go.AddComponent<ActorMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}