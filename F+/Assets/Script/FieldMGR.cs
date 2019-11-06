using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMGR : MonoBehaviour
{
    FieldInformation.GridInfo[,] gridInfos = new FieldInformation.GridInfo[10,10];
    public FieldInformation.GridInfo[,] GridInfos
    {
        get { return gridInfos; }
    }

    [SerializeField] private GameObject wallPrefab;
    

    // Start is called before the first frame update
    void Start()
    {
        this.CreateStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 配列からステージを生成する
    /// </summary>
    private void CreateStage()
    {
        for(int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                gridInfos[i, j].GridNum = new Vector2Int(i, j);
                gridInfos[i, j].Type = FieldInformation.FieldType.none;
                gridInfos[i, j].OnFieldObj = null;
            }
        }

        gridInfos[6, 5].Type = FieldInformation.FieldType.wall;
        Instantiate(wallPrefab, this.GridToWorld(new Vector2Int(6, 5)), Quaternion.identity);
    }

    public Vector3 GridToWorld(Vector2Int grid)
    {
        Vector3 world = new Vector3(
            (float)grid.x * FieldInformation.GridSize,
            0.0f,
            (float)grid.y * FieldInformation.GridSize);

        return world;
    }

    public Vector2Int WorldToGrid(Vector3 world)
    {
        Vector2Int grid = new Vector2Int(
            (int)(world.x / FieldInformation.GridSize),
            (int)(world.z / FieldInformation.GridSize));

        return grid;
    }

    #region singleton

    static FieldMGR _instance;

    public static FieldMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(FieldMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use MidiBridge in the scene hierarchy.");
                    _instance = (FieldMGR)previous;
                }
                else
                {
                    var go = new GameObject("MessageWindow");
                    _instance = go.AddComponent<FieldMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
