using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FieldMGR : MonoBehaviour
{
    public const int fieldMax = 10;

    private float initPosY = 0.0f;

    private struct SaveData
    {
        public FieldInformation.GridInfo[,] infos;
    }
    private SaveData saveData = new SaveData();

    FieldInformation.GridInfo[,] gridInfos = new FieldInformation.GridInfo[fieldMax, fieldMax];


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
        if(Input.GetKeyDown(KeyCode.S))
        {
            this.SaveField();
        }
    }

    private void SaveField()
    {
        saveData.infos = new FieldInformation.GridInfo[fieldMax, fieldMax];
        for (int i = 0; i < fieldMax; ++i)
        {
            for (int j = 0; j < fieldMax; ++j)
            {
                saveData.infos[i,j] = gridInfos[i, j];
            }
        }

        StreamWriter writer;

        string jsonstr = JsonUtility.ToJson(saveData);

        writer = new StreamWriter(Application.dataPath + "/savedata.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// 配列からステージを生成する
    /// </summary>
    private void CreateStage()
    {
        for(int i = 0; i < fieldMax; ++i)
        {
            for (int j = 0; j < fieldMax; ++j)
            {
                gridInfos[i, j].GridNum = new Vector2Int(i, j);
                gridInfos[i, j].Type = FieldInformation.FieldType.none;
                gridInfos[i, j].OnFieldObj = null;
            }
        }

        gridInfos[1 + fieldMax / 2, 3 + fieldMax / 2].Type = FieldInformation.FieldType.wall;
        Instantiate(wallPrefab, this.GridToWorld(new Vector2Int(1, 3)), Quaternion.identity);
    }

    public void SetInitY(float posY)
    {
        initPosY = posY;
    }

    public Vector3 GridToWorld(Vector2Int grid)
    {
        Vector3 world = new Vector3(
            (float)grid.x * FieldInformation.GridSize,
            initPosY,
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
