using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMGR : MonoBehaviour
{
    public enum EnemyType
    {
        normal = 0, // 通常敵
        max
    }

    [SerializeField] private Enemy_Normal NormalPrefab;

    private List<Actor> enemies = new List<Actor>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateEnemy(EnemyType type, Point point)
    {
        EnemyBase enemy = null;
        switch(type)
        {
            case EnemyType.normal: enemy = Instantiate(NormalPrefab);
                break;
        }

        ActorMGR.instance.SetEnemy(ref enemy);

        enemies.Add(enemy);
    }

    #region singleton

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
